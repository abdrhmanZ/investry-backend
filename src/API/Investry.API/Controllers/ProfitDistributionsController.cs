using Investry.API.Common.Extensions;
using Investry.Application.Constants;
using Investry.Application.Features.ProfitDistributions.Commands.DistributeProfit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Investry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfitDistributionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfitDistributionsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [Authorize(Roles = "Founder")]
        [HttpPost("distribute")]
        public async Task<IActionResult> DistributeProfit([FromBody] DistributeProfitRequest request)
            => (await _mediator.Send(new DistributeProfitCommand(request, User.FindFirstValue(CustomClaimTypes.Uid)))).ToApiResponse();
        
    }
}
