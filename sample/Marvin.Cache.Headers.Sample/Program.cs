var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add HttpCacheHeaders services with custom options
builder.Services.AddHttpCacheHeaders(
    expirationModelOptions =>
    {
        expirationModelOptions.MaxAge = 600;
        expirationModelOptions.SharedMaxAge = 300;
    },
    validationModelOptions =>
    {
        validationModelOptions.MustRevalidate = true;
        validationModelOptions.ProxyRevalidate = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.UseHttpCacheHeaders();

app.MapControllers();

app.Run();
