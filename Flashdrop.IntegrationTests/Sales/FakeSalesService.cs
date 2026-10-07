using Flashdrop.Application.Sales;
using Flashdrop.Application.Sales.DTOs.Requests;
using Flashdrop.Application.Sales.DTOs.Responses;

namespace Flashdrop.IntegrationTests.Sales;

public class FakeSalesService : ISalesService
{
    public IEnumerable<SaleForListDto> Sales { get; set; } = [];
    public SaleDto? Sale { get; set; }
    public Guid CreatedId { get; set; } = Guid.NewGuid();
    public Exception? ExceptionToThrow { get; set; }

    public void Reset()
    {
        Sales = [];
        Sale = null;
        CreatedId = Guid.NewGuid();
        ExceptionToThrow = null;
    }

    public Task<IEnumerable<SaleForListDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfConfigured();
        return Task.FromResult(Sales);
    }

    public Task<SaleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ThrowIfConfigured();
        return Task.FromResult(Sale);
    }

    public Task<Guid> CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default)
    {
        ThrowIfConfigured();
        return Task.FromResult(CreatedId);
    }

    public Task UpdateAsync(Guid id, UpdateSaleRequest request, CancellationToken cancellationToken = default)
    {
        ThrowIfConfigured();
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ThrowIfConfigured();
        return Task.CompletedTask;
    }

    private void ThrowIfConfigured()
    {
        if (ExceptionToThrow is not null)
        {
            throw ExceptionToThrow;
        }
    }
}
