using Investry.API.Common.Extensions;
using Investry.API.DTOs.Requests;
using Investry.Application.Constants;
using Investry.Application.Features.Users.Commands.ChangePassword;
using Investry.Application.Features.Users.Commands.UpdateProfile;
using Investry.Application.Features.Users.Commands.UploadProfilePicture;
using Investry.Application.Features.Users.Queries.GetProfile;
using Investry.Application.Models.Media;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Investry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("upload-profile-picture")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] UploadProfilePictureRequest request)
        {
            var fileDto = new FileDto
            {
                Content = request.File.OpenReadStream(),
                FileName = request.File.FileName,
                ContentType = request.File.ContentType
            };
            return (await _mediator.Send(new UploadProfilePictureCommand(User.FindFirstValue(CustomClaimTypes.Uid), fileDto))).ToApiResponse();
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
            => (await _mediator.Send(new GetProfileQuery(User.FindFirstValue(CustomClaimTypes.Uid)))).ToApiResponse();

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
            => (await _mediator.Send(new ChangePasswordCommand(User.FindFirstValue(CustomClaimTypes.Uid), request.CurrentPassword, request.NewPassword))).ToApiResponse();

        [HttpPatch]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
            => (await _mediator.Send(new UpdateProfileCommand(User.FindFirstValue(CustomClaimTypes.Uid), request.FirstName, request.LastName, request.PhoneNumber))).ToApiResponse();
    }
}
