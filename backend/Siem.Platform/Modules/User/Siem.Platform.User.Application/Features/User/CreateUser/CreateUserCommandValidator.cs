using FluentValidation;

namespace Siem.Platform.User.Application.Features.User.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(t => t.Email)
            .NotEmpty().WithErrorCode("email.required")
            .EmailAddress().WithErrorCode("email.invalid");

        //RuleFor(t => t.Password)
        //    .NotEmpty().WithErrorCode("password.required")
        //    .MinimumLength(6).WithErrorCode("password.too_short")
        //    .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
        //    .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
        //    .Matches("[0-9]").WithMessage("Password must contain a digit.")
        //    .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a special character.");

        RuleFor(t => t.FirstName)
            .NotEmpty().WithErrorCode("first_name.required")
            .MinimumLength(3).WithErrorCode("first_name.too_short")
            .MaximumLength(100).WithErrorCode("first_name.too_long");

        RuleFor(t => t.LastName)
            .NotEmpty().WithErrorCode("last_name.required")
            .MinimumLength(3).WithErrorCode("last_name.too_short")
            .MaximumLength(100).WithErrorCode("last_name.too_long");

    }
}
