using Mvc.Configuration;

namespace Mvc.Models
{
    public class ConfigurationModel
    {
        public AppConfiguration OptionsConfiguration { get; set; }

        public AppConfiguration SnapshotConfiguration { get; set; }

        public AppConfiguration OptionsMonitorConfiguration { get; set; }
    }
}