using Microsoft.AspNetCore.Identity;

namespace IntegraMailing.Data
{
    public class ApplicationUser: IdentityUser
    {
        public string? AccountType { get; set; }
    }
}
