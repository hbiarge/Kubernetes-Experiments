using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;
using Shared.Extensions;
using Shared.HealthChecks;
using Web.Configuration;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
            ProxyConfiguration = ProxyConfiguration.FromConfiguration(configuration);
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public ProxyConfiguration ProxyConfiguration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllersWithViews()
                .Services
                .Configure<AppConfiguration>(Configuration.GetSection(AppConfiguration.Key))
                .AddApplicationHealthChecks();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseProxySupportIfNecessary(ProxyConfiguration);

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
