using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Investry.Persistence.Repositories
{
    public class SupportTicketRepository : Repository<SupportTicket>, ISupportTicketRepository
    {
        private readonly InvestryDbContext _dbContext;

        public SupportTicketRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SupportTicket>> GetByUserIdAsync(string userId)
        {
            return await _dbContext.SupportTickets
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<SupportTicket>> GetAllWithFiltersAsync(string? status, string? search)
        {
            var query = _dbContext.SupportTickets
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(t => t.Status == status);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(t =>
                    t.UserName.ToLower().Contains(term) ||
                    t.Subject.ToLower().Contains(term));
            }

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
