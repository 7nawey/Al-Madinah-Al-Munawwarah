using Microsoft.AspNetCore.Identity;

namespace AlMadina.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public string Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public string Role { get; set; } = "User";
    }
}