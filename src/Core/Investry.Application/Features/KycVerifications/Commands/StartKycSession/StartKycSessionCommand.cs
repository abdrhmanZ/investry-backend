using Investry.Application.Common;
using Investry.Application.DTOs;
using MediatR;

namespace Investry.Application.Features.KycVerification.Commands.StartKycSession
{
    public record StartKycSessionCommand(string UserId) : IRequest<Result<StartKycSessionResponse>>;
}
