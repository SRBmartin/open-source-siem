using System.Text.Json.Serialization;

namespace Iam.Platform.Infrastructure.Identity.Models;

public sealed class KeycloakCredential
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "password";

    [JsonPropertyName("value")]
    public string Value { get; init; } = default!;

    [JsonPropertyName("temporary")]
    public bool Temporary { get; init; } = false;
}
