using IntegraMailing.Data;
using IntegraMailing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IntegraMailing.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUser _currentUser;
        public HomeController(UserManager<ApplicationUser> userManager, ApplicationUser currentUser)
        {

            _userManager = userManager;
            _currentUser = currentUser;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            await GetUserInfo();

            return View();
        }
        [Authorize]
        public async Task<IActionResult> Lista()
        {
            // _logger.LogInformation("Página Listas foi acessada.");
            await GetUserInfo();

            var listaViewModel = LoadCSVController.listaViewModel;
            return RedirectToAction("GetCampanhas", "LoadCSV");
            return View("~/Views/Home/Lista.cshtml", listaViewModel);
        }
        public async Task<IActionResult> Perfil()
        {
            // _logger.LogInformation("Página Listas foi acessada.");
            await GetUserInfo();

            
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private async Task GetUserInfo()
        {
            _currentUser = await _userManager.GetUserAsync(User);
            if (_currentUser != null)
            {
                ViewData["AccountType"] = _currentUser.AccountType;
                ViewData["UserEmail"] = _currentUser.Email;
                ViewData["UserName"] = _currentUser.UserName;

            }
        }
    }
}