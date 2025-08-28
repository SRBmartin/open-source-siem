namespace Keycloak.Seeder.Application.Contracts;

public interface ISeederWorker
{
    Task RunAsync(CancellationToken cancellationToken = default);
}
