namespace Siem.Platform.User.Application.Contracts;

public interface IRandomGenerator
{
    string GenerateRandomPassword(int length = 12, bool requireUpper = true, bool requireLower = true, bool requireDigit = true, bool requireSymbol = true);
}
