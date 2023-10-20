using IntegraMailing.Data;
using IntegraMailing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace IntegraMailing.Controllers
{
    public class HomeController : Controller
    {

        public static HomeModel homeModel = new HomeModel
        {

        };
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUser _currentUser;
        private readonly ApplicationDbContext _context;
        public HomeController(UserManager<ApplicationUser> userManager, ApplicationUser currentUser, ApplicationDbContext context)
        {

            _userManager = userManager;
            _currentUser = currentUser;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            _currentUser = await _userManager.GetUserAsync(User);
            homeModel.empresas = await _context.Empresas.ToListAsync();
            GetUserInfo(_currentUser);


            return View("~/Views/Home/Index.cshtml", homeModel);
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
        [HttpPost]
        public async Task<IActionResult> ApplyUserToEnterprise(string empresaId, string nomeUsuario)
        {
            var user = await _userManager.FindByEmailAsync(nomeUsuario);
            

            if(user != null)
            {
                user.Empresa = empresaId;
                await _context.SaveChangesAsync();

            }

            return await Index();
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
        public async Task<IActionResult> CreateEmpresa(string nomeEmpresa)
        {

            _context.Empresas.Add(new Empresas { Nome = nomeEmpresa });

            await _context.SaveChangesAsync();

            return await Index();

        }
        private void GetUserInfo(ApplicationUser user)
        {
            if (user != null)
            {
                ViewData["AccountType"] = user.AccountType;
                ViewData["UserEmail"] = user.Email;
                ViewData["UserName"] = user.UserName;

            }
        }
    }
}