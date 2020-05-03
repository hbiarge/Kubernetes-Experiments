using Microsoft.Extensions.DependencyInjection;

namespace Shared.HealthChecks
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<ConfigurableHealthCheck>(Constants.Health.LivenessCheckName, tags: new[] { Constants.Health.LivenessTag })
                .AddCheck<ConfigurableHealthCheck>(Constants.Health.ReadynessCheckName, tags: new[] { Constants.Health.ReadinessTag });

            services.AddSingleton<HealthCheckState>();

            return services;
        }
    }
}