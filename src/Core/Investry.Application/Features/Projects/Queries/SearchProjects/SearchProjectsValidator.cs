using FluentValidation;

namespace Investry.Application.Features.Projects.Queries.SearchProjects
{
    public class SearchProjectsValidator : AbstractValidator<SearchProjectsQuery>
    {
        public SearchProjectsValidator()
        {
            RuleFor(query => query.Search).MaximumLength(200);
            RuleFor(query => query.Page).GreaterThan(0);
            RuleFor(query => query.PageSize).InclusiveBetween(1, 50);
            RuleFor(query => query.FundingModel)
                .Must(model => !model.HasValue || Enum.IsDefined(model.Value))
                .WithMessage("Funding model must be Reward, Equity or Mudarabah.");

            RuleFor(query => query.Page)
                .Must((query, page) => ((long)page - 1) * query.PageSize <= int.MaxValue)
                .WithMessage("The requested page is too large.")
                .When(query => query.Page > 0 && query.PageSize > 0);
        }
    }
}
