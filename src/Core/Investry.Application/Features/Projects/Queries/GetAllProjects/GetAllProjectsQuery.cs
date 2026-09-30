using Investry.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investry.Application.Features.Projects.Queries.GetAllProjects
{
    public record GetAllProjectsQuery() : IRequest<Result<IReadOnlyList<ProjectSummaryDto>>>;
}
