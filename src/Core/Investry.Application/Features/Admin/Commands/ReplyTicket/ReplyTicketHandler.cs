using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Support;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.ReplyTicket
{
    public class ReplyTicketHandler
        : IRequestHandler<ReplyTicketCommand, Result<SupportTicketDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReplyTicketHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<SupportTicketDto>> Handle(
            ReplyTicketCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Reply))
                return Result<SupportTicketDto>.Failure(new List<Error>
                {
                    new("Ticket.ReplyRequired", "Reply cannot be empty.", ErrorType.Validation)
                });

            var ticket = await _unitOfWork.SupportTicketRepository.GetAsync(request.TicketId);

            if (ticket is null)
                return Result<SupportTicketDto>.Failure(new List<Error>
                {
                    new("Ticket.NotFound", "Ticket not found.", ErrorType.NotFound)
                });

            if (ticket.Status == "Resolved")
                return Result<SupportTicketDto>.Failure(new List<Error>
                {
                    new("Ticket.AlreadyResolved", "This ticket has already been resolved.", ErrorType.Validation)
                });

            ticket.AdminReply = request.Reply.Trim();
            ticket.Status = "Resolved";
            ticket.RepliedAt = DateTime.UtcNow;

            await _unitOfWork.SupportTicketRepository.UpdateAsync(ticket);
            await _unitOfWork.SaveAsync();

            return Result<SupportTicketDto>.Success(new SupportTicketDto
            {
                Id = ticket.Id,
                Category = ticket.Category,
                Subject = ticket.Subject,
                Message = ticket.Message,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt,
                AdminReply = ticket.AdminReply,
                RepliedAt = ticket.RepliedAt
            });
        }
    }
}
