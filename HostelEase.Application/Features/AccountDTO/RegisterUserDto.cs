using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelEase.Application.Features.AccountDTO
{
    public class RegisterUserDto
    {
            public string FirstName { get; set; } = null!;
            public string? LastName { get; set; }
            public string Email { get; set; } = null!;
            public DateTime? DateOfBirth { get; set; }
            public string Role { get; set; } = default!;
            public string PhoneNumber { get; set; } = default!;
            public string Password { get; set; } = null!;
    }
}
