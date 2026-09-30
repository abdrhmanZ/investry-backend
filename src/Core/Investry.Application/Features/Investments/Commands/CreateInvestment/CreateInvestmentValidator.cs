using FluentValidation;

namespace Investry.Application.Features.Investments.Commands.CreateInvestment
{
    public class CreateInvestmentValidator : AbstractValidator<CreateInvestmentCommand>
    {
        public CreateInvestmentValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty();

            RuleFor(x => x.Amount)
                .GreaterThan(0);
        }
    }
}
