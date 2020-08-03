using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DOAdminAFIP.Models;

namespace DOAdminAFIP.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Hello!";
            return View(new ErrorViewModel() { });
        }
    }
}
