using EBus.Abstractions;

namespace Iam.Platform.Application.Features.Auth.IntrospectToken;

public record IntrospectTokenCommand(string? Token) : IRequest<bool>;
