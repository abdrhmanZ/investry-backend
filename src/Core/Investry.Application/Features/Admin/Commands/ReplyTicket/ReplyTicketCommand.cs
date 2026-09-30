using Investry.Application.Common;
using Investry.Application.Features.Support;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.ReplyTicket
{
    public record ReplyTicketCommand(Guid TicketId, string Reply)
        : IRequest<Result<SupportTicketDto>>;
}
