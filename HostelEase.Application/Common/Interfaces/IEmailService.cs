

namespace HostelEase.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendRegistrationConfirmationEmailAsync(string toEmail, string firstName, string confirmationLink);

        Task ResendConfirmationEmailAsync(string toEmail, string firstName, string confirmationLink);
        Task SendAccountCreatedEmailAsync(string toEmail, string firstName, string loginLink);
        Task SendPasswordResetEmailAsync(string toEmail, string firstNmae, string resetLink);
    }
}
