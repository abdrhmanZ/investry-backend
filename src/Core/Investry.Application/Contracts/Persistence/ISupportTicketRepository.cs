using Investry.Domain.Entities;

namespace Investry.Application.Contracts.Persistence
{
    public interface ISupportTicketRepository : IRepository<SupportTicket>
    {
        Task<List<SupportTicket>> GetByUserIdAsync(string userId);
        Task<List<SupportTicket>> GetAllWithFiltersAsync(string? status, string? search);
    }
}
