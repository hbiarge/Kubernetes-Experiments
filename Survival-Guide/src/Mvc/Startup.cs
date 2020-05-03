using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mvc.Configuration;
using Mvc.Models;
using Shared;
using Shared.Extensions;
using Shared.HealthChecks;

namespace Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ProxyConfiguration = ProxyConfiguration
                .FromConfiguration(configuration);
        }

        public IConfiguration Configuration { get; }

        public ProxyConfiguration ProxyConfiguration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMiddlewareAnalysis()
                .AddControllersWithViews()
                .Services
                .AddDataProtection()
                .Services
                .Configure<AppConfiguration>(Configuration.GetSection(AppConfiguration.Key))
                .AddSingleton<SampleSingleton>()
                .AddApplicationHealthChecks()
                .AddDistributedMemoryCache();
        }

        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory,
            DiagnosticListener diagnosticListener)
        {
            // Listen for middleware events and log them to the console.
            var listener = new TestDiagnosticListener("/", loggerFactory);
            diagnosticListener.SubscribeWithAdapter(listener);

            app.UseProxySupportIfNecessary(ProxyConfiguration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapApplicationHealthChecks();
            });
        }
    }
}
