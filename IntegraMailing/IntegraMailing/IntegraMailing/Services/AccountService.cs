// Localização do arquivo: /Services/AccountService.cs

using IntegraMailing.Data;
using IntegraMailing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;


namespace IntegraMailing.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task GetUserInfoAsync(Controller controller)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user != null)
            {
                controller.ViewData["AccountType"] = user.AccountType;
                controller.ViewData["UserEmail"] = user.Email;
                controller.ViewData["UserName"] = user.UserName;
            }
        }

        public void GetUserInfo(ApplicationUser user, Controller controller)
        {
            if (user != null)
            {
                controller.ViewData["AccountType"] = user.AccountType;
                controller.ViewData["UserEmail"] = user.Email;
                controller.ViewData["UserName"] = user.UserName;
            }
        }
    }

    public interface IAccountService
    {
        Task GetUserInfoAsync(Controller controller);
        void GetUserInfo(ApplicationUser user, Controller controller);
    }
}
