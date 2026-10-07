using Flashdrop.Application.Sales.DTOs.Requests;
using Flashdrop.Application.Sales.DTOs.Responses;

namespace Flashdrop.Application.Sales;

public interface ISalesService
{
    Task<IEnumerable<SaleForListDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SaleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateSaleRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}