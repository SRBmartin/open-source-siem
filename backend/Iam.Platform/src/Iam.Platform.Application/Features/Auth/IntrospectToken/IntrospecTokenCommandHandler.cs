using EBus.Abstractions;
using Iam.Platform.Application.Interfaces;
using System.Reflection.PortableExecutable;

namespace Iam.Platform.Application.Features.Auth.IntrospectToken;

public class IntrospecTokenCommandHandler (
    ITokenValidationService tokenValidator    
) : IRequestHandler<IntrospectTokenCommand, bool>
{

    public async Task<bool> Handle(IntrospectTokenCommand command, CancellationToken cancellationToken)
    {
        var authToken = command.Token;
        if (string.IsNullOrEmpty(authToken) || !authToken.StartsWith("Bearer "))
            return false;

        var token = authToken.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token))
            return false;

        var active = await tokenValidator.ValidateTokenAsync(token, cancellationToken);

        return active;
    }

}
