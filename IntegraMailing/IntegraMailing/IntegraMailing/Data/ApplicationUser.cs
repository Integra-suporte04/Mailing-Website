using Microsoft.AspNetCore.Identity;

namespace IntegraMailing.Data
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string AccountType { get; set; }
    }
}
