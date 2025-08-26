using FluentValidation;

namespace Siem.Platform.User.Application.Features.Auth.VerifyEmail;

public sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithErrorCode("activation.user_id.required")
            .Must(id => Guid.TryParse(id, out _))
                .WithMessage("Invalid user id format.")
                .WithErrorCode("activation.user_id.invalid");

        RuleFor(x => x.ActivationToken)
            .NotEmpty().WithErrorCode("activation.token.required")
            .Matches("^[A-Za-z0-9_-]+$")
                .WithMessage("Token format is invalid.")
                .WithErrorCode("activation.token.invalid")
            .MinimumLength(16).WithErrorCode("activation.token.too_short")
            .MaximumLength(256).WithErrorCode("activation.token.too_long");
    }
}
