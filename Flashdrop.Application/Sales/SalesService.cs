using AutoMapper;
using AutoMapper.QueryableExtensions;
using Flashdrop.Application.Common.Extensions;
using Flashdrop.Application.Interfaces.Repositories;
using Flashdrop.Application.Sales.DTOs.Requests;
using Flashdrop.Application.Sales.DTOs.Responses;
using Flashdrop.Domain.Entities;

namespace Flashdrop.Application.Sales;

public class SalesService(
    IUnitOfWork unitOfWork,
    IRepository<Sale> salesRepository,
    IMapper mapper,
    IDefaultEntitiesProvider defaultEntities) : ISalesService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IRepository<Sale> _salesRepository = salesRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IDefaultEntitiesProvider _defaultEntitiesProvider = defaultEntities;

    public async Task<IEnumerable<SaleForListDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sales = _salesRepository.GetAll()
            .ProjectTo<SaleForListDto>(_mapper.ConfigurationProvider)
            .ToList();
        return sales;
    }

    public async Task<SaleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _salesRepository.GetByIdAsync(id, cancellationToken);
        if (sale is null)
            return null;

        return _mapper.Map<SaleDto>(sale);
    }

    public async Task<Guid> CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default)
    {
        var sale = _mapper.Map<Sale>(request);

        sale.AvailableStock = sale.TotalStock;
        sale.Status = SaleStatus.Scheduled;

        await _salesRepository.CreateAsync(sale, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return sale.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateSaleRequest request, CancellationToken cancellationToken = default)
    {
        var sale = await _salesRepository.GetByIdAsync(id, cancellationToken);
        _mapper.Map(request, sale);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _salesRepository.GetByIdOrThrowAsync(id, cancellationToken);
        _salesRepository.Delete(sale, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}