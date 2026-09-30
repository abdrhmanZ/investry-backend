using Investry.API.Common.Extensions;
using Investry.Application.Constants;
using Investry.Application.Features.CapitalReturns.Commands.ReturnCapital;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Investry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapitalReturnsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CapitalReturnsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Founder")]
        [HttpPost("return")]
        public async Task<IActionResult> ReturnCapital([FromBody] ReturnCapitalRequest request)
            => (await _mediator.Send(new ReturnCapitalCommand(request, User.FindFirstValue(CustomClaimTypes.Uid)))).ToApiResponse();
        
    }
}
