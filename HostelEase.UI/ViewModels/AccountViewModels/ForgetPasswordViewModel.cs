using System.ComponentModel.DataAnnotations;

namespace HostelEase.UI.ViewModels.AccountViewModels
{
    public class ForgetPasswordViewModel
    {
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email provided")]
        [Required(ErrorMessage = "Email address is required")]
        public string Email { get; set; } = default!;
    }
}
