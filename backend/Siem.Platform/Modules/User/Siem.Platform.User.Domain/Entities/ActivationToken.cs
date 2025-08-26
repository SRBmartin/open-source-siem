namespace Siem.Platform.User.Domain.Entities;

public class ActivationToken : Entity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }

    public User User { get; private set; }

    public ActivationToken() { }

    private ActivationToken(Guid id, Guid userId, string token, DateTimeOffset expiresAt, bool isUsed) : base()
    {
        Id = id;
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        IsUsed = isUsed;
    }

    public static ActivationToken Issue(Guid userId, string token, DateTimeOffset? now = null, TimeSpan? ttl = null)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Token required.", nameof(token));

        var created = now ?? DateTimeOffset.UtcNow;
        var duration = ttl ?? TimeSpan.FromDays(1);

        return new ActivationToken(Guid.NewGuid(), userId, token.Trim(), created.Add(duration), false);
    }

    public bool IsExpired(DateTimeOffset? at = null) => (at ?? DateTimeOffset.UtcNow) >= ExpiresAt;
    
    public void MarkUsed()
    {
        if (!IsUsed) IsUsed = true;
    }

}
