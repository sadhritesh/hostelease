
using Microsoft.AspNetCore.Identity;

namespace HostelEase.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        //extended properties
        public string FirstName { get; set; } = default!;
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? LastLogin {  get; set; }
        public bool IsActive { get; set; }

        //audit cols 
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        //navigation property
        public IEnumerable<Address>? Addresses { get; set; }
    }
}
