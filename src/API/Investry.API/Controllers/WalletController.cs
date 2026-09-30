using Investry.API.Common.Extensions;
using Investry.API.DTOs.Requests;
using Investry.Application.Constants;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Features.Wallet.Commands.CreateDepositSession;
using Investry.Application.Features.Wallet.Commands.ProcessWebhook;
using Investry.Application.Features.Wallet.Queries.GetWalletBalance;
using Investry.Application.Features.Wallet.Queries.GetWalletTransactions;
using Investry.Infrastructure.Payments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Investry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WalletController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> CreateDeposit([FromBody] DepositRequest request)
            => (await _mediator.Send(new CreateCheckoutSessionCommand(User.FindFirstValue(CustomClaimTypes.Uid), request.Amount))).ToApiResponse();

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            var payload = await new StreamReader(Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            return (await _mediator.Send(new ProcessWebhookCommand(payload, signature))).ToApiResponse();
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
            => (await _mediator.Send(new GetWalletBalanceQuery(User.FindFirstValue(CustomClaimTypes.Uid)))).ToApiResponse();

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => (await _mediator.Send(new GetWalletTransactionsQuery(User.FindFirstValue(CustomClaimTypes.Uid), page, pageSize))).ToApiResponse();
    }
}
