using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Categories.Queries.GetAllCategories
{
    public record GetAllCategoriesQuery() : IRequest<Result<List<CategoryDto>>>;
}
