using Flashdrop.Application.Products;
using Flashdrop.Application.Sales;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Flashdrop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddAutoMapper(cfg => { }, assembly);
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISalesService, SalesService>();

        return services;
    }
}
