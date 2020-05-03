using Microsoft.AspNetCore.Mvc;
using Shared.HealthChecks;

namespace Mvc.Controllers
{
    public class HealthController : Controller
    {
        private readonly HealthCheckState _healthCheckState;

        public HealthController(HealthCheckState healthCheckState)
        {
            _healthCheckState = healthCheckState;
        }

        public IActionResult Index()
        {
            return View(_healthCheckState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetNotReady()
        {
            _healthCheckState.SetNotReady();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetReady()
        {
            _healthCheckState.SetReady();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetNotLive()
        {
            _healthCheckState.SetNotLive();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetLive()
        {
            _healthCheckState.SetLive();

            return RedirectToAction("Index");
        }
    }
}
