using FluentValidation;

namespace Siem.Platform.Logging.Application.Features.Tags.GrantAccess;

internal sealed class GrantLogTagAccessCommandValidator : AbstractValidator<GrantLogTagAccessCommand>
{
    public GrantLogTagAccessCommandValidator()
    {
        RuleFor(x => x.TagId).NotEmpty().WithErrorCode("logging.tag_id.required");
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode("logging.user_id.required");
        RuleFor(x => x.Role).IsInEnum().WithErrorCode("logging.role.invalid");
    }
}