using AutoMapper;
using AutoMapper.QueryableExtensions;
using Flashdrop.Application.Common.Extensions;
using Flashdrop.Application.Interfaces.Repositories;
using Flashdrop.Application.Products.DTOs.Requests;
using Flashdrop.Application.Products.DTOs.Response;
using Flashdrop.Data.Entities;

namespace Flashdrop.Application.Products;

public class ProductService
    (IUnitOfWork unitOfWork,
    IRepository<Product> productRepository,
    IMapper mapper) : IProductService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IRepository<Product> _productRepository = productRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<ProductForListDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = _productRepository.GetAll()
            .ProjectTo<ProductForListDto>(_mapper.ConfigurationProvider)
            .ToList();
        return products;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return null;

        return _mapper.Map<ProductDto>(product);
    }

    public async Task CreateAsync(CreateProductRequest productDto, CancellationToken cancellationToken = default)
    {
        var product = _mapper.Map<Product>(productDto);
        await _productRepository.CreateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Guid id, UpdateProductRequest productDto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdOrThrowAsync(id, cancellationToken);
        _mapper.Map(productDto, product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdOrThrowAsync(id, cancellationToken);
        _productRepository.Delete(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
