using System.ComponentModel.DataAnnotations;

namespace HostelEase.Infrastructure.Identity
{
    public class Address
    {
        public Guid Id {  get; set; }
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string Country { get; set; } = default!;

        //foreign key to application user
        public Guid UserId;
        public ApplicationUser User { get; set; } = default!;

        //audit
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

    }
}
