using AutoMapper;
using Flashdrop.Application.Common.Exceptions;
using Flashdrop.Application.Interfaces.Repositories;
using Flashdrop.Application.Sales;
using Flashdrop.Application.Sales.DTOs.Requests;
using Flashdrop.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Flashdrop.UnitTests.Application.Sales;

public class SalesServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IRepository<Sale>> _salesRepository = new();
    private readonly IMapper _mapper;
    private readonly SalesService _service;

    public SalesServiceTests()
    {
        var loggerFactory = new Mock<ILoggerFactory>();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<SalesProfile>(), loggerFactory.Object);
        _mapper = configuration.CreateMapper();
        _service = new SalesService(_unitOfWork.Object, _salesRepository.Object, _mapper,
            Mock.Of<IDefaultEntitiesProvider>());
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsMappedSales()
    {
        // Arrange
        var sales = new[] { CreateSale(), CreateSale() };
        _salesRepository.Setup(x => x.GetAll()).Returns(sales.AsQueryable());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(sales.Select(x => x.Id), result.Select(x => x.Id));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoSalesExist()
    {
        // Arrange
        _salesRepository.Setup(x => x.GetAll()).Returns(Array.Empty<Sale>().AsQueryable());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PropagatesRepositoryException()
    {
        // Arrange
        _salesRepository.Setup(x => x.GetAll()).Throws(new InvalidOperationException());

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetAllAsync());
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedSale_WhenFound()
    {
        // Arrange
        var sale = CreateSale();
        _salesRepository.Setup(x => x.GetByIdAsync(sale.Id, It
            .IsAny<CancellationToken>())).ReturnsAsync(sale);

        // Act
        var result = await _service.GetByIdAsync(sale.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sale.Id, result.Id);
        Assert.Equal(sale.AvailableStock, result.AvailableStock);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenMissing()
    {
        // Arrange
        _salesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        // Act
        var result = await _service.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_PropagatesRepositoryException()
    {
        // Arrange
        _salesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetByIdAsync(Guid.NewGuid()));
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_CreatesScheduledSaleAndSaves()
    {
        // Arrange
        var request = new CreateSaleRequest
        {
            ProductId = Guid.NewGuid(), TotalStock = 12, PricePerUnit = 4m, MaxUnitsPerUser = 2,
            StartsAt = DateTimeOffset.UtcNow, EndsAt = DateTimeOffset.UtcNow.AddDays(1)
        };
        Sale? created = null;
        _salesRepository.Setup(x => x.CreateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .Callback<Sale, CancellationToken>((sale, _) => created = sale)
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var id = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(created);
        Assert.Equal(id, created.Id);
        Assert.Equal(request.TotalStock, created.AvailableStock);
        Assert.Equal(SaleStatus.Scheduled, created.Status);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_PropagatesRepositoryException()
    {
        // Arrange
        _salesRepository.Setup(x => x.CreateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(new CreateSaleRequest()));
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_PropagatesSaveException()
    {
        // Arrange
        _salesRepository.Setup(x => x.CreateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(new CreateSaleRequest()));
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_UpdatesExistingSaleAndSaves()
    {
        // Arrange
        var sale = CreateSale();
        var request = new UpdateSaleRequest
        {
            TotalStock = 20, PricePerUnit = 9m, MaxUnitsPerUser = 3,
            StartsAt = DateTimeOffset.UtcNow, EndsAt = DateTimeOffset.UtcNow.AddDays(2), RowVersion = [1]
        };
        _salesRepository.Setup(x => x.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sale);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _service.UpdateAsync(sale.Id, request);

        // Assert
        Assert.Equal(20, sale.TotalStock);
        Assert.Equal(9m, sale.PricePerUnit);
        Assert.Equal(request.RowVersion, sale.RowVersion);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_PropagatesRepositoryException()
    {
        // Arrange
        _salesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateAsync(Guid.NewGuid(), new UpdateSaleRequest()));
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_PropagatesSaveException()
    {
        // Arrange
        var sale = CreateSale();
        _salesRepository.Setup(x => x.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sale);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UpdateAsync(sale.Id, new UpdateSaleRequest()));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_DeletesExistingSaleAndSaves()
    {
        // Arrange
        var sale = CreateSale();
        _salesRepository.Setup(x => x.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sale);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(sale.Id);

        // Assert
        _salesRepository.Verify(x => x.Delete(sale, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFound_WhenMissing()
    {
        // Arrange
        _salesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(Guid.NewGuid()));
        _salesRepository.Verify(x => x.Delete(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_PropagatesSaveException()
    {
        // Arrange
        var sale = CreateSale();
        _salesRepository.Setup(x => x.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sale);
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync(sale.Id));
    }

    #endregion

    #region Test Helpers

    private static Sale CreateSale() => new()
    {
        Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), TotalStock = 10, AvailableStock = 7,
        PricePerUnit = 5m, MaxUnitsPerUser = 2, StartsAt = DateTimeOffset.UtcNow,
        EndsAt = DateTimeOffset.UtcNow.AddDays(1), Status = SaleStatus.Live, RowVersion = [1]
    };

    #endregion
}
