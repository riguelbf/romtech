using Domain.Products;
using Infrastructure.DataBase.DataContext;
using Infrastructure.DataBase.Repositories.Base;

namespace Infrastructure.DataBase.Repositories.Products
{
    public interface IProductRepository : IRepository<Product>
    {
    }

    public class ProductRepository(ApplicationDbContext context) : Repository<Product>(context), IProductRepository;
}
