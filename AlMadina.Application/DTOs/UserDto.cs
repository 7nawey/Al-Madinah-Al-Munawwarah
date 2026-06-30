using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlMadina.Application.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Role { get; set; }
    }

    public class RegisterUserDto
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Address { get; set; }

        public string? PhoneNumber { get; set; }
    }

    public class UpdateUserDto
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }
    }

    public class LoginUserDto
    {
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; }

    }
}
