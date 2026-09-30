using Investry.API.Common;
using Investry.API.Common.Extensions;
using Investry.API.DTOs.Requests;
using Investry.Application.Constants;
using Investry.Application.Contracts.Identity;
using Investry.Application.Features.Support;
using Investry.Application.Features.Support.Commands.CreateTicket;
using Investry.Application.Features.Support.Queries.GetMyTickets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Investry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Founder,Investor")]
    public class SupportController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public SupportController(IMediator mediator, IIdentityService identityService)
        {
            _mediator = mediator;
            _identityService = identityService;
        }

        /// <summary>
        /// Create a new support ticket.
        /// </summary>
        [HttpPost("tickets")]
        [ProducesResponseType(typeof(ApiResponse<SupportTicketDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
        {
            var userId = User.FindFirstValue(CustomClaimTypes.Uid)!;
            var userInfo = await _identityService.GetUserByIdAsync(userId);

            var command = new CreateTicketCommand(
                UserId: userId,
                UserName: $"{userInfo?.FirstName} {userInfo?.LastName}".Trim(),
                UserEmail: userInfo?.Email ?? string.Empty,
                UserRole: User.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
                Category: request.Category,
                Subject: request.Subject,
                Message: request.Message);

            return (await _mediator.Send(command)).ToApiResponse();
        }

        /// <summary>
        /// Get current user's support tickets.
        /// </summary>
        [HttpGet("my-tickets")]
        [ProducesResponseType(typeof(ApiResponse<List<SupportTicketDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyTickets()
            => (await _mediator.Send(new GetMyTicketsQuery(
                User.FindFirstValue(CustomClaimTypes.Uid)!))).ToApiResponse();
    }
}
