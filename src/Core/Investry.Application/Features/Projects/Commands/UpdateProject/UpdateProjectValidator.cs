using FluentValidation;

namespace Investry.Application.Features.Projects.Commands.UpdateProject
{
    public class UpdateProjectValidator : AbstractValidator<UpdateProjectCommand>
    {
        public UpdateProjectValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().MaximumLength(200)
                .When(x => x.Title != null);

            RuleFor(x => x.ShortDescription)
                .NotEmpty().MaximumLength(500)
                .When(x => x.ShortDescription != null);

            RuleFor(x => x.TargetAmount)
                .GreaterThan(0)
                .When(x => x.TargetAmount.HasValue);

            RuleFor(x => x.DurationInDays)
                .InclusiveBetween(7, 90).WithMessage("Campaign duration must be between 7 and 90 days.")
                .When(x => x.DurationInDays.HasValue);

            RuleFor(x => x.EquityPercentage)
                .InclusiveBetween(1, 100)
                .When(x => x.EquityPercentage.HasValue);
        }
    }
}
