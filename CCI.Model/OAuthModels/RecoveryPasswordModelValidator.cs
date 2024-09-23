using FluentValidation;

namespace CCI.Model.OAuthModels
{
    public class RecoveryPasswordModelValidator : AbstractValidator<RecoveryPasswordModel>
    {
        public RecoveryPasswordModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email can't be empty.")
                    .NotNull().WithMessage("Email can't be empty.")
                    .EmailAddress().WithMessage("Invalid Email");
        }
    }
}
