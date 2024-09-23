using FluentValidation;

namespace CCI.Model.OAuthModels
{
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        public RegisterModelValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username cannot be empty");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Username cannot be empty");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Username cannot be empty");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email can't be empty.")
                    .NotNull().WithMessage("Email can't be empty.")
                    .EmailAddress().WithMessage("Invalid Email");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number cannot be empty");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[""!@$%^&*(){}:;<>,.?/+\-_=|'[\]~\\]").WithMessage("Your password must contain one or more special characters.");
        }
    }
}
