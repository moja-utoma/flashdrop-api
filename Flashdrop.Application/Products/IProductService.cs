using Flashdrop.Application.Products.DTOs.Requests;
using Flashdrop.Application.Products.DTOs.Response;

namespace Flashdrop.Application.Products;

public interface IProductService
{
    Task<IEnumerable<ProductForListDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateProductRequest productDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateProductRequest productDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
