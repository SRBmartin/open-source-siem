using EBus.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Siem.Platform.Logging.Application.Contracts;
using Siem.Platform.Logging.Application.DTOs.Tags;
using Siem.Platform.Logging.Domain.Entities;
using Siem.Platform.Logging.Domain.Repositories;
using Siem.Platform.Shared.Application.Abstractions.Common.Http;
using System.Text.RegularExpressions;

namespace Siem.Platform.Logging.Application.Features.Tags.Create;

public class CreateLogTagCommandHandler (
    ILogTagRepository logTagRepository,
    ILogTagAccessRepository logTagAccessRepository,
    ITopicProvisionerService topicProvisionerService,
    ILoggingDefaultsService loggingDefaultsService,
    IIngestInfoService ingestInfoService,
    ILogger<CreateLogTagCommandHandler> logger
) : IRequestHandler<CreateLogTagCommand, Result<CreateLogTagResultDto>>
{
    public async Task<Result<CreateLogTagResultDto>> Handle(CreateLogTagCommand cmd, CancellationToken ct)
    {
        var slug = Slug(cmd.Name);

        if (await logTagRepository.ExistsByNameAsync(slug, ct))
            return Result<CreateLogTagResultDto>.Failure(
                new Error("logging.tag.exists", "A tag with that name already exists."));

        var retentionMs = cmd.RetentionDays.HasValue
            ? cmd.RetentionDays.Value * 24L * 60 * 60 * 1000
            : (long?)null;

        try
        {
            await topicProvisionerService.EnsureTopicsAsync(slug, cmd.Partitions, retentionMs, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to ensure topics for tag {Tag}", slug);
            return Result<CreateLogTagResultDto>.Failure(
                new Error("logging.topic.provision_failed", "Failed to create Kafka topics."));
        }

        var topic = topicProvisionerService.BuildTopic(slug);

        var tag = LogTag.Create(
            name: slug,
            topic: topic,
            partitions: cmd.Partitions ?? loggingDefaultsService.DefaultPartitions,
            retentionMs: retentionMs ?? loggingDefaultsService.DefaultRetentionMs,
            createdByUserId: cmd.CreatedBy);

        await logTagRepository.AddAsync(tag, ct);
        await logTagRepository.SaveChangesAsync(ct);

        if (cmd.CreatedBy.HasValue)
        {
            var access = LogTagAccess.Grant(tag.Id, cmd.CreatedBy.Value, LogAccessRole.Admin);
            await logTagAccessRepository.AddAsync(access, ct);
            await logTagAccessRepository.SaveChangesAsync(ct);
        }

        var dto = new CreateLogTagResultDto(
            tag.Id, slug, topic, ingestInfoService.Build(topic));

        return Result<CreateLogTagResultDto>.Success(dto);
    }

    private static string Slug(string s) =>
        Regex.Replace(s.Trim().ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');

}
