using Domain.Products;
using Infrastructure.DataBase.DataContext;
using Infrastructure.DataBase.Repositories.Base;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase.Repositories.Products
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<(List<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<Product, bool>>? filter = null);
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product, CancellationToken cancellationToken);
        Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken);
        Task<bool> AddStockAsync(int id, int quantity, CancellationToken cancellationToken);
    }

    public class ProductRepository(ApplicationDbContext context) : Repository<Product>(context), IProductRepository
    {
        public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize,
            Expression<Func<Product, bool>>? filter = null)
        {
            var baseFilter = (Expression<Func<Product, bool>>)(p => !p.IsDeleted);
            var query = filter != null ? DbSet.Where(baseFilter).Where(filter) : DbSet.Where(baseFilter);
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await DbSet.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task AddAsync(Product product)
        {
            await DbSet.AddAsync(product);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            DbSet.Update(product);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken)
        {
            var product = await DbSet.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
            
            if (product == null)
                return false;
            
            product.IsDeleted = true;
            DbSet.Update(product);
            
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> AddStockAsync(int id, int quantity, CancellationToken cancellationToken)
        {
            const int maxRetries = 3;
            var retries = 0;
            var delayMs = 100;

            while (retries < maxRetries)
            {
                var product = await DbSet.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

                if (product == null)
                    return false;

                product.AddStock(quantity);
                DbSet.Update(product);

                try
                {
                    await Context.SaveChangesAsync(cancellationToken);
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    retries++;
                    
                    if (retries >= maxRetries)
                        return false;
                    
                    await Task.Delay(delayMs, cancellationToken);
                    delayMs *= 2;
                    Context.Entry(product).State = EntityState.Detached;
                }
            }
            return false;
        }
    }
}
