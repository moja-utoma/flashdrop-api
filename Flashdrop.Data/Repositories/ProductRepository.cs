using Flashdrop.Application.Interfaces.Repositories;
using Flashdrop.Domain.Entities;
using Flashdrop.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Flashdrop.Persistence.Repositories;

public class ProductRepository
    (FlashdropDbContext context) : IRepository<Product>
{
    private readonly FlashdropDbContext _context = context;

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FindAsync(id, cancellationToken);
    }

    public IQueryable<Product> GetAll()
    {
        return _context.Products;
    }

    public async Task CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public void Delete(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(product);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AnyAsync(p => p.Id == id, cancellationToken);
    }
}
