using Flashdrop.Application.Sales;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Flashdrop.IntegrationTests.Sales;

public class SalesApiFactory : WebApplicationFactory<Program>
{
    public FakeSalesService SalesService { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ISalesService>();
            services.AddSingleton<ISalesService>(SalesService);
        });
    }
}
