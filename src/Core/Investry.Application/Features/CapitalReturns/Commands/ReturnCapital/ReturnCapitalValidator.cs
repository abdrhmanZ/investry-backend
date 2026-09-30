using FluentValidation;

namespace Investry.Application.Features.CapitalReturns.Commands.ReturnCapital
{
    public class ReturnCapitalValidator : AbstractValidator<ReturnCapitalCommand>
    {
        public ReturnCapitalValidator()
        {
            RuleFor(x => x.ReturnCapitalRequest.ProjectId)
                .NotEmpty();
        }
    }
}
