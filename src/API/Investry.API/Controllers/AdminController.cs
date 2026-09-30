using Investry.API.Common;
using Investry.API.Common.Extensions;
using Investry.API.DTOs.Requests;
using Investry.Application.Common.Models;
using Investry.Application.Constants;
using Investry.Application.Features.Admin.Commands.ApproveProject;
using Investry.Application.Features.Admin.Commands.BanUser;
using Investry.Application.Features.Admin.Commands.CreateAdmin;
using Investry.Application.Features.Admin.Commands.DeleteAdmin;
using Investry.Application.Features.Admin.Commands.RejectProject;
using Investry.Application.Features.Admin.Commands.UnbanUser;
using Investry.Application.Features.Admin.Queries.GetAdminAccounts;
using Investry.Application.Features.Admin.Queries.GetAdminTickets;
using Investry.Application.Features.Admin.Commands.ReplyTicket;
using Investry.Application.Features.Support;
using Investry.Application.Features.Admin.Queries.GetAdminProjects;
using Investry.Application.Features.Admin.Queries.GetAdminUserDetails;
using Investry.Application.Features.Admin.Queries.GetAdminUsers;
using Investry.Application.Features.Admin.Queries.GetEndedCampaigns;
using Investry.Application.Features.Wallet.Commands.ProcessCampaignEnd;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Investry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ─── Projects ───────────────────────────────────────────────────

        /// <summary>
        /// Get all projects for admin review with optional filtering.
        /// </summary>
        [HttpGet("projects")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AdminProjectDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProjects(
            [FromQuery] string? status,
            [FromQuery] string? search)
            => (await _mediator.Send(new GetAdminProjectsQuery(status, search))).ToApiResponse();

        /// <summary>
        /// Approve a project — changes status from PendingReview to Published.
        /// </summary>
        [HttpPut("projects/{projectId}/approve")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ApproveProject(Guid projectId)
            => (await _mediator.Send(new ApproveProjectCommand(projectId))).ToApiResponse();

        /// <summary>
        /// Reject a project — changes status from PendingReview to Rejected and notifies the founder via email.
        /// </summary>
        [HttpPut("projects/{projectId}/reject")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RejectProject(
            Guid projectId,
            [FromBody] RejectProjectRequest request)
            => (await _mediator.Send(new RejectProjectCommand(projectId, request.Reason))).ToApiResponse();

        // ─── Campaigns ──────────────────────────────────────────────────

        [HttpGet("campaigns/ended")]
        public async Task<IActionResult> GetEndedCampaigns([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => (await _mediator.Send(new GetEndedCampaignsQuery(page, pageSize))).ToApiResponse();

        [HttpPost("campaigns/{id}/process")]
        public async Task<IActionResult> ProcessCampaign(Guid id) 
            => (await _mediator.Send(new ProcessCampaignEndCommand(id))).ToApiResponse();

        // ─── Users ──────────────────────────────────────────────────────

        /// <summary>
        /// Get all users with pagination, filtering by role/status, and search.
        /// </summary>
        [HttpGet("users")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<AdminUserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? role = null,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null)
            => (await _mediator.Send(new GetAdminUsersQuery(page, pageSize, role, status, search))).ToApiResponse();

        /// <summary>
        /// Get a single user's details by ID.
        /// </summary>
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<AdminUserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserDetails(string userId)
            => (await _mediator.Send(new GetAdminUserDetailsQuery(userId))).ToApiResponse();

        /// <summary>
        /// Ban a user with a reason and duration.
        /// </summary>
        [HttpPost("users/{userId}/ban")]
        [ProducesResponseType(typeof(ApiResponse<BanUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BanUser(
            string userId,
            [FromBody] BanUserRequest request)
            => (await _mediator.Send(new BanUserCommand(userId, request.DurationInDays, request.Reason))).ToApiResponse();

        /// <summary>
        /// Unban / reactivate a user.
        /// </summary>
        [HttpPost("users/{userId}/unban")]
        [ProducesResponseType(typeof(ApiResponse<UnbanUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UnbanUser(string userId)
            => (await _mediator.Send(new UnbanUserCommand(userId))).ToApiResponse();

        // ─── Admin Accounts ─────────────────────────────────────────────

        /// <summary>
        /// Get all admin accounts.
        /// </summary>
        [HttpGet("accounts")]
        [ProducesResponseType(typeof(ApiResponse<List<AdminAccountDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAdminAccounts()
            => (await _mediator.Send(new GetAdminAccountsQuery())).ToApiResponse();

        /// <summary>
        /// Get a single admin account by ID.
        /// </summary>
        [HttpGet("accounts/{id}")]
        [ProducesResponseType(typeof(ApiResponse<AdminAccountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAdminById(string id)
            => (await _mediator.Send(new GetAdminAccountByIdQuery(id))).ToApiResponse();

        /// <summary>
        /// Create a new admin account.
        /// </summary>
        [HttpPost("accounts")]
        [ProducesResponseType(typeof(ApiResponse<AdminAccountDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request)
            => (await _mediator.Send(new CreateAdminCommand(
                request.FirstName, request.LastName, request.Email, request.Password))).ToApiResponse();

        /// <summary>
        /// Delete an admin account.
        /// </summary>
        [HttpDelete("accounts/{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAdmin(string id)
            => (await _mediator.Send(new DeleteAdminCommand(
                id, User.FindFirstValue(CustomClaimTypes.Uid)!))).ToApiResponse();

        // ─── Support Tickets ────────────────────────────────────────────

        /// <summary>
        /// Get all support tickets with optional filtering.
        /// </summary>
        [HttpGet("tickets")]
        [ProducesResponseType(typeof(ApiResponse<List<AdminTicketDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTickets(
            [FromQuery] string? status = null,
            [FromQuery] string? search = null)
            => (await _mediator.Send(new GetAdminTicketsQuery(status, search))).ToApiResponse();

        /// <summary>
        /// Reply to a support ticket and mark it as Resolved.
        /// </summary>
        [HttpPut("tickets/{id}/reply")]
        [ProducesResponseType(typeof(ApiResponse<SupportTicketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReplyTicket(
            Guid id,
            [FromBody] ReplyTicketRequest request)
            => (await _mediator.Send(new ReplyTicketCommand(id, request.Reply))).ToApiResponse();
    }
}
