using IntegraMailing.Data;
using IntegraMailing.Models;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace IntegraMailing.Services
{
    public class ListaService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUser _currentUser;

        public ListaService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ListaViewModel> GetListaViewModelAsync(ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            var listaViewModel = new ListaViewModel
            {
                // ... inicialize sua ViewModel aqui ...
                
            };

            return listaViewModel;
        }

        public async Task<UserInfo> GetUserInfo(ClaimsPrincipal user)
        {
            _currentUser = await _userManager.GetUserAsync(user);

            
            if (_currentUser != null)
            {
                return new UserInfo
                {
                    appUser = _currentUser,
                    accountType = _currentUser.AccountType,
                    email = _currentUser.Email,
                    userName = _currentUser.UserName
                };

            }
            else
            {
                return new UserInfo
                {

                };
            }
        }

        public struct UserInfo
        {
            public ApplicationUser appUser;
            public string accountType;
            public string email;
            public string userName;

        }
    }
}
