using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelEase.Domain.Entities
{
    public class Hostel
    {
        public Guid HostelId { get; set; } 
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        //Navigation
        public ICollection<HostelManagerAssignment> ManagerAssignments { get; set; } = new List<HostelManagerAssignment>();

    }
}
