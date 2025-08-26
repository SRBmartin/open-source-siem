using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Infrastructure.Identity.Models;

namespace Iam.Platform.Infrastructure.Identity.Mappers;

public static class CreateUserDtoExtensions
{
    public static KeycloakCreateUserRequest ToKeycloakRequest(this CreateUserDto dto)
    {
        return new KeycloakCreateUserRequest
        {
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Username = dto.Email,
            EmailVerified = false,
            Enabled = true,
            Credentials = new List<KeycloakUserCredential>
            {
                new()
                {
                    Type = "password",
                    Value = dto.Password,
                    Temporary = false
                }
            }
        };

    }

}
