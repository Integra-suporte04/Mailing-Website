using IntegraMailing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IntegraMailing.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;

namespace IntegraMailing.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult SignUp()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpGet("CheckEmail")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
               return Json(false); // Email já está em uso
            }


            return Json(true); // Email disponível
        }

        [HttpPost]
        public async Task<IActionResult> Register(string name, string pass, string email)
        {
            var user = new ApplicationUser
            {
                UserName = name,
                Email = email,
                PhoneNumber = "039120391",
                LockoutEnd = DateTime.Now,
                AccountType = "Usuario"
            };

            var result = await _userManager.CreateAsync(user, pass.ToString());

            if (result.Succeeded)
            {
                // usuário criado com sucesso, possivelmente redirecionar para a página de login
                return RedirectToAction("SignIn");
            }
            else
            {
                // se houver algum erro, adicione-os ao ModelState e retorne à vista de registro
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("~/Views/Account/SignUp.cshtml");
            }

        }

        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(SignInModel model)
        {
            if (ModelState.IsValid)
            {
                Debug.WriteLine(model.Email.GetType());
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        Debug.WriteLine(User.Identity.IsAuthenticated);
                        return RedirectToAction("Index", "Home");  // Redirecionar para a página inicial ou a página desejada
                    }
                    
                }
                else
                {
                   Debug.WriteLine("user is null!");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View("~/Views/Account/SignIn.cshtml",model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("SignIn", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(string field, int fieldId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View("~/Views/Home/Perfil.cshtml");
            }

            switch (fieldId)
            {
                case 0:
                    user.UserName = field;
                    break;
                case 1:
                    user.Email = field;
                    break;
                case 2:
                    //_userManager.ChangePasswordAsync(user, _userManager.pass, );
                    break;
            }
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Perfil", "Home");
        }







    }
}
