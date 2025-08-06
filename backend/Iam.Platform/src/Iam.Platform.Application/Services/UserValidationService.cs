using Iam.Platform.Application.DTOs.User;
using Iam.Platform.Application.Interfaces;
using System.Text.RegularExpressions;

namespace Iam.Platform.Application.Services;

public class UserValidationService : IUserValidationService
{
    public string? ValidateCreateUser(CreateUserDto userDto, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(userDto.Email))
            errors.Add("Email is required.");
        else if (!Regex.IsMatch(userDto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            errors.Add("Email format is invalid.");

        if (string.IsNullOrWhiteSpace(userDto.Password) || userDto.Password.Length < 6)
            errors.Add("Password must be at least 6 characters.");

        if (string.IsNullOrWhiteSpace(userDto.FirstName))
            errors.Add("First name is required.");

        if (string.IsNullOrWhiteSpace(userDto.LastName))
            errors.Add("Last name is required.");

        return errors.Count > 0 ? string.Join('\n', errors) : null!;
    }
}
