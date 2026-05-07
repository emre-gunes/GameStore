using Microsoft.AspNetCore.Identity;

namespace GameStore.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
