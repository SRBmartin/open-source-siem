using FluentValidation;

namespace Siem.Platform.Logging.Application.Features.Tags.GetById;

internal sealed class GetTagByIdQueryValidator : AbstractValidator<GetTagByIdQuery>
{
    public GetTagByIdQueryValidator()
    {
        RuleFor(x => x.TagId).NotEmpty().WithErrorCode("logging.tag_id.required");
        RuleFor(x => x.UserId).NotEmpty().WithErrorCode("logging.user_id.required");
    }
}