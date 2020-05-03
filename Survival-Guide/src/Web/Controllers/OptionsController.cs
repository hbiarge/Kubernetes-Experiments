using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Configuration;
using Web.Models;

namespace Web.Controllers
{
    public class OptionsController : Controller
    {
        private readonly IOptions<AppConfiguration> _configurationOptions;
        
        public OptionsController(IOptions<AppConfiguration> configurationOptions)
        {
            _configurationOptions = configurationOptions;
        }

        public IActionResult Index()
        {
            return View(new ConfigurationModel
            {
                OptionsConfiguration = _configurationOptions.Value,
            });
        }
    }
}
