using Investry.API.Common;
using Investry.API.Common.Extensions;
using Investry.Application.Constants;
using Investry.Application.DTOs;
using Investry.Application.Features.KycVerification.Commands.StartKycSession;
using Investry.Application.Features.KycVerifications.Commands.HandleKycWebhook;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace Investry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KycController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        public KycController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        /// <summary>
        /// Start a new KYC session for the authenticated user.
        /// </summary>
        /// <remarks>
        /// This endpoint initiates a KYC session using Didit for the currently authenticated user.
        /// The server returns a verification URL where the user can complete the KYC process.
        ///
        /// Sample request:
        ///
        ///     POST /api/kyc/create-session
        ///
        /// No request body is required. The authenticated user's ID is extracted from the JWT.
        /// </remarks>
        /// <response code="200">Session created successfully, returns session ID and verification URL</response>
        /// <response code="400">An active KYC session already exists for this user</response>
        /// <response code="401">Unauthorized, user must be authenticated</response>
        /// <response code="500">Internal server error or failed to create session with Didit</response>
        [Authorize]
        [HttpPost("create-session")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<StartKycSessionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateSession()
        {
            var userId = User.FindFirstValue(CustomClaimTypes.Uid);
            var result = await _mediator.Send(new StartKycSessionCommand(userId));

            return result.ToApiResponse();
        }

        /// <summary>
        /// Receives webhook notifications from Didit regarding KYC session updates.
        /// </summary>
        /// <remarks>
        /// This endpoint is called by Didit when a user's KYC session status changes.
        /// The request contains a JSON payload describing the session and status.
        ///
        /// The server verifies the webhook signature using the shared secret.
        /// Once verified, the KYC session status is updated in the database.
        ///
        /// Sample request body:
        ///
        ///     POST /api/kyc/callback
        ///     Content-Type: application/json
        ///     X-Signature-V2: "abc123signature"
        ///
        ///     {
        ///         "session_id": "abc123456",
        ///         "status": "approved",
        ///         "webhook_type": "session_status",
        ///         "workflow_id": "workflow123",
        ///         "vendor_data": "user123"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Webhook processed successfully</response>
        /// <response code="400">Invalid webhook signature or session not found</response>
        [HttpPost("callback")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<Unit>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Callback()
        {
            Request.EnableBuffering();

            string rawBody;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                rawBody = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
            }

            var dto = System.Text.Json.JsonSerializer.Deserialize<DiditWebhookDto>(rawBody);

            var signature = Request.Headers["X-Signature-V2"].FirstOrDefault();

            var result = await _mediator.Send(new HandleKycWebhookCommand(dto, signature, rawBody));

            return result.ToApiResponse();
        }



        //[Authorize]
        //[HttpPost("start")]
        //public async Task<IActionResult> StartKyc()
        //{
        //    var userId = User.FindFirstValue(CustomClaimTypes.Uid);
        //    var verificationUrl = await _mediator.Send(new StartKycSessionCommand(userId));
        //    return Ok(new { verificationUrl });
        //}

        //[HttpPost("webhook")]
        //public async Task<IActionResult> Webhook([FromBody] DiditWebhookDto dto)
        //{
        //    var signature = Request.Headers["X-Signature"].FirstOrDefault();

        //    if (!VerifySignature(dto, signature))
        //        return Unauthorized();

        //    await _mediator.Send(new HandleKycWebhookCommand(dto.SessionId, dto.Status));
        //    return Ok();
        //}

        //private bool VerifySignature(DiditCallbackRequest dto, string signature)
        //{
        //    var secret = _configuration["DiditSettings:WebhookSecret"];

        //    using var hmac = new System.Security.Cryptography.HMACSHA256(
        //        System.Text.Encoding.UTF8.GetBytes(secret));

        //    var payload = $"{dto.SessionId}|{dto.Status}";
        //    var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));

        //    var computedSignature = Convert.ToBase64String(hash);

        //    return computedSignature == signature;
        //}
    }
}
