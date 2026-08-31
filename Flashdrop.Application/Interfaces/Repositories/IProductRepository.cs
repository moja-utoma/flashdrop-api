using Flashdrop.Data.Entities;

namespace Flashdrop.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    IQueryable<Product> GetAll();
    Task Create(Product product);
    Task Delete(Guid id);
}
