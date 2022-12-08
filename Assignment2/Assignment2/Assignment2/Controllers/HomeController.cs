using Microsoft.AspNetCore.Mvc;

namespace Assignment2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
