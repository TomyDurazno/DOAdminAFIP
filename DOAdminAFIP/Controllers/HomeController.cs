using Microsoft.AspNetCore.Mvc;

namespace DOAdminAFIP.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Hello!";
            return View();
        }
    }
}
