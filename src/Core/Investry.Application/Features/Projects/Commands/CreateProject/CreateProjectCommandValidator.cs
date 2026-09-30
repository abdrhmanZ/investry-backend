using FluentValidation;
using Investry.Domain.Enums;

namespace Investry.Application.Features.Projects.Commands.CreateProject
{
    public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
    {
        public CreateProjectCommandValidator()
        {
            RuleFor(x=>x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.ShortDescription)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.TargetAmount)
                .GreaterThan(0);

            RuleFor(x => x.StartDate)
                .GreaterThan(DateTime.UtcNow);

            RuleFor(x => x.CampaignDurationInDays)
                .InclusiveBetween(7, 90).WithMessage("Campaign duration must be between 7 and 90 days.");

            RuleFor(x => x.Location)
                .MaximumLength(200);

            RuleFor(x => x.PromotionalVideoURL)
                .MaximumLength(300);

            // Rewar Validation
            When(x => x.FundingModel == FundingModel.Reward, () =>
            {
                RuleFor(x => x.RewardTiers)
                .NotNull()
                .Must(tiers => tiers?.Any() ?? false)
                .WithMessage("At least one reward tier is required");

                RuleForEach(x => x.RewardTiers).ChildRules(tier =>
                {
                    tier.RuleFor(t => t.Title).NotEmpty();
                    tier.RuleFor(t => t.Amount).GreaterThan(0);
                });
            });

            // Equity Validation
            When(x => x.FundingModel == FundingModel.Equity, () =>
            {
                RuleFor(x => x.MinimumContribution)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Minimum contribution must be greater than zero.");

                RuleFor(x => x.EquityPercentage)
                    .NotNull()
                    .InclusiveBetween(1, 100)
                    .WithMessage("Equity percentage must be between 1 and 100.");
            });

            // Mudarabah Validation
            When(x => x.FundingModel == FundingModel.Mudarabah, () =>
            {
                RuleFor(x => x.MinimumContribution)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Minimum contribution must be greater than zero.");

                RuleFor(x => x.InvestorsProfitSharePercentage)
                    .NotNull()
                    .GreaterThan(0)
                    .LessThan(100)
                    .WithMessage("Investors profit percentage must be between 0 and 100.");

                RuleFor(x => x.DurationInMonths)
                    .NotNull()
                    .GreaterThan(0)
                    .LessThanOrEqualTo(120)
                    .WithMessage("Duration must be between 1 and 120 months.");

                RuleFor(x => x.ProfitDistributionFrequency)
                    .NotNull()
                    .IsInEnum()
                    .WithMessage("Valid profit distribution frequency is required.");
            });
        }
    }
}
