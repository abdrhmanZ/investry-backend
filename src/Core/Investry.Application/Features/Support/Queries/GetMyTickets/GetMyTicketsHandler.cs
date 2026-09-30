using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using MediatR;

namespace Investry.Application.Features.Support.Queries.GetMyTickets
{
    public class GetMyTicketsHandler
        : IRequestHandler<GetMyTicketsQuery, Result<List<SupportTicketDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMyTicketsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<SupportTicketDto>>> Handle(
            GetMyTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _unitOfWork.SupportTicketRepository
                .GetByUserIdAsync(request.UserId);

            var dtos = tickets.Select(t => new SupportTicketDto
            {
                Id = t.Id,
                Category = t.Category,
                Subject = t.Subject,
                Message = t.Message,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                AdminReply = t.AdminReply,
                RepliedAt = t.RepliedAt
            }).ToList();

            return Result<List<SupportTicketDto>>.Success(dtos);
        }
    }
}
