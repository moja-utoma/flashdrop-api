using Flashdrop.Application.Sales.DTOs.Requests;
using Flashdrop.Application.Sales.DTOs.Responses;

namespace Flashdrop.Application.Sales;

public class SalesService : ISalesService
{
    public async Task<IEnumerable<SaleDto>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<SaleDto> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(UpdateSaleRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}