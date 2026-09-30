using FluentValidation;

namespace Investry.Application.Features.ProfitDistributions.Commands.DistributeProfit
{
    public class DistributeProfitValidator : AbstractValidator<DistributeProfitCommand>
    {
        public DistributeProfitValidator()
        {
            RuleFor(x => x.DistributeProfitRequest.ProjectId)
                .NotEmpty();

            RuleFor(x => x.DistributeProfitRequest.PeriodStart)
                .NotEmpty()
                .LessThan(x => x.DistributeProfitRequest.PeriodEnd)
                .WithMessage("Period start must be before period end");

            RuleFor(x => x.DistributeProfitRequest.PeriodEnd)
                .NotEmpty();

            RuleFor(x => x.DistributeProfitRequest.NetProfit)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Net profit cannot be negative");
        }
    }
}
