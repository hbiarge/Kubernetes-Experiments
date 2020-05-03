using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace Shared.HealthChecks
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapApplicationHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("health/live", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(Constants.Health.LivenessTag)
            });

            endpoints.MapHealthChecks("health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(Constants.Health.ReadinessTag)
            });

            return endpoints;
        }
    }
}