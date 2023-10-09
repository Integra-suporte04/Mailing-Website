using IntegraMailing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IntegraMailing.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult SignUp()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(string name, string pass, string email, DateTime dob, string accountType)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Name = name,
                Password = pass,
                DateOfBirth = dob,
                AccountType = accountType
            };

            var result = await _userManager.CreateAsync(user, pass);

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
    }
}
