using FluentValidation;

namespace Siem.Platform.Logging.Application.Features.Tags.GetMyTags;

internal sealed class GetMyTagsQueryValidator : AbstractValidator<GetMyTagsQuery>
{
    public GetMyTagsQueryValidator()
        => RuleFor(x => x.UserId).NotEmpty().WithErrorCode("logging.user_id.required");
}