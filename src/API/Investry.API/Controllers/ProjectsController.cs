  using AutoMapper;
using Investry.API.Common.Extensions;
using Investry.API.DTOs.Requests;
using Investry.Application.Common;
using Investry.Application.Constants;
using Investry.Application.Features.Projects.Commands.ActivateMudarabah;
using Investry.Application.Features.Projects.Commands.CreateProject;
using Investry.Application.Features.Projects.Commands.DeleteProject;
using Investry.Application.Features.Projects.Commands.UpdateProject;
using Investry.Application.Features.Projects.Queries.GetAllProjects;
using Investry.Application.Features.Projects.Queries.GetFounderProjects;
using Investry.Application.Features.Projects.Queries.GetProjectDetails;
using Investry.Application.Models.Media;
using Investry.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Investry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ProjectsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [Authorize(Roles = "Founder")]
        [HttpPost("create-project")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateProject([FromForm] CreateProjectRequest request)
        {
            var command = _mapper.Map<CreateProjectCommand>(request);
            command.UserId = User.FindFirstValue(CustomClaimTypes.Uid);

            if (request.CoverImage != null)
            {
                command.CoverImage = new FileDto
                {
                    Content = request.CoverImage.OpenReadStream(),
                    FileName = request.CoverImage.FileName,
                    ContentType = request.CoverImage.ContentType
                };
            }

            if (request.MediaFiles != null)
            {
                command.MediaFiles = request.MediaFiles.Select(file => new FileDto
                {
                    Content = file.OpenReadStream(),
                    FileName = file.FileName,
                    ContentType = file.ContentType
                }).ToList();
            }

            return (await _mediator.Send(command)).ToApiResponse();

        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetProject(Guid id)
        {
            var query = new GetProjectDetailsQuery(id);
            return (await _mediator.Send(query)).ToApiResponse();
        }

        [Authorize(Roles = "Founder")]
        [HttpPost("{id}/activate-mudarabah")]
        public async Task<IActionResult> ActivateMudarabah(Guid id)
        {
            return (await _mediator.Send(new ActivateMudarabahCommand(id, User.FindFirstValue(CustomClaimTypes.Uid)))).ToApiResponse();
        }

        [Authorize(Roles = "Founder")]
        [HttpGet("my-projects")]

        [ProducesResponseType(typeof(Result<List<ProjectSummaryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProjects()
            => (await _mediator.Send(new GetFounderProjectsQuery(User.FindFirstValue(CustomClaimTypes.Uid)))).ToApiResponse();

        //[HttpGet("dashboard-projects")]
        //[ProducesResponseType(typeof(Result<List<ProjectSummaryDto>>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetDashboardProjects() 
        //    => (await _mediator.Send(new GetFounderProjectsQuery(User.FindFirstValue(CustomClaimTypes.Uid)))).ToApiResponse();

        [HttpGet("all-projects")]
        [ProducesResponseType(typeof(Result<List<ProjectSummaryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProjects()
            => (await _mediator.Send(new GetAllProjectsQuery())).ToApiResponse();

        [HttpDelete("{id}")]
        [Authorize(Roles = "Founder")]
        public async Task<IActionResult> DeleteProject(Guid id)
            => (await _mediator.Send(new DeleteProjectCommand(id, User.FindFirstValue(CustomClaimTypes.Uid)))).ToApiResponse();

        [HttpPatch("{id}")]
        [Authorize(Roles = "Founder")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromForm] UpdateProjectApiRequest request)
        {
            var command = _mapper.Map<UpdateProjectCommand>(request);
            command.ProjectId = id;
            command.UserId = User.FindFirstValue(CustomClaimTypes.Uid);

            if (request.NewMediaFiles != null)
            {
                command.NewMediaFiles = new List<FileDto>();
                foreach (var file in request.NewMediaFiles)
                {
                    command.NewMediaFiles.Add(new FileDto
                    {
                        Content = file.OpenReadStream(),
                        FileName = file.FileName,
                        ContentType = file.ContentType
                    });
                }
            }
            return (await _mediator.Send(command)).ToApiResponse();
        }
    }
}


