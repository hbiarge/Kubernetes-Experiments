using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;
using Shared.Extensions;
using Shared.HealthChecks;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ProxyConfiguration = ProxyConfiguration.FromConfiguration(configuration);
        }

        public IConfiguration Configuration { get; }

        public ProxyConfiguration ProxyConfiguration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .Services
                .AddSingleton(sp => ProxyConfiguration)
                .AddMemoryCache()
                .AddDistributedMemoryCache()
                .AddSwagger()
                .AddApplicationHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseProxySupportIfNecessary(ProxyConfiguration);

            // No HttpsRedirection or HSTS. There will be TLS termination at Ingress level

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger(ProxyConfiguration);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapApplicationHealthChecks();
            });
        }
    }
}
