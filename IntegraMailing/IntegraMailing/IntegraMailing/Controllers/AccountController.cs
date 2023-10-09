using Microsoft.AspNetCore.Mvc;

namespace IntegraMailing.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignUp()
        {
            return View();
        }
    }
}
