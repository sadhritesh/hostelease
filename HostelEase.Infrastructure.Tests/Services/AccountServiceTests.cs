
using HostelEase.Application.Common.Interfaces;
using HostelEase.Application.Features.AccountDTO;
using HostelEase.Infrastructure.Identity;
using HostelEase.Infrastructure.Services;
using HostelEase.UI.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Moq;

namespace HostelEase.Infrastructure.Tests.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<ILogger<AccountService>> _loggerMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _loggerMock = new Mock<ILogger<AccountService>>();

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null
                );

            var contextMock = new Mock<HttpContext>();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var signInLoggerMock = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object, new Mock<HttpContextAccessor>().Object,
                claimsFactoryMock.Object, null, signInLoggerMock.Object, null, null
                );

            _emailServiceMock = new Mock<IEmailService>();
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["AppSettings:BaseUrl"]).Returns("https://example.com");

            _accountService = new AccountService(
                _loggerMock.Object,
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _emailServiceMock.Object,
                _configurationMock.Object
                );
        }

        #region RegisterUserAsync Tests
        [Fact]
        public async Task RegisterUserAsync_WithNullDto_ReturnFailure()
        {
            //Arrange
            //Act
            var result = await _accountService.RegisterUserAsync(null);
            //Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Registration data is required", result.Errors);
        }

        [Fact]
        public async Task RegisterUserAsync_WhenUserAlreadyExists_ReturnFailure()
        {
            //Arrange
            var dto = new RegisterUserDto
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123!",
                Role = RoleType.Student.ToString(),
                PhoneNumber = "1234567890"
            };

            var user = new ApplicationUser
            {
                Email = "test@example.com",
                FirstName = "John"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            //Act
            var result = await _accountService.RegisterUserAsync(dto);

            //Assert
            Assert.False(result.isSuccess);
            Assert.Contains("User already exists", result.Errors);
            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>()), Times.Never);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            _emailServiceMock.Verify(es => es.SendRegistrationConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RegisterUserAsync_WhenCreateAsyncFails_ReturnsFailure()
        {
            //Arrange
            var email = "test@gmail.com";
            var password = "Password123!";

            var user = new ApplicationUser { Id = Guid.NewGuid(), Email = email, FirstName = "Test" };
            var dto = new RegisterUserDto
            {
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123!",
                Role = RoleType.Student.ToString(),
                PhoneNumber = "1234567890"
            };

            var identityErrors = new[] { new IdentityError { Description = "Password too weak"} };
            var identityResult = IdentityResult.Failed(identityErrors);

            _userManagerMock.Setup(um => um.FindByEmailAsync(user.Email)).ReturnsAsync((ApplicationUser)null!);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), password))
                .ReturnsAsync(identityResult);

            //Act
            var result = await _accountService.RegisterUserAsync(dto);

            //Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Password too weak", result.Errors);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleType.Student.ToString()), Times.Never);
            _emailServiceMock.Verify(es => es.SendRegistrationConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RegisterUserAsync_WhenRoleAssignmenFails_DeleteUserAndReturnFailure()
        {
            //Arrange
            var email = "test@gmail.com";

            var user = new ApplicationUser { Id = Guid.NewGuid(), Email = email, FirstName = "Test" };
            var dto = new RegisterUserDto
            {
                Email = email,
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123!",
                Role = RoleType.Student.ToString(),
                PhoneNumber = "1234567890"
            };

            var identityErrors = new[] { new IdentityError { Description = "Error! Role was not assigned" } };
            var identityResult = IdentityResult.Failed(identityErrors);

            _userManagerMock.Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser)null!);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(identityResult);
            _userManagerMock.Setup(um => um.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            //Act
            var result = await _accountService.RegisterUserAsync(dto);

            //Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Error! Role was not assigned", result.Errors);
            _emailServiceMock.Verify(es => es.SendRegistrationConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RegistrationUserAsync_WithValidData_ReturnSuccess()
        {
            //Arrange
            var dto = new RegisterUserDto
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123!",
                Role = RoleType.Student.ToString(),
                PhoneNumber = "1234567890",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), dto.Role))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync("testToken");
            _emailServiceMock.Setup(es => es.SendRegistrationConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            //Act
            var result = await _accountService.RegisterUserAsync(dto);

            //Assert
            Assert.True(result.isSuccess);
            Assert.Contains("User created successfully", result.Message);
        }

        [Fact]
        public async Task RegistrationUserAsync_WhenEmailServiceFails_ContinuesAndReturnSuccess()
        {
            var dto = new RegisterUserDto
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123!",
                Role = RoleType.Student.ToString(),
                PhoneNumber = "1234567890",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), dto.Role))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync("testToken");
            _emailServiceMock.Setup(es => es.SendRegistrationConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Email service unavailable"));

            //Act
            var result = await _accountService.RegisterUserAsync(dto);

            //Assert
            Assert.True(result.isSuccess);
            Assert.Contains("User created successfully", result.Message);
        }

        #endregion

        #region LoginUserAsync Tests

        [Fact]
        public async Task LoginUserAsync_WithNullLoginUserDto_ReturnFailure()
        {
            // Arrange
            LoginUserDto? loginUserDto = null;

            // Act
            var result = await _accountService.LoginUserAsync(loginUserDto!);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Login info is required.", result.Errors);
        }

        [Fact]
        public async Task LoginUserAsync_WithNonexistentUser_ReturnFailure()
        {
            // Arrange
            var loginUserDto = new LoginUserDto
            {
                Email = "nonexistent@example.com",
                Password = "Password123!",
                RememberMe = false
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _accountService.LoginUserAsync(loginUserDto);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Invalid email or password", result.Errors);
            _userManagerMock.Verify(x => x.FindByEmailAsync(loginUserDto.Email), Times.Once);
        }

        [Fact]
        public async Task LoginUserAsync_WithUnconfirmedEmail_ReturnFailure()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = false
            };

            var loginUserDto = new LoginUserDto
            {
                Email = "user@example.com",
                Password = "Password123!",
                RememberMe = false
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(loginUserDto.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.IsEmailConfirmedAsync(user))
                .ReturnsAsync(false);

            // Act
            var result = await _accountService.LoginUserAsync(loginUserDto);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Please confirm your email", result.Errors);
            _userManagerMock.Verify(x => x.IsEmailConfirmedAsync(user), Times.Once);
        }

        [Fact]
        public async Task LoginUserAsync_WithInvalidPassword_ReturnFailure()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true
            };

            var loginUserDto = new LoginUserDto
            {
                Email = "user@example.com",
                Password = "WrongPassword123!",
                RememberMe = false
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(loginUserDto.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.IsEmailConfirmedAsync(user))
                .ReturnsAsync(true);

            _signInManagerMock
                .Setup(x => x.PasswordSignInAsync(user, loginUserDto.Password, loginUserDto.RememberMe, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _accountService.LoginUserAsync(loginUserDto);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Invalid email or password", result.Errors);
            _signInManagerMock.Verify(
                x => x.PasswordSignInAsync(user, loginUserDto.Password, loginUserDto.RememberMe, false),
                Times.Once);
        }

        [Fact]
        public async Task LoginUserAsync_WithValidCredentials_ReturnSuccess()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true
            };

            var loginUserDto = new LoginUserDto
            {
                Email = "user@example.com",
                Password = "Password123!",
                RememberMe = false
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(loginUserDto.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.IsEmailConfirmedAsync(user))
                .ReturnsAsync(true);

            _signInManagerMock
                .Setup(x => x.PasswordSignInAsync(user, loginUserDto.Password, loginUserDto.RememberMe, false))
                .ReturnsAsync(SignInResult.Success);

            _userManagerMock
                .Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _accountService.LoginUserAsync(loginUserDto);

            // Assert
            Assert.True(result.isSuccess);
            Assert.Contains("User logged in successfully", result.Errors == null ? result.Message : "");
            Assert.NotNull(user.LastLogin);
            _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task LoginUserAsync_WithValidCredentialsAndRememberMe_ReturnSuccess()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true
            };

            var loginUserDto = new LoginUserDto
            {
                Email = "user@example.com",
                Password = "Password123!",
                RememberMe = true
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(loginUserDto.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.IsEmailConfirmedAsync(user))
                .ReturnsAsync(true);

            _signInManagerMock
                .Setup(x => x.PasswordSignInAsync(user, loginUserDto.Password, true, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _userManagerMock
                .Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _accountService.LoginUserAsync(loginUserDto);

            // Assert
            Assert.True(result.isSuccess);
            _signInManagerMock.Verify(
                x => x.PasswordSignInAsync(user, loginUserDto.Password, true, false),
                Times.Once);
        }

        [Fact]
        public async Task LoginUserAsync_WhenUpdateLastLoginFails_ReturnFailure()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true
            };

            var loginUserDto = new LoginUserDto
            {
                Email = "user@example.com",
                Password = "Password123!",
                RememberMe = false
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(loginUserDto.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.IsEmailConfirmedAsync(user))
                .ReturnsAsync(true);

            _signInManagerMock
                .Setup(x => x.PasswordSignInAsync(user, loginUserDto.Password, loginUserDto.RememberMe, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var identityError = new IdentityError { Description = "Database error" };
            _userManagerMock
                .Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            // Act
            var result = await _accountService.LoginUserAsync(loginUserDto);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Unable to update user information.", result.Errors);
        }

        [Fact]
        public async Task LoginUserAsync_UpdatesLastLoginToUtcNow()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true,
                LastLogin = null
            };

            var loginUserDto = new LoginUserDto
            {
                Email = "user@example.com",
                Password = "Password123!",
                RememberMe = false
            };

            var beforeLogin = DateTime.UtcNow;

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(loginUserDto.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.IsEmailConfirmedAsync(user))
                .ReturnsAsync(true);

            _signInManagerMock
                .Setup(x => x.PasswordSignInAsync(user, loginUserDto.Password, loginUserDto.RememberMe, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _userManagerMock
                .Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _accountService.LoginUserAsync(loginUserDto);

            // Assert
            Assert.True(result.isSuccess);
            Assert.NotNull(user.LastLogin);
            Assert.True(user.LastLogin >= beforeLogin && user.LastLogin <= DateTime.UtcNow);
        }

        #endregion

        #region ConfirmEmail Tests

        [Fact]
        public async Task ConfirmEmailAsync_UserIdIsNullorEmpty_ReturnFailure()
        {
            // Arrange
            var userId = Guid.Empty;
            var token = "validToken";

            // Act
            var result = await _accountService.ConfirmEmailAsync(userId, token);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Invalid userId or token.", result.Errors);
        }

        [Fact]
        public async Task ConfirmEmailAsync_TokenIsNullorEmpty_ReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = string.Empty;

            // Act
            var result = await _accountService.ConfirmEmailAsync(userId, token);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Invalid userId or token.", result.Errors);
        }

        [Fact]
        public async Task ConfirmEmailAsync_UserNotFound_ReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "validToken";

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _accountService.ConfirmEmailAsync(userId, token);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("User not found", result.Errors);
            _userManagerMock.Verify(x => x.FindByIdAsync(userId.ToString()), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailAsync_EmailAlreadyConfirmed_ReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "validToken";

            var user = new ApplicationUser
            {
                Id = userId,
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            // Act
            var result = await _accountService.ConfirmEmailAsync(userId, token);

            // Assert
            Assert.True(result.isSuccess);
            Assert.Contains("Email is already confirmed.", result.Message);
            _userManagerMock.Verify(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ConfirmEmailAsync_InvalidEncodedToken_ReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var invalidToken = "!!!invalid_base64_token!!!";

            var user = new ApplicationUser
            {
                Id = userId,
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = false
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            // Act
            var result = await _accountService.ConfirmEmailAsync(userId, invalidToken);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Invalid confirmation token.", result.Errors);
            _userManagerMock.Verify(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ConfirmEmailAsync_WhenIdentityConfirmationFails_ReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var validToken = "CkIyQjRkRTZmNTkzYTRmNmI3ZDljMzFhMTU1YTMyOGQ2"; // Valid base64 encoded token
            var user = new ApplicationUser
            {
                Id = userId,
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = false
            };

            var identityError = new IdentityError { Description = "Invalid token" };
            var identityResult = IdentityResult.Failed(identityError);

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _accountService.ConfirmEmailAsync(userId, validToken);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Invalid token", result.Errors);
            _emailServiceMock.Verify(es => es.SendAccountCreatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ConfirmEmailAsync_WhenWelcomeEmailFails_ReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var validToken = "CkIyQjRkRTZmNTkzYTRmNmI3ZDljMzFhMTU1YTMyOGQ2"; // Valid base64 encoded token
            var user = new ApplicationUser
            {
                Id = userId,
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = false
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _emailServiceMock
                .Setup(es => es.SendAccountCreatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Email service unavailable"));

            // Act
            var result = await _accountService.ConfirmEmailAsync(userId, validToken);

            // Assert
            Assert.True(result.isSuccess);
            Assert.Contains($"Email confirmed and account created successfully for {user.Email}", result.Message);
        }

        [Fact]
        public async Task ConfirmEmailAsync_WithValidInput_ReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var validToken = "CkIyQjRkRTZmNTkzYTRmNmI3ZDljMzFhMTU1YTMyOGQ2"; // Valid base64 encoded token
            var user = new ApplicationUser
            {
                Id = userId,
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = false
            };

            _userManagerMock
                .Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _emailServiceMock
                .Setup(es => es.SendAccountCreatedEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _accountService.ConfirmEmailAsync(userId, validToken);

            // Assert
            Assert.True(result.isSuccess);
            Assert.Contains($"Email confirmed and account created successfully for {user.Email}", result.Message);
            _userManagerMock.Verify(x => x.ConfirmEmailAsync(user, It.IsAny<string>()), Times.Once);
            _emailServiceMock.Verify(es => es.SendAccountCreatedEmailAsync(user.Email!, user.FirstName, It.IsAny<string>()), Times.Once);
        }


        #endregion

        #region ResetPasswordAsync

        [Fact]
        public async Task ResetPasswordAsync_ResetPasswordDtoIsNull_ReturnFailure()
        {
            // Arrange
            ResetPasswordDto? resetPasswordDto = null;

            // Act
            var result = await _accountService.ResetPasswordAsync(resetPasswordDto!);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Password Reset Data is required", result.Errors);
        }

        [Fact]
        public async Task ResetPasswordAsync_UserNotFound_ReturnFailure()
        {
            // Arrange
            var resetPasswordDto = new ResetPasswordDto
            {
                Email = "nonexistent@example.com",
                Password = "NewPassword123!",
                Token = "validToken"
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _accountService.ResetPasswordAsync(resetPasswordDto);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Invalid email provided", result.Errors);
            _userManagerMock.Verify(x => x.FindByEmailAsync(resetPasswordDto.Email), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAsync_EmailNotConfirmed_ReturnFailure()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = false
            };

            var resetPasswordDto = new ResetPasswordDto
            {
                Email = "user@example.com",
                Password = "NewPassword123!",
                Token = "validToken"
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(resetPasswordDto.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.IsEmailConfirmedAsync(user))
                .ReturnsAsync(false);

            // Act
            var result = await _accountService.ResetPasswordAsync(resetPasswordDto);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Invalid email provided", result.Errors);
            _userManagerMock.Verify(x => x.IsEmailConfirmedAsync(user), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAsync_InvalidEncodedToken_ReturnFailure()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true
            };

            var resetPasswordDto = new ResetPasswordDto
            {
                Email = "user@example.com",
                Password = "NewPassword123!",
                Token = "!!!invalid_base64_token!!!"
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(resetPasswordDto.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.IsEmailConfirmedAsync(user))
                .ReturnsAsync(true);

            // Act
            var result = await _accountService.ResetPasswordAsync(resetPasswordDto);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Invalid reset password token given", result.Errors);
            _userManagerMock.Verify(x => x.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ResetPasswordAsync_IdentityResetPasswordFails_ReturnAllIdentityErrors()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true
            };

            var validToken = "CkIyQjRkRTZmNTkzYTRmNmI3ZDljMzFhMTU1YTMyOGQ2"; // Valid base64 encoded token

            var resetPasswordDto = new ResetPasswordDto
            {
                Email = "user@example.com",
                Password = "NewPassword123!",
                Token = validToken
            };

            var identityErrors = new[]
            {
                new IdentityError { Description = "Password too weak" },
                new IdentityError { Description = "Token expired" }
            };
            var identityResult = IdentityResult.Failed(identityErrors);

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(resetPasswordDto.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.IsEmailConfirmedAsync(user))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), resetPasswordDto.Password))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _accountService.ResetPasswordAsync(resetPasswordDto);

            // Assert
            Assert.False(result.isSuccess);
            Assert.Contains("Password too weak", result.Errors);
            Assert.Contains("Token expired", result.Errors);
            Assert.Equal(2, result.Errors.Count());
        }

        [Fact]
        public async Task ResetPasswordAsync_ValidInput_ReturnSuccess()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                UserName = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true
            };

            var validToken = "CkIyQjRkRTZmNTkzYTRmNmI3ZDljMzFhMTU1YTMyOGQ2"; // Valid base64 encoded token

            var resetPasswordDto = new ResetPasswordDto
            {
                Email = "user@example.com",
                Password = "NewPassword123!",
                Token = validToken
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(resetPasswordDto.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.IsEmailConfirmedAsync(user))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), resetPasswordDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _accountService.ResetPasswordAsync(resetPasswordDto);

            // Assert
            Assert.True(result.isSuccess);
            Assert.Contains("Password reset successfully", result.Message);
            _userManagerMock.Verify(x => x.ResetPasswordAsync(user, It.IsAny<string>(), resetPasswordDto.Password), Times.Once);
        }

        #endregion
    }
}
