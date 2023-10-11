using IntegraMailing.Data;
using IntegraMailing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IntegraMailing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["AccountType"] = user.AccountType;
                ViewData["UserEmail"] = user.Email;
                ViewData["UserName"] = user.UserName;

                Debug.WriteLine("User is NOT null");
                _logger.LogInformation("a");

            }
            else
            {

                Debug.WriteLine("User is null");
                _logger.LogInformation("b");

                // ... o restante do seu código ...

            }
            return View();
        }

        public IActionResult Lista()
        {
           // _logger.LogInformation("Página Listas foi acessada.");
            var listaViewModel = LoadCSVController.listaViewModel;
            return View("~/Views/Home/Lista.cshtml", listaViewModel);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}