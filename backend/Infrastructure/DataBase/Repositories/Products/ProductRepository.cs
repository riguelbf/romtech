using Domain.Products;
using Infrastructure.DataBase.DataContext;
using Infrastructure.DataBase.Repositories.Base;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Queries;

namespace Infrastructure.DataBase.Repositories.Products
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<(List<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<Product, bool>>? filter = null);
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
    }
}
