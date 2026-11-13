using Flashdrop.Application.Interfaces.Repositories;
using Flashdrop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flashdrop.Persistence.Repositories;

public class SaleRepository(FlashdropDbContext context) : IRepository<Sale>
{
    private readonly FlashdropDbContext _context = context;
    
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.FindAsync(id, cancellationToken);
    }

    public IQueryable<Sale> GetAll()
    {
        return _context.Sales;
    }

    public async Task CreateAsync(Sale entity, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(entity, cancellationToken); 
    }

    public void Delete(Sale entity, CancellationToken cancellationToken = default)
    {
        _context.Sales.Remove(entity);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.AnyAsync(e => e.Id == id, cancellationToken);
    }
}