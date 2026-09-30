using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetAdminTickets
{
    public record GetAdminTicketsQuery(string? Status, string? Search)
        : IRequest<Result<List<Support.AdminTicketDto>>>;
}
