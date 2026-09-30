namespace Investry.Application.Contracts.Persistence
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetAsync(Guid id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<bool> ExistsAsync(Guid id);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
