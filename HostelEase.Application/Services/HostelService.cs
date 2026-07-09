using HostelEase.Application.Interfaces.RepositoryContracts;
using HostelEase.Application.Interfaces.ServiceContracts;
using HostelEase.Domain.Entities;


namespace HostelEase.Application.Services
{
    public class HostelService : IHostelService
    {   
        private readonly IUnitOfWork _context;
        public HostelService(IUnitOfWork context) 
        {
            _context = context;
        }
        public async Task AddHostel(Hostel hostel)
        {
            await _context.Hostels.AddAsync(hostel);
            await _context.CommitAsync();
        }

        public async Task<Hostel> GetHostelById(Guid id)
        {
            Hostel? hostel = await _context.Hostels.GetByIdAsync(id);

            if (hostel == null)
            {
                throw new Exception("Hostel Not found");
            }

            return hostel;
        }

        public async Task<IEnumerable<Hostel>> GetAllHostels() 
        {
            return await _context.Hostels.GetAllAsync();
        }
    }
}
