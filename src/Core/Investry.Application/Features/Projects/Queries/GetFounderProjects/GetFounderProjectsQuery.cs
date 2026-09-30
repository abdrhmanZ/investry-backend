using Investry.Application.Common;
using Investry.Application.Features.Projects.Queries.GetAllProjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investry.Application.Features.Projects.Queries.GetFounderProjects
{
    public record GetFounderProjectsQuery(string UserId) : IRequest<Result<IReadOnlyList<FounderProjectDto>>>;
}
