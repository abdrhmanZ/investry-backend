using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Support.Queries.GetMyTickets
{
    public record GetMyTicketsQuery(string UserId)
        : IRequest<Result<List<SupportTicketDto>>>;
}
