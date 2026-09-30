using Investry.Application.Common;
using Investry.Application.Models.Media;
using MediatR;

namespace Investry.Application.Features.Users.Commands.UploadProfilePicture
{
    public record UploadProfilePictureCommand(string UserId, FileDto ProfilePicture) : IRequest<Result<string>>;
}
