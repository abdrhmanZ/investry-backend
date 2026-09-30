using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Users.Queries.GetProfile
{
    public record GetProfileQuery(string UserId) : IRequest<Result<ProfileDto>>;
}
