using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string Name, string? Description) : IRequest<Result<Guid>>;
}
