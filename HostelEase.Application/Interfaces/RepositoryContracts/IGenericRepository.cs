
using HostelEase.Domain.Entities;

namespace HostelEase.Application.Interfaces.RepositoryContracts
{
    public interface IGenericRepository<T>
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
    }
}
