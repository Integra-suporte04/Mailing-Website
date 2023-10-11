using Microsoft.AspNetCore.Identity;

namespace IntegraMailing.Data
{
    public class ApplicationUser: IdentityUser
    {
        //public DateTime DateOfBirth { get; set; }
        public string? AccountType { get; set; }
    }
}
