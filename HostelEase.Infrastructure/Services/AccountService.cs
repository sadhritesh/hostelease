using HostelEase.Application.Common.Interfaces;
using HostelEase.Application.Common.Result;
using HostelEase.Application.Features.AccountDTO;
using HostelEase.Application.Interfaces.ServiceContracts;
using HostelEase.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;


namespace HostelEase.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountService> _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountService(ILogger<AccountService> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Result> ConfirmEmailAsync(Guid userId, string token)
        {
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("UserId and token are required");
                return Result.Failure(new[] { "Invalid userId or token." });
            }

            _logger.LogInformation("Confirm Email is executing for {userId}", userId);

            _logger.LogDebug("Searching for user with {userId}", userId);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                _logger.LogWarning("Email confirmation failed. User {UserId} was not found.", userId);
                return Result.Failure(new[] { "User not found" });
            }
            _logger.LogDebug("User found successfully. Checking if email is confirmed for {email}", user.Email);

            if (user.EmailConfirmed)
            {
                _logger.LogInformation(
                    "Email already confirmed for {Email}",
                    user.Email);

                return Result.Success("Email is already confirmed.");
            }

            _logger.LogDebug("Decoding confirmation token for {email}", user.Email);

            string decodedToken;

            try
            {
                var decodedBytes = WebEncoders.Base64UrlDecode(token);
                decodedToken = Encoding.UTF8.GetString(decodedBytes);
            } 
            catch (FormatException ex)
            {
                _logger.LogWarning(ex, "Invalid confirmation token for {Email}", user.Email);
                return Result.Failure(new[] { "Invalid confirmation token." });
            }

            _logger.LogDebug("Confirming Email for {userId}", userId);

            var identityResult = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!identityResult.Succeeded)
            {
                _logger.LogWarning("Email confirmation failed for {Email}. Errors: {Errors}", user.Email, string.Join(", ", identityResult.Errors.Select(e => e.Description)));

                var errors = identityResult.Errors.Select(e => e.Description).ToList();
                return Result.Failure(errors);
            }

            _logger.LogInformation("Email confirmed successfully for {Email}", user.Email);

            try
            {
                _logger.LogDebug("Sending account created confirmation mail for {Email}", user.Email);
                var baseUrl = _configuration["AppSettings:BaseUrl"];
                var loginLink = $"{baseUrl}/Account/Login";

                await _emailService.SendAccountCreatedEmailAsync(user.Email!, user.FirstName, loginLink);
                _logger.LogDebug("Sent account created confirmation mail for {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Account created confirmation mail failed for {Email}", user.Email);
            }

            return Result.Success($"Email confirmed and account created successfully for {user.Email}");
        }

        public async Task<Result<ProfileDto>> GetUserProfileByEmailAsync(string email)
        {
           if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Profile fetch failed because email is null");
                return Result.Failure<ProfileDto>(new[] {"Email is required"});
            }

            _logger.LogInformation("Profile fetching started for {Email}", email);

            _logger.LogDebug("Finding user with {Email}", email);
            
           var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("No user found for {Email}", email);
                return Result.Failure<ProfileDto>(new[] { "Invalid email given" });
            }

            _logger.LogInformation("User profile fetched successfully for {Email}", email);

            var profileDto = new ProfileDto 
            { 
                Email = email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
            };

            _logger.LogInformation("Profile fetch completed successfully for {Email}", email);

            return Result.Success<ProfileDto>(profileDto);

        }

        public async Task<Result> LoginUserAsync(LoginUserDto? loginUserDto)
        {

            if (loginUserDto == null)
            {
                _logger.LogWarning("Login failed. Login Info is required");
                return Result.Failure(new[] { "Login info is required." });
            }
            _logger.LogInformation("Login attempt started for {Email}", loginUserDto.Email);

            _logger.LogDebug("Searching user with email {Email}", loginUserDto.Email);

            var user = await _userManager.FindByEmailAsync(loginUserDto.Email);

            if (user == null)
            {
                _logger.LogWarning("Login failed. User not found for email {Email}", loginUserDto.Email);
                return Result.Failure(new[] { "Invalid email or password" });
            }

            _logger.LogDebug("User {Email} found. Checking email confirmation.", loginUserDto.Email);

            if (! await _userManager.IsEmailConfirmedAsync(user)) {
                _logger.LogWarning("Login failed. Email is not confirmed for {Email}", loginUserDto.Email);
                return Result.Failure(new[] { "Please confirm your email" });
            }

            _logger.LogDebug("Attempting password sign in for {Email}", loginUserDto.Email);

            var signInResult =  await _signInManager.PasswordSignInAsync(user, loginUserDto.Password, loginUserDto.RememberMe, lockoutOnFailure: false);


            if (!signInResult.Succeeded)
            {
                _logger.LogWarning("Login faild.Invalid password for {Email}", loginUserDto.Email);
                return Result.Failure(new[] { "Invalid email or password" });
            }

            _logger.LogInformation("User {Email} logged in successfully.", loginUserDto.Email);

            _logger.LogDebug("Updating Lastgoin for {Email}", loginUserDto.Email);

            user.LastLogin = DateTime.UtcNow;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                _logger.LogError("Failed to update LastLogin for {Email}. Errors: {Errors}", loginUserDto.Email, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                return Result.Failure(new[] { "Unable to update user information." });
            }

            _logger.LogInformation("LastLogin updated successfully for {Email}", loginUserDto.Email);

            return Result.Success("User logged in successfully");

        }

        public async Task<Result> LogoutUserAsync()
        {   
            _logger.LogInformation("User logout started");
           await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out successfully");
           return Result.Success("User logged out successfully");
        }

        public async Task<Result> RegisterUserAsync(RegisterUserDto dto)
        {
            // Validate input
            if (dto == null)
            {
                _logger.LogWarning("User registration failed because registration data was null.");
                return Result.Failure(new[] { "Registration data is required" });
            }

            var isUserExists = await _userManager.FindByEmailAsync(dto.Email);

            if (isUserExists != null)
            {
                _logger.LogWarning("User registration failed because User already exists with {Email}", dto.Email);
                return Result.Failure(new[] { "User already exists" });
            }

            _logger.LogInformation("User registeration started for {Email}", dto.Email);

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                IsActive = true,
                PhoneNumber = dto.PhoneNumber
            };

            _logger.LogDebug("Creating identity user for {Email}", dto.Email);

            IdentityResult result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                _logger.LogWarning("user creation failed for {Email}. Errors: {Errors}", user.Email, result.Errors.Select(x => x.Description));

                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure(errors);
            }

            _logger.LogInformation("User created successfully for {UserId}", user.Id);

            _logger.LogDebug("Assigning role {Role} to {UserName}", dto.Role, user.UserName);

            IdentityResult roleAssignResult = await _userManager.AddToRoleAsync(user, dto.Role.ToString());

            if (!roleAssignResult.Succeeded)
            {
                _logger.LogWarning("User role assignment failed. Deleting user for {UserName}",user.UserName);
                // Delete user if role assignment fails
                var deleteResult = await _userManager.DeleteAsync(user);

                if (!deleteResult.Succeeded)
                {
                    _logger.LogError("User Deletion failed for {UserName}", user.UserName);

                }

                var errors = roleAssignResult.Errors.Select(e => e.Description).ToList();
                return Result.Failure(errors);
            }

            _logger.LogInformation("Role {Role} assigned to {UserName}", dto.Role, user.UserName);

            try
            {   
                _logger.LogDebug("Generating email confirmation token for {Email}", dto.Email);
                var token = await GenerateEmailConfirmationTokenAsync(user);
                var baseUrl = _configuration["AppSettings:BaseUrl"];
                var confirmationLink = $"{baseUrl}/account/confirm-email?userId={user.Id}&token={token}";

                _logger.LogDebug("Sending confirmation email for {Email}", dto.Email);
                await _emailService.SendRegistrationConfirmationEmailAsync(user.Email!, user.FirstName, confirmationLink);

                _logger.LogInformation("Confirmation email sent to {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
            }

            _logger.LogInformation("User registration completed successfully for {UserName}", user.UserName);

            return Result.Success("User created successfully. Please check your email to confirm your account.");
        }

        public async Task<Result> SendEmailConfirmationAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Result.Failure(new[] { "Email is required" });
            }

            var user  = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Result.Failure(new[] { "User not found" });
            }

            if (await _userManager.IsEmailConfirmedAsync(user)) 
            {
                return Result.Failure(new[] {"User not found"});
            }

            var token = await GenerateEmailConfirmationTokenAsync(user);
            var baseUrl = _configuration["AppSettings:BaseUrl"];

            var confirmationLink = $"{baseUrl}/account/confirm-email?userId={user.Id}&token={token}";

            await _emailService.SendRegistrationConfirmationEmailAsync(user.Email!, user.FirstName!, confirmationLink);

            return Result.Success("Mail send successfully");
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {

            if (resetPasswordDto == null)
            {
                _logger.LogWarning("Password reset failed. Password reset data is required");
                return Result.Failure(new[] { "Password Reset Data is required" });
            }

            _logger.LogInformation("Password reset started for {email}", resetPasswordDto.Email);

            _logger.LogDebug("finding user with {email}", resetPasswordDto.Email);

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (user == null)
            {
                _logger.LogWarning("Password reset failed. User not found with {email}", resetPasswordDto.Email);
                return Result.Failure(new[] { "Invalid email provided" });
            }

            _logger.LogDebug("User found successfully for {email}", user.Email);

            _logger.LogDebug("Checking if user's email is confirmed");

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogWarning("Password reset failed. Email is not confirmed for {email}", user.Email);
                return Result.Failure(new[] { "Invalid email provided" });
            }

            _logger.LogDebug("Email is confirmed for {email}. Decoding reset password token", user.Email);
            string decodedToken;
            try
            {
                var decodedBytes = WebEncoders.Base64UrlDecode(resetPasswordDto.Token);
                decodedToken = Encoding.UTF8.GetString(decodedBytes);
            }
            catch (FormatException ex)
            {
                _logger.LogWarning(ex, "Password reset failed. Invalid reset password token given for {email}", user.Email);
                return Result.Failure(new[] { "Invalid reset password token given" });
            }

            _logger.LogDebug("Reseting password for {email}", user.Email);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.Password);

            if (!result.Succeeded)
            {
                
                var errors = result.Errors.Select(e => e.Description).ToList();

                _logger.LogWarning("Password reset failed for {email}. Errors: {Errors}", user.Email, errors);

                return Result.Failure(errors);
            }

            _logger.LogInformation("Password reset successfully for {email}", user.Email);

            return Result.Success("Password reset successfully");
        }

        public async Task<Result> SendResetPasswordLinkAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user  = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

            if (user == null)
            {
                return Result.Failure(new[] { "Invalid email provided" });
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Result.Failure(new[] { "Invalid email provided" });
            }

            var token = await GeneratePasswordResetTokenAsync(user);
            
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var resetLink = $"{baseUrl}/account/reset-password?userId={user.Email}&token={token}";

            await _emailService.SendPasswordResetEmailAsync(user.Email!, user.FirstName!, resetLink);

            return Result.Success("Email sent successfully");
        }
        private async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return encodedToken;
        }

        private async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
           if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

           var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return encodedToken;
        }
    }
}
