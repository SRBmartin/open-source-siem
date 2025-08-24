using FluentValidation;

namespace Siem.Platform.User.Application.Features.User.ModifyRole;

public sealed class ModifyRoleCommandValidator : AbstractValidator<ModifyRoleCommand>
{
    public ModifyRoleCommandValidator()
    {
        RuleFor(x => x.InitiatorUserId)
            .NotEmpty().WithErrorCode("validation.initiator_user_id.required");

        RuleFor(x => x.TargetUserId)
            .NotEmpty().WithErrorCode("validation.target_user_id.required")
            .Must(s => Guid.TryParse(s, out _)).WithErrorCode("validation.target_user_id.invalid");

        RuleFor(x => x.Role)
            .NotEmpty().WithErrorCode("validation.role.required");

        RuleFor(x => x.Action)
            .NotEmpty().WithErrorCode("validation.action.required")
            .Must(a => a.Equals("add", StringComparison.OrdinalIgnoreCase) || a.Equals("remove", StringComparison.OrdinalIgnoreCase))
                .WithErrorCode("validation.action.invalid");

        RuleFor(x => x)
            .Must(x => !string.Equals(x.InitiatorUserId, x.TargetUserId, StringComparison.OrdinalIgnoreCase))
            .WithMessage("You cannot modify your own roles.")
            .WithErrorCode("authorization.self_edit_forbidden");
    }
}
