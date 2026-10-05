using Flashdrop.API.Extensions;
using Flashdrop.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seed database if empty
if (app.Environment.IsDevelopment())
{
    await app.Services.SeedDatabaseAsync();
}

app.Run();
