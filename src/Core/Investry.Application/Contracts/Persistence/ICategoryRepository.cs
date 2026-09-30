using Investry.Domain.Entities;

namespace Investry.Application.Contracts.Persistence
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<Category>> GetByIdsAsync(List<Guid> ids);
        Task<Category?> GetByNameAsync(string name);
    }
}
