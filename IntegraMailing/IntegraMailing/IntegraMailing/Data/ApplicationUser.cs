using Microsoft.AspNetCore.Identity;

namespace IntegraMailing.Data
{
    public class ApplicationUser: IdentityUser
    {
        public string? AccountType { get; set; }
        public string? Empresa { get; set; }
    }
}
