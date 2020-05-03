using Microsoft.AspNetCore.Mvc;

namespace Mvc.Controllers
{
    public class VariablesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
