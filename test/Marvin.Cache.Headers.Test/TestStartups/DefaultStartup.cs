// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marvin.Cache.Headers.Test.TestStartups
{
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
                switch (context.Request.Path)
                {
                    case "/bad-request":
                        context.Response.StatusCode = 400;
                        break;
                    case "/server-error":
                        context.Response.StatusCode = 500;
                        break;
                    case "/not-found":
                        context.Response.StatusCode = 404;
                        break;
                    default:
                        context.Response.StatusCode = 200;
                        break;
                }
                
                await context.Response.WriteAsync($"Hello from {nameof(DefaultStartup)}");
            });
        }
    }
}