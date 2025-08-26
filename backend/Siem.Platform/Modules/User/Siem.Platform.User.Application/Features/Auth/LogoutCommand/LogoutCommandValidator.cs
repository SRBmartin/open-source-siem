using FluentValidation;

namespace Siem.Platform.User.Application.Features.Auth.LogoutCommand;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode("user_id.required");
    }
}
