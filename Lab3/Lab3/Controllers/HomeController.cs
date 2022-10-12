using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult SongForm() => View();

        [HttpPost]
        public IActionResult Sing()
        {
            if (Request.Form["numBottles"] == "")
                return View("SongForm");
            else
                TempData["NumBottles"] = Request.Form["numBottles"];

            return View();
        }

        public IActionResult CreatePerson() => View();

        [HttpPost]
        public IActionResult DisplayPerson(Person person) => View(person);

        public IActionResult Error() => View();
    }
}
