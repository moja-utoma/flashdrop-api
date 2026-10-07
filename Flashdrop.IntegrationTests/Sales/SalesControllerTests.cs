using System.Net;
using System.Net.Http.Json;
using Flashdrop.Application.Common.Exceptions;
using Flashdrop.Application.Sales.DTOs.Requests;
using Flashdrop.Application.Sales.DTOs.Responses;

namespace Flashdrop.IntegrationTests.Sales;

public class SalesControllerTests : IClassFixture<SalesApiFactory>
{
    private readonly HttpClient _client;
    private readonly FakeSalesService _salesService;

    public SalesControllerTests(SalesApiFactory factory)
    {
        _client = factory.CreateClient();
        _salesService = factory.SalesService;
        _salesService.Reset();
    }

    #region GetAll Tests
    [Fact]
    public async Task GetAll_ReturnsSales()
    {
        // Arrange
        _salesService.Sales = [new SaleForListDto { Id = Guid.NewGuid(), TotalStock = 5, AvailableStock = 3 }];

        // Act
        var response = await _client.GetAsync("/api/sales");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<SaleForListDto>>();
        Assert.Single(result!);
    }

    [Fact]
    public async Task GetAll_ReturnsInternalServerError_WhenServiceFails()
    {
        // Arrange
        _salesService.ExceptionToThrow = new InvalidOperationException("service failure");

        // Act
        var response = await _client.GetAsync("/api/sales");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        _salesService.ExceptionToThrow = null;
    }
    #endregion

    #region GetById Tests
    [Fact]
    public async Task GetById_ReturnsSale_WhenFound()
    {
        // Arrange
        var sale = new SaleDto { Id = Guid.NewGuid(), TotalStock = 8, AvailableStock = 6 };
        _salesService.Sale = sale;

        // Act
        var response = await _client.GetAsync($"/api/sales/{sale.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SaleDto>();
        Assert.Equal(sale.Id, result!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenServiceReturnsNull()
    {
        // Arrange
        _salesService.Sale = null;

        // Act
        var response = await _client.GetAsync($"/api/sales/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenServiceThrowsNotFound()
    {
        // Arrange
        _salesService.ExceptionToThrow = new NotFoundException("Sale", Guid.NewGuid());

        // Act
        var response = await _client.GetAsync($"/api/sales/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _salesService.ExceptionToThrow = null;
    }
    #endregion

    #region Create Tests
    [Fact]
    public async Task Create_ReturnsSaleId()
    {
        // Arrange
        var id = Guid.NewGuid();
        _salesService.CreatedId = id;

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", ValidCreateRequest());

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(id, await response.Content.ReadFromJsonAsync<Guid>());
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = ValidCreateRequest();
        request.TotalStock = 0;

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsInternalServerError_WhenServiceFails()
    {
        // Arrange
        _salesService.ExceptionToThrow = new InvalidOperationException("service failure");

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", ValidCreateRequest());

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        _salesService.ExceptionToThrow = null;
    }
    #endregion

    #region Update Tests
    [Fact]
    public async Task Update_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.PutAsJsonAsync($"/api/sales/{id}", ValidUpdateRequest());

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = ValidUpdateRequest();
        request.RowVersion = [];

        // Act
        var response = await _client.PutAsJsonAsync($"/api/sales/{Guid.NewGuid()}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenServiceThrowsNotFound()
    {
        // Arrange
        _salesService.ExceptionToThrow = new NotFoundException("Sale", Guid.NewGuid());

        // Act
        var response = await _client.PutAsJsonAsync($"/api/sales/{Guid.NewGuid()}", ValidUpdateRequest());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _salesService.ExceptionToThrow = null;
    }
    #endregion

    #region Delete Tests
    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/sales/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenServiceThrowsNotFound()
    {
        // Arrange
        _salesService.ExceptionToThrow = new NotFoundException("Sale", Guid.NewGuid());

        // Act
        var response = await _client.DeleteAsync($"/api/sales/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _salesService.ExceptionToThrow = null;
    }
    #endregion

    #region Test Helpers
    private static CreateSaleRequest ValidCreateRequest() => new()
    {
        ProductId = Guid.NewGuid(), TotalStock = 10, PricePerUnit = 4m, MaxUnitsPerUser = 2,
        StartsAt = DateTimeOffset.UtcNow.AddHours(1), EndsAt = DateTimeOffset.UtcNow.AddHours(2)
    };

    private static UpdateSaleRequest ValidUpdateRequest() => new()
    {
        TotalStock = 10, PricePerUnit = 4m, MaxUnitsPerUser = 2,
        StartsAt = DateTimeOffset.UtcNow.AddHours(1), EndsAt = DateTimeOffset.UtcNow.AddHours(2), RowVersion = [1]
    };
    #endregion
}
