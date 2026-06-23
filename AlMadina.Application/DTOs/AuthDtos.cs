namespace AlMadina.Application.DTOs
{
    public class SendOtpDto
    {
        public string Email { get; set; }
    }

    public class VerifyOtpDto
    {
        public string Email { get; set; }

        public string Otp { get; set; }
    }

    public class GoogleLoginDto
    {
        public string IdToken { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }

        public UserDto User { get; set; }
    }
}