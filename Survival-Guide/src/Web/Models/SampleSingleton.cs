using Microsoft.Extensions.Options;
using Web.Configuration;

namespace Web.Models
{
    public class SampleSingleton
    {
        private readonly IOptionsMonitor<AppConfiguration> _configuration;

        public SampleSingleton(IOptionsMonitor<AppConfiguration> configuration)
        {
            _configuration = configuration;
        }

        public AppConfiguration Configuration => _configuration.CurrentValue;
    }
}