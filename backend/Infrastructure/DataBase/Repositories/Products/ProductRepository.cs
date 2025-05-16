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
    }

    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) { }

        public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize,
            Expression<Func<Product, bool>>? filter = null)
        {
            var query = filter != null ? DbSet.Where(filter) : DbSet;
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
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
    }
}
