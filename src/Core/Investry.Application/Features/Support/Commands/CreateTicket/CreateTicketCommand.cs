using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Support.Commands.CreateTicket
{
    public record CreateTicketCommand(
        string UserId,
        string UserName,
        string UserEmail,
        string UserRole,
        string Category,
        string Subject,
        string Message
    ) : IRequest<Result<SupportTicketDto>>;
}
