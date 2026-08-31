using Flashdrop.Application.Interfaces.Repositories;

namespace Flashdrop.Data;

public class UnitOfWork
    (FlashdropDbContext context) : IUnitOfWork
{
    private readonly FlashdropDbContext _context = context;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
