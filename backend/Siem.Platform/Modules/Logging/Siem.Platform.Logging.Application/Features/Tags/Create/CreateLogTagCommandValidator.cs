using FluentValidation;

namespace Siem.Platform.Logging.Application.Features.Tags.Create;

public sealed class CreateLogTagCommandValidator : AbstractValidator<CreateLogTagCommand>
{
    public CreateLogTagCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode("logging.tag_name.required")
            .Matches("^[a-z0-9]([a-z0-9-]*[a-z0-9])?$")
            .WithMessage("name must be [a-z0-9-], no leading/trailing '-'")
            .WithErrorCode("logging.tag_name.invalid");

        RuleFor(x => x.Partitions)
            .GreaterThan(0)
            .When(x => x.Partitions.HasValue)
            .WithErrorCode("logging.partitions.invalid");

        RuleFor(x => x.RetentionDays)
            .GreaterThan(0)
            .When(x => x.RetentionDays.HasValue)
            .WithErrorCode("logging.retention_days.invalid");
    }
}
