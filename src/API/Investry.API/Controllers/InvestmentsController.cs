using Investry.API.Common.Extensions;
using Investry.Application.Common;
using Investry.Application.Constants;
using Investry.Application.Features.Investments.Commands.CreateInvestment;
using Investry.Application.Features.Investments.Queries.GetMyInvestments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Investry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvestmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Investor")]
        [HttpPost("create-investment")]
        public async Task<IActionResult> Invest([FromBody] CreateInvestmentRequest request)
        {
            var command = new CreateInvestmentCommand(
                UserId: User.FindFirstValue(CustomClaimTypes.Uid),
                ProjectId: request.ProjectId,
                Amount: request.Amount,
                RewardTierId: request.RewardTierId
            );

            return (await _mediator.Send(command)).ToApiResponse();
        }

        [Authorize(Roles = "Investor")]
        [HttpGet("my-investments")]
        [ProducesResponseType(typeof(Result<IReadOnlyList<InvestorInvestmentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyInvestments()
            => (await _mediator.Send(new GetMyInvestmentsQuery(
                User.FindFirstValue(CustomClaimTypes.Uid)))).ToApiResponse();
    }
}
