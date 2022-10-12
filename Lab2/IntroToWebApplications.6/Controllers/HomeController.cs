using IntroToWebApplications._6.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IntroToWebApplications._6.Controllers
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
            return View();
        }

        [HttpPost]
        public IActionResult Index(Person person)
        {
            ViewData["Message"] = "Your application processed some data.";
            // read more on ViewDataAttributes as homework ;)

            if (person == null || !ModelState.IsValid)
            {
                Person invalidPerson = new()
                {
                    FirstName = "Invalid"
                };
                return View(invalidPerson);
            }
            return View(person);
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