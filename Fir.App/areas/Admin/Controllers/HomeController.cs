using Microsoft.AspNetCore.Mvc;

namespace Fir.App.areas.Admin.Controllers
{
        [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
