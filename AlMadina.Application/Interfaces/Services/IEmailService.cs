namespace AlMadina.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendOtpAsync(
            string email,
            string otp);
    }
}