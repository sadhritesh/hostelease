
using HostelEase.Application.Common.Result;
using HostelEase.Application.Features.AccountDTO;

namespace HostelEase.Application.Interfaces.ServiceContracts
{
    public interface IAccountService
    {
        Task<Result> RegisterUserAsync(RegisterUserDto registerUserDTO);
        Task<Result> ConfirmEmailAsync(Guid userId, string token);
        Task<Result> LoginUserAsync(LoginUserDto loginUserDto);
        Task<Result> LogoutUserAsync();
        Task<Result> SendEmailConfirmationAsync(string email);
        Task<Result<ProfileDto>> GetUserProfileByEmailAsync(string email);
        Task<Result> SendResetPasswordLinkAsync(ForgotPasswordDto forgotPasswordDto);
        Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    }
}
