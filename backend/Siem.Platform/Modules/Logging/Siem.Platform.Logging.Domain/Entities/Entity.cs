namespace Siem.Platform.Logging.Domain.Entities;

public abstract class Entity
{
    public DateTime CreatedAt { get; private set; }

    protected Entity()
    {
        CreatedAt = DateTime.UtcNow;
    }

}
