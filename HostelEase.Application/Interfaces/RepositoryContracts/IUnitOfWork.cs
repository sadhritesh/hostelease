
namespace HostelEase.Application.Interfaces.RepositoryContracts
{
    public interface IUnitOfWork : IDisposable
    {
        IHostelRepository Hostels {  get; }
        Task<int>CommitAsync (CancellationToken cancellationToken = default);
    }
}
