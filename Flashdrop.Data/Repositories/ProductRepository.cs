using Flashdrop.Application.Interfaces.Repositories;
using Flashdrop.Data.Entities;

namespace Flashdrop.Data.Repositories;

public class ProductRepository
    (FlashdropDbContext context) : IProductRepository
{
    private readonly FlashdropDbContext _context = context;

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public IQueryable<Product> GetAll()
    {
        return _context.Products;
    }

    public async Task Create(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public async Task Delete(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
