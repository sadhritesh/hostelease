
namespace HostelEase.Application.Features.AccountDTO
{
    public class LoginUserDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
