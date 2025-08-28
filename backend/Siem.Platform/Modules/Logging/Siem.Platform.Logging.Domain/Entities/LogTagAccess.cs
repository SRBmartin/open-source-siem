namespace Siem.Platform.Logging.Domain.Entities;

public enum LogAccessRole { Reader = 0, Admin = 1 }

public sealed class LogTagAccess : Entity
{
    public Guid Id { get; private set; }
    public Guid TagId { get; private set; }
    public Guid UserId { get; private set; }
    public LogAccessRole Role { get; private set; }
    public bool IsDeleted { get; private set; }

    private LogTagAccess() { }

    public static LogTagAccess Grant(Guid tagId, Guid userId, LogAccessRole role)
    {
        if (tagId == Guid.Empty || userId == Guid.Empty) throw new ArgumentException("ids required");
        return new LogTagAccess { Id = Guid.NewGuid(), TagId = tagId, UserId = userId, Role = role };
    }

    public void Revoke() => IsDeleted = true;
    public void Promote(LogAccessRole role) => Role = role;
}

