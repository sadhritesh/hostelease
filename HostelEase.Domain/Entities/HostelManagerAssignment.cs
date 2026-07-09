using System.ComponentModel.DataAnnotations.Schema;

namespace HostelEase.Domain.Entities
{
    public class HostelManagerAssignment
    {
        //AssignmentId: int
        //HostelId: int
        //ManagerUserId: string
        //AssignedDate: datetime
        //IsActive: bool

        public Guid AssignmentId { get; set; }
        public Guid HostelId { get; set; }
        public Guid ManagerUserId { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool IsActive { get; set; }

        //Navigation 
        public Hostel Hostel { get; set; } = default!;
        //public ApplicationUser Manager { get; set; } = default!;
    }
}