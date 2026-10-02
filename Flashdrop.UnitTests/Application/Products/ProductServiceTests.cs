using AutoMapper;
using Flashdrop.Application.Common.Exceptions;
using Flashdrop.Application.Interfaces.Repositories;
using Flashdrop.Application.Products;
using Flashdrop.Application.Products.DTOs.Requests;
using Flashdrop.Data.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Flashdrop.UnitTests.Application.Products;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRepository<Product>> _mockProductRepository;
    private readonly IMapper _mapper;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProductRepository = new Mock<IRepository<Product>>();

        var mockLoggerFactory = new Mock<ILoggerFactory>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductsProfile>();
        }, mockLoggerFactory.Object);

        mapperConfig.AssertConfigurationIsValid();
        _mapper = new Mapper(mapperConfig);

        _productService = new ProductService(
            _mockUnitOfWork.Object,
            _mockProductRepository.Object,
            _mapper);
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedProducts_WhenProductsExist()
    {
        // Arrange
        var products = new List<Product>
        {
            CreateTestProduct(id: Guid.NewGuid(), name: "Product 1"),
            CreateTestProduct(id: Guid.NewGuid(), name: "Product 2")
        };

        _mockProductRepository
            .Setup(x => x.GetAll())
            .Returns(products.AsQueryable());

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("Product 1", resultList[0].Name);
        Assert.Equal("Product 2", resultList[1].Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoProductsExist()
    {
        // Arrange
        _mockProductRepository
            .Setup(x => x.GetAll())
            .Returns(new List<Product>().AsQueryable());

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldCallRepositoryGetAll_Once()
    {
        // Arrange
        _mockProductRepository
            .Setup(x => x.GetAll())
            .Returns(new List<Product>().AsQueryable());

        // Act
        await _productService.GetAllAsync();

        // Assert
        _mockProductRepository.Verify(x => x.GetAll(), Times.Once);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMappedProduct_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateTestProduct(id: productId, name: "Test Product");

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(product.Description, result.Description);
        Assert.Equal(product.BasePrice, result.BasePrice);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldCallRepositoryGetByIdAsync_WithCorrectId()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        await _productService.GetByIdAsync(productId);

        // Assert
        _mockProductRepository.Verify(
            x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldPassCancellationToken_ToRepository()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId, cancellationToken))
            .ReturnsAsync((Product?)null);

        // Act
        await _productService.GetByIdAsync(productId, cancellationToken);

        // Assert
        _mockProductRepository.Verify(
            x => x.GetByIdAsync(productId, cancellationToken),
            Times.Once);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldCreateProductAndSaveChanges_WhenValidRequestProvided()
    {
        // Arrange
        var createRequest = new CreateProductRequest
        {
            Name = "Test Product",
            Description = "Test Description",
            BasePrice = 99.99m,
            Category = ProductCategory.Electronics
        };

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _productService.CreateAsync(createRequest);

        // Assert
        _mockProductRepository.Verify(
            x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldMapRequestToProduct_WhenCalledWithValidData()
    {
        // Arrange
        var createRequest = new CreateProductRequest
        {
            Name = "New Product",
            Description = "New Description",
            BasePrice = 49.99m,
            Category = ProductCategory.Fashion
        };

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _productService.CreateAsync(createRequest);

        // Assert
        _mockProductRepository.Verify(
            x => x.CreateAsync(
                It.Is<Product>(p => p.Name == "New Product" && p.BasePrice == 49.99m),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldPassCancellationTokenToRepository_AndUnitOfWork()
    {
        // Arrange
        var createRequest = new CreateProductRequest
        {
            Name = "Test",
            Description = "Test",
            BasePrice = 10m,
            Category = ProductCategory.Other
        };

        var cancellationToken = new CancellationToken();

        _mockProductRepository
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), cancellationToken))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(1);

        // Act
        await _productService.CreateAsync(createRequest, cancellationToken);

        // Assert
        _mockProductRepository.Verify(
            x => x.CreateAsync(It.IsAny<Product>(), cancellationToken),
            Times.Once);
        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(cancellationToken),
            Times.Once);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProductAndSaveChanges_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var updateRequest = new UpdateProductRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            BasePrice = 199.99m,
            Category = ProductCategory.Electronics
        };

        var existingProduct = CreateTestProduct(id: productId);

        // Mock the underlying GetByIdAsync method for the extension method to call
        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _productService.UpdateAsync(productId, updateRequest);

        // Assert
        _mockProductRepository.Verify(
            x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var updateRequest = new UpdateProductRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            BasePrice = 199.99m,
            Category = ProductCategory.Electronics
        };

        // Mock GetByIdAsync to return null, which will trigger the extension method to throw
        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        // When extension method doesn't find the product, it throws NotFoundException
        await Assert.ThrowsAsync<NotFoundException>(
            () => _productService.UpdateAsync(productId, updateRequest));
    }

    [Fact]
    public async Task UpdateAsync_ShouldPassCancellationTokenToRepository_AndUnitOfWork()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var updateRequest = new UpdateProductRequest
        {
            Name = "Updated",
            Description = "Updated",
            BasePrice = 10m,
            Category = ProductCategory.Other
        };

        var cancellationToken = new CancellationToken();
        var product = CreateTestProduct(id: productId);

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId, cancellationToken))
            .ReturnsAsync(product);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(1);

        // Act
        await _productService.UpdateAsync(productId, updateRequest, cancellationToken);

        // Assert
        _mockProductRepository.Verify(
            x => x.GetByIdAsync(productId, cancellationToken),
            Times.Once);
        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(cancellationToken),
            Times.Once);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProductAndSaveChanges_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = CreateTestProduct(id: productId);

        // Mock the underlying GetByIdAsync for the extension method
        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _mockProductRepository
            .Setup(x => x.Delete(product, It.IsAny<CancellationToken>()));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _productService.DeleteAsync(productId);

        // Assert
        _mockProductRepository.Verify(
            x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
            Times.Once);
        _mockProductRepository.Verify(
            x => x.Delete(product, It.IsAny<CancellationToken>()),
            Times.Once);
        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Mock GetByIdAsync to return null, extension method will throw NotFoundException
        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _productService.DeleteAsync(productId));
    }

    [Fact]
    public async Task DeleteAsync_ShouldPassCancellationTokenToRepository_AndUnitOfWork()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();
        var product = CreateTestProduct(id: productId);

        _mockProductRepository
            .Setup(x => x.GetByIdAsync(productId, cancellationToken))
            .ReturnsAsync(product);

        _mockProductRepository
            .Setup(x => x.Delete(product, cancellationToken));

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(1);

        // Act
        await _productService.DeleteAsync(productId, cancellationToken);

        // Assert
        _mockProductRepository.Verify(
            x => x.GetByIdAsync(productId, cancellationToken),
            Times.Once);
        _mockProductRepository.Verify(
            x => x.Delete(product, cancellationToken),
            Times.Once);
        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(cancellationToken),
            Times.Once);
    }

    #endregion

    #region Test Helpers

    private Product CreateTestProduct(
        Guid? id = null,
        Guid? sellerId = null,
        string? name = null,
        string? description = null,
        decimal basePrice = 99.99m,
        ProductCategory category = ProductCategory.Electronics)
    {
        return new Product
        {
            Id = id ?? Guid.NewGuid(),
            SellerId = sellerId ?? Guid.NewGuid(),
            Name = name ?? "Test Product",
            Description = description ?? "Test Description",
            BasePrice = basePrice,
            Category = category,
            CreatedAt = DateTimeOffset.UtcNow,
            ImageUrl = "https://example.com/image.jpg"
        };
    }

    #endregion
}
