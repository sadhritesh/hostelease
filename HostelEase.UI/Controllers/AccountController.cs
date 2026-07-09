using HostelEase.Application.Features.AccountDTO;
using HostelEase.Application.Interfaces.ServiceContracts;
using HostelEase.UI.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HostelEase.UI.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new RegisterUserDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Password = model.Password,
                Role = model.Role.ToString(),
            };

            var result = await _accountService.RegisterUserAsync(user);

            if (result.isSuccess)
                return RedirectToAction("RegistrationConfirmation");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }

        [HttpGet("registration-confirmation")]
        public IActionResult RegistrationConfirmation()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loginUserDto = new LoginUserDto()
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe,
            };

            var result = await _accountService.LoginUserAsync(loginUserDto);

            if (!result.isSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View(model);
            }

            ViewBag.Message = result.Message;

            return RedirectToAction("Index", "Hostel");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
        {
            if (userId == Guid.Empty || string.IsNullOrEmpty(token))
            {
                throw new ValidationException("Invalid email and token given");
            }

            var result = await _accountService.ConfirmEmailAsync(userId, token);

            if (!result.isSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View("Error");
            }

            return View("EmailConfirmed");
        }

        [HttpGet("forgot-password")]
        public IActionResult SendResetPasswordLink()
        {
            return View();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> SendResetPasswordLink(ForgetPasswordViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var forgotPasswordDto = new ForgotPasswordDto()
            {
                Email = model.Email,
            };

            var result = await _accountService.SendResetPasswordLinkAsync(forgotPasswordDto);

            if (!result.isSuccess)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(model);
            }

            return View("SendResetPasswordLinkConfirmation");
           
        }

        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string userid, string token)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid reset password rquest");
            }
            return View(new ResetPasswordViewModel() { Email = userid, Token = token});
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var resetPasswordDto = new ResetPasswordDto()
            {
                Email = model.Email,
                Password = model.Password,
                Token = model.Token
            };

            var result = await _accountService.ResetPasswordAsync(resetPasswordDto);

            if (!result.isSuccess)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(model);
            }

            return View("Login");
        }

    }
}
