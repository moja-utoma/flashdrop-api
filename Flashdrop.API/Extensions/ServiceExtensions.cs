using Flashdrop.API.ExceptionHandling;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Flashdrop.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddFluentValidationAutoValidation();

        return services;
    }

}