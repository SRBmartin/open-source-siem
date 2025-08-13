namespace Siem.Platform.User.Domain.Entities;

public class User : Entity
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public bool IsDeleted { get; private set; } = default!;

    private readonly List<ActivationToken> _activationTokens = new();
    public IReadOnlyCollection<ActivationToken> ActivationTokens => _activationTokens.AsReadOnly();

    private User() { }

    private User(Guid id, string email, string firstName, string lastName, bool isDeleted) : base()
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsDeleted = isDeleted;
    }

    public static User Create(string email, string firstName, string lastName, string id)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email required.", nameof(email));
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id required.", nameof(id));

        var user = new User(
            new(id),
            email.Trim(),
            firstName.Trim(),
            lastName.Trim(),
            false // IsDeleted will be set to false initially on creation
        );

        return user;

    }

}
