using EBus.Abstractions;
using Siem.Platform.Logging.Application.DTOs.Tags;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;

namespace Siem.Platform.Logging.Application.Features.Tags.Create;

public sealed record CreateLogTagCommand(
    string Name,
    int? Partitions,
    int? RetentionDays,
    Guid? CreatedBy
) : IRequest<Result<CreateLogTagResultDto>>;
