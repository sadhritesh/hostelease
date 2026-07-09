using HostelEase.Application.Interfaces.RepositoryContracts;
using HostelEase.Domain.Entities;
using HostelEase.Infrastructure.Data;


namespace HostelEase.Infrastructure.Repositories
{
    public class HostelsRepository : GenericRepository<Hostel>, IHostelRepository
    {
        public HostelsRepository(ApplicationDbContext context) : base (context)
        {
        }
        public Task<Hostel?> GetHostelWithManagersAsync(Guid hostelId)
        {
            throw new NotImplementedException();
        }
    }
}
