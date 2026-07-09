using System.Collections.Generic;
using HostelEase.Domain.Entities;

namespace HostelEase.Application.Interfaces.ServiceContracts
{
    public interface IHostelService
    {
        Task AddHostel(Hostel dto);
        Task<Hostel> GetHostelById(Guid id);
        Task<IEnumerable<Hostel>> GetAllHostels();
    }
}
