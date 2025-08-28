using FluentValidation;

namespace Siem.Platform.User.Application.Features.User.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithErrorCode("validation.user_id.required");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithErrorCode("validation.password.current.required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithErrorCode("validation.password.new.required")
            .MinimumLength(8).WithErrorCode("validation.password.new.too_short")
            .Matches("[A-Z]").WithErrorCode("validation.password.new.upper")
            .Matches("[a-z]").WithErrorCode("validation.password.new.lower")
            .Matches("[0-9]").WithErrorCode("validation.password.new.digit")
            .Matches("[^a-zA-Z0-9]").WithErrorCode("validation.password.new.special")
            .NotEqual(x => x.CurrentPassword).WithErrorCode("validation.password.new.same_as_current");
    }
}
