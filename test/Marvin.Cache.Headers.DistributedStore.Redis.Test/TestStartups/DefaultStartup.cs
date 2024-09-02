// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marvin.Cache.Headers.Test.TestStartups;

public class DefaultStartup
{
    public DefaultStartup()
    {
        var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddHttpCacheHeaders();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseHttpCacheHeaders();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        app.Run(async context =>
        {
            await context.Response.WriteAsync($"Hello from {nameof(DefaultStartup)}");
        });
    }
}