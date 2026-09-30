using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Support;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetAdminTickets
{
    public class GetAdminTicketsHandler
        : IRequestHandler<GetAdminTicketsQuery, Result<List<AdminTicketDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAdminTicketsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<AdminTicketDto>>> Handle(
            GetAdminTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _unitOfWork.SupportTicketRepository
                .GetAllWithFiltersAsync(request.Status, request.Search);

            var dtos = tickets.Select(t => new AdminTicketDto
            {
                Id = t.Id,
                UserName = t.UserName,
                UserEmail = t.UserEmail,
                UserRole = t.UserRole,
                Category = t.Category,
                Subject = t.Subject,
                Message = t.Message,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                AdminReply = t.AdminReply,
                RepliedAt = t.RepliedAt
            }).ToList();

            return Result<List<AdminTicketDto>>.Success(dtos);
        }
    }
}
