using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Shared.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseProxySupportIfNecessary(this IApplicationBuilder app, ProxyConfiguration proxyConfiguration)
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("Acheve");

            switch (proxyConfiguration.ProxyBehavior)
            {
                case ProxyBehavior.NoProxy when proxyConfiguration.IsDefaultPathBase:
                    
                    logger.LogInformation(
                        "Proxy behavior: {proxyBehavior}. No proxy configuration required",
                        proxyConfiguration.ProxyBehavior);

                    break;
                case ProxyBehavior.NoProxy:
                    
                    logger.LogWarning(
                            "Proxy behavior is {proxyBehavior} but a non default PathBase has been configured {PathBase}. Review your configuration",
                            proxyConfiguration.ProxyBehavior,
                            proxyConfiguration.DefaultOrCurrentPathBaseWithoutTrailingSlash);

                    break;
                case ProxyBehavior.MaintainProxyPath when proxyConfiguration.IsDefaultPathBase:
                    
                    logger.LogWarning(
                        "Proxy behavior is {proxyBehavior} but no PathBase has been configured. Review your configuration",
                        proxyConfiguration.ProxyBehavior,
                        proxyConfiguration.DefaultOrCurrentPathBaseWithoutTrailingSlash);

                    break;
                case ProxyBehavior.MaintainProxyPath:
                    
                    logger.LogInformation(
                        "Proxy behavior: {proxyBehavior}. Using PathBase middleware with path: {PathBase}",
                        proxyConfiguration.ProxyBehavior,
                        proxyConfiguration.DefaultOrCurrentPathBaseWithoutTrailingSlash);

                    app.UsePathBase(new PathString(proxyConfiguration.DefaultOrCurrentPathBaseWithoutTrailingSlash));

                    break;
                case ProxyBehavior.RemoveProxyPath when proxyConfiguration.IsDefaultPathBase:
                    
                    logger.LogWarning(
                        "Proxy behavior is {proxyBehavior} but no PathBase has been configured. Review your configuration",
                        proxyConfiguration.ProxyBehavior,
                        proxyConfiguration.DefaultOrCurrentPathBaseWithoutTrailingSlash);

                    break;
                case ProxyBehavior.RemoveProxyPath:
                    
                    logger.LogInformation(
                        "Proxy behavior: {proxyBehavior}. Set PathBase manually with path: {PathBase}",
                        proxyConfiguration.ProxyBehavior,
                        proxyConfiguration.DefaultOrCurrentPathBaseWithoutTrailingSlash);

                    app.Use((context, next) =>
                    {
                        context.Request.PathBase = proxyConfiguration.DefaultOrCurrentPathBaseWithoutTrailingSlash;

                        return next();
                    });

                    break;
            }

            return app;
        }

        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, ProxyConfiguration proxyConfiguration)
        {
            var shouldUseBasePath = proxyConfiguration.ProxyBehavior != ProxyBehavior.NoProxy
                                    && proxyConfiguration.IsDefaultPathBase == false;

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(options =>
            {
                if (shouldUseBasePath)
                {
                    options.RouteTemplate = "swagger/{documentName}/swagger.json";
                    options.PreSerializeFilters.Add((document, request) =>
                    {
                        document.Servers = new List<OpenApiServer>
                        {
                            new OpenApiServer
                            {
                                Url = proxyConfiguration.DefaultOrCurrentPathBaseWithoutTrailingSlash
                            }
                        };
                    });
                }
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            var url = shouldUseBasePath
                ? $"{proxyConfiguration.DefaultOrCurrentPathBaseWithoutTrailingSlash}/swagger/v1/swagger.json"
                : "/swagger/v1/swagger.json";

            app.UseSwaggerUI(
                options =>
                {
                    options.SwaggerEndpoint(url, "API V1");
                });

            return app;
        }
    }
}