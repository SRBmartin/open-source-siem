using FluentValidation;

namespace Siem.Platform.User.Application.Features.Auth.LoginCommand;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithErrorCode("username.required");
        RuleFor(x => x.Password).NotEmpty().WithErrorCode("password.required");
    }

}
