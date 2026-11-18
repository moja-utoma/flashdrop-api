using Flashdrop.API.Extensions;
using Flashdrop.Application;
using Flashdrop.Infrastructure;
using Flashdrop.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Flashdrop API v1");
    });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seed database if empty
if (app.Environment.IsDevelopment())
{
    await app.Services.SeedDatabaseAsync();
}

app.Run();

public partial class Program;
