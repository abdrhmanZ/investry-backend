using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using MediatR;

namespace Investry.Application.Features.Support.Commands.CreateTicket
{
    public class CreateTicketHandler
        : IRequestHandler<CreateTicketCommand, Result<SupportTicketDto>>
    {
        private static readonly HashSet<string> AllowedCategories =
            new(StringComparer.OrdinalIgnoreCase) { "Account", "Payment", "Project", "Technical", "Other" };

        private readonly IUnitOfWork _unitOfWork;

        public CreateTicketHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<SupportTicketDto>> Handle(
            CreateTicketCommand request, CancellationToken cancellationToken)
        {
            if (!AllowedCategories.Contains(request.Category))
                return Result<SupportTicketDto>.Failure(new List<Error>
                {
                    new("Ticket.InvalidCategory",
                        "Category must be one of: Account, Payment, Project, Technical, Other.",
                        ErrorType.Validation)
                });

            if (string.IsNullOrWhiteSpace(request.Subject) || request.Subject.Length > 200)
                return Result<SupportTicketDto>.Failure(new List<Error>
                {
                    new("Ticket.InvalidSubject",
                        "Subject is required and must not exceed 200 characters.",
                        ErrorType.Validation)
                });

            if (string.IsNullOrWhiteSpace(request.Message) || request.Message.Length > 2000)
                return Result<SupportTicketDto>.Failure(new List<Error>
                {
                    new("Ticket.InvalidMessage",
                        "Message is required and must not exceed 2000 characters.",
                        ErrorType.Validation)
                });

            var ticket = new SupportTicket
            {
                UserId = request.UserId,
                UserName = request.UserName,
                UserEmail = request.UserEmail,
                UserRole = request.UserRole,
                Category = request.Category,
                Subject = request.Subject.Trim(),
                Message = request.Message.Trim()
            };

            await _unitOfWork.SupportTicketRepository.AddAsync(ticket);
            await _unitOfWork.SaveAsync();

            return Result<SupportTicketDto>.Success(new SupportTicketDto
            {
                Id = ticket.Id,
                Category = ticket.Category,
                Subject = ticket.Subject,
                Message = ticket.Message,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt,
                AdminReply = null,
                RepliedAt = null
            });
        }
    }
}
