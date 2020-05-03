using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Extensions;

namespace Shared.HealthChecks
{
    public class ConfigurableHealthCheck : IHealthCheck
    {
        private readonly HealthCheckState _state;

        public ConfigurableHealthCheck(HealthCheckState state)
        {
            _state = state;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            if (context.Registration.Name == Constants.Health.LivenessCheckName)
            {
                var status = _state.IsLive
                    ? HealthStatus.Healthy
                    : HealthStatus.Unhealthy;

                var result = new HealthCheckResult(status);
                return Task.FromResult(result);
            }

            if (context.Registration.Name == Constants.Health.ReadynessCheckName)
            {
                var status = _state.IsReady
                    ? HealthStatus.Healthy
                    : HealthStatus.Unhealthy;

                var result = new HealthCheckResult(status);
                return Task.FromResult(result);
            }

            var defaultResult = new HealthCheckResult(HealthStatus.Healthy);
            return Task.FromResult(defaultResult);
        }
    }
}