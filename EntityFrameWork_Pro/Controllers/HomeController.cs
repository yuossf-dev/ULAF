using System.Diagnostics;
using EntityFrameWork_Pro.Models;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameWork_Pro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult SetLanguage(string lang, string returnUrl)
        {
            Response.Cookies.Append("Language", lang, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });
            return Redirect(returnUrl ?? "/");
        }

        public IActionResult Welcome()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
