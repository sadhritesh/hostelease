using HostelEase.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HostelEase.UI.Controllers
{
    public class RemoteValidationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public RemoteValidationController(UserManager<ApplicationUser> userManager) 
        { 
            _userManager = userManager;
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsEmailAvailable(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                return Json(true);
            }

            return Json(false);
        }
    }
}
