using Microsoft.AspNetCore.Mvc;

namespace ServerSideZip.Controllers
{
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}