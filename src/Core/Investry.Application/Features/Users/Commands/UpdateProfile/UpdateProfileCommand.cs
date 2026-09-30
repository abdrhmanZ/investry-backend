using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Users.Commands.UpdateProfile
{
    public record UpdateProfileCommand(string UserId, string? FirstName, string? LastName, string? PhoneNumber) : IRequest<Result<bool>>;
}
