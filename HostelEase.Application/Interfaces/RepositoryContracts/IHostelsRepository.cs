using HostelEase.Domain.Entities;

namespace HostelEase.Application.Interfaces.RepositoryContracts
{
    public interface IHostelRepository : IGenericRepository<Hostel>
    {
        Task<Hostel?> GetHostelWithManagersAsync(Guid hostelId);
    }
}
