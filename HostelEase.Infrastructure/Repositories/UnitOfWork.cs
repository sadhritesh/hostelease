
using HostelEase.Application.Interfaces.RepositoryContracts;
using HostelEase.Infrastructure.Data;

namespace HostelEase.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private HostelsRepository? _hostelsRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IHostelRepository Hostels
        {
            get
            {
                if (_hostelsRepository == null)
                {
                    _hostelsRepository = new HostelsRepository(_context);
                }

                return _hostelsRepository;
            }
        }
        public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
