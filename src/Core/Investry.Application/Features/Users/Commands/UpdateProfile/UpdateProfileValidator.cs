using FluentValidation;

namespace Investry.Application.Features.Users.Commands.UpdateProfile
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.")
                .When(x => x.FirstName != null);

            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.")
                .When(x => x.LastName != null);

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Invalid phone number format.")
                .When(x => x.PhoneNumber != null);
        }
    }
}
