// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/HttpCacheHeaders

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Marvin.Cache.Headers.Sample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddResponseCaching();

            // Add HttpCacheHeaders services with custom options
            services.AddHttpCacheHeaders(
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // add Microsoft's ResponseCaching middleware to the request pipeline (with InMemory cache store)
            app.UseResponseCaching();

            // add HttpCacheHeaders middleware to the request pipeline
            app.UseHttpCacheHeaders();

            app.UseMvc();
        }
    }
}
