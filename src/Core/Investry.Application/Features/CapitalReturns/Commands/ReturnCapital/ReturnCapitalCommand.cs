using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.CapitalReturns.Commands.ReturnCapital
{
    public record ReturnCapitalCommand(ReturnCapitalRequest ReturnCapitalRequest, string UserId) : IRequest<Result<bool>>;
}
