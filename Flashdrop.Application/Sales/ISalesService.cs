using Flashdrop.Application.Sales.DTOs.Requests;
using Flashdrop.Application.Sales.DTOs.Responses;

namespace Flashdrop.Application.Sales;

public interface ISalesService
{
    Task<IEnumerable<SaleDto>> GetAll();
    Task<SaleDto> GetById(Guid id, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateSaleRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}