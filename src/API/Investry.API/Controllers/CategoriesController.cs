using Investry.API.Common.Extensions;
using Investry.Application.Features.Categories.Commands.CreateCategory;
using Investry.Application.Features.Categories.Queries.GetAllCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Investry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories() 
            => (await _mediator.Send(new GetAllCategoriesQuery())).ToApiResponse();

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command) 
            => (await _mediator.Send(command)).ToApiResponse();
    }
}
