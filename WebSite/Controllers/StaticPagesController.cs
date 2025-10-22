using Microsoft.AspNetCore.Mvc;

namespace WebSite.Controllers
{
    public class StaticPagesController : Controller
    {
        [Route("privacy/index.php")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("cookie/index.php")]
        public IActionResult Cookie()
        {
            return View();
        }

        [Route("terms/index.php")]
        public IActionResult Terms()
        {
            return View();
        }

        [Route("about/index.php")]
        public IActionResult About()
        {
            return View();
        }
        [Route("allgame/index.php")]
        public IActionResult Allgame()
        {
            return View();
        }

        [Route("contact/index.php")]
        public IActionResult Contact()
        {
            return View();
        }
   
    }
}