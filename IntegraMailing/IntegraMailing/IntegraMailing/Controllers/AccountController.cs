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
        public async Task<IActionResult> Register(string pass, string email)
        {
            var user = new ApplicationUser
            {
                UserName = email,
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

                return RedirectToAction("SignUp");
            }

        }

        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(SignInModel model)
        {
            if (ModelState.IsValid)
            {
                Debug.WriteLine("model is valid");
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                if (user != null)
                {
                    Debug.WriteLine("BeforeResult");
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                    Debug.WriteLine(result);
                    if (result.Succeeded)
                    {
                        Debug.WriteLine("Authenticated: "+User.Identity.IsAuthenticated);
                        Debug.WriteLine("Remember me: "+model.RememberMe);
                        return RedirectToAction("Index", "Home");  // Redirecionar para a página inicial ou a página desejada
                    }
                    
                }
                else
                {
                   Debug.WriteLine("user is null!");
                }
                Debug.WriteLine("Failed attempt");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return RedirectToAction("SignIn");
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
                    break;
                case 1:
                    user.Email = field;
                    user.UserName = field;
                    break;

            }
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Perfil", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string NewPassword)
        {

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                ModelState.AddModelError(string.Empty, "A nova senha não pode estar vazia.");
                return View();  // ou outro tratamento de erro
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
                return View();  // ou outro tratamento de erro
            }

            // Remove a senha atual (se houver)
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                // Handle errors (log, show error message to user, etc.)
                // Você pode querer logar ou lidar com quaisquer erros aqui
                return BadRequest("Erro ao alterar senha. Remover a senha atual falhou");  // ou outro tratamento de erro
            }

            // Adiciona a nova senha
            var addPasswordResult = await _userManager.AddPasswordAsync(user, NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                // Handle errors (log, show error message to user, etc.)
                // Você pode querer logar ou lidar com quaisquer erros aqui
                return BadRequest("Erro ao alterar senha. Adicionar a nova senha falhou, favor contactar o suporte");  // ou outro tratamento de erro
            }

            // Se chegou aqui, a operação foi bem-sucedida
            return RedirectToAction("Perfil", "Home");
        }

    }
}
