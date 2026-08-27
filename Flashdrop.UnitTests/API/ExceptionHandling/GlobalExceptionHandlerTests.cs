using Flashdrop.API.ExceptionHandling;
using Flashdrop.Application.Common.Exceptions;
using Flashdrop.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace Flashdrop.UnitTests.API.ExceptionHandling;

public class GlobalExceptionHandlerTests
{
    private readonly GlobalExceptionHandler _handler;

    public GlobalExceptionHandlerTests()
    {
        _handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);
    }

    [Fact]
    public async Task NotFoundException_ReturnsStatus404()
    {
        var context = CreateHttpContext();
        var exception = new NotFoundException(nameof(Sale), Guid.NewGuid());

        var handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }

    [Fact]
    public async Task AppValidationException_ReturnsStatus400WithErrors()
    {
        var context = CreateHttpContext();
        var errors = new Dictionary<string, string[]> { ["quantity"] = new[] { "Must be greater than 0." } };
        var exception = new AppValidationException(errors);

        var handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task UnknownException_ReturnsStatus500()
    {
        var context = CreateHttpContext();
        var exception = new InvalidOperationException("Something broke.");

        var handled = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }
}
