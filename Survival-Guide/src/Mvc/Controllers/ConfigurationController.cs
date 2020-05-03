using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mvc.Configuration;
using Mvc.Models;

namespace Mvc.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly IOptions<AppConfiguration> _configurationOptions;
        private readonly IOptionsSnapshot<AppConfiguration> _configurationSnapshot;
        private readonly SampleSingleton _singleton;

        public ConfigurationController(
            IOptions<AppConfiguration> configurationOptions,
            IOptionsSnapshot<AppConfiguration> configurationSnapshot,
            SampleSingleton singleton)
        {
            _configurationOptions = configurationOptions;
            _configurationSnapshot = configurationSnapshot;
            _singleton = singleton;
        }

        public IActionResult Index()
        {
            return View(new ConfigurationModel
            {
                OptionsConfiguration = _configurationOptions.Value,
                SnapshotConfiguration = _configurationSnapshot.Value,
                OptionsMonitorConfiguration = _singleton.Configuration
            });
        }
    }
}
