
using Microsoft.AspNetCore.Identity;

namespace HostelEase.Infrastructure.Identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn {  get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
