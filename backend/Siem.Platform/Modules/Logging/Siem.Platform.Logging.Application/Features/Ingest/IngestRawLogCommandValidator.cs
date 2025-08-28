using FluentValidation;

namespace Siem.Platform.Logging.Application.Features.Ingest;

internal sealed class IngestRawLogCommandValidator : AbstractValidator<IngestRawLogCommand>
{
    public IngestRawLogCommandValidator()
    {
        RuleFor(x => x.TagId).NotEmpty().WithErrorCode("logging.tag_id.required");
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode("logging.user_id.required");
        RuleFor(x => x.Message).NotEmpty().WithErrorCode("logging.message.required");
        RuleFor(x => x.Severity).NotEmpty().WithErrorCode("logging.severity.required");
    }
}
