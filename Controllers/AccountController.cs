using ChatBotCoachWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatBotCoachWebsite.Controllers
{
    public class AccountController : Controller
    {
        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public ActionResult Verify(Account account)
        {
            return View();
        }
    }
}
