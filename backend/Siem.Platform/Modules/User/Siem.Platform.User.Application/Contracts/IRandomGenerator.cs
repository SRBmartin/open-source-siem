namespace Siem.Platform.User.Application.Contracts;

public interface IRandomGenerator
{
    /// <summary>
    /// Based on given parameters, generates a random password.
    /// </summary>
    /// <param name="length">Fixed length of the password</param>
    /// <param name="requireUpper">Should contain uppercase letters</param>
    /// <param name="requireLower">Should contain lowercase letters</param>
    /// <param name="requireDigit">Should contain digits</param>
    /// <param name="requireSymbol">Should contain special simbols</param>
    /// <returns>Randomly generated password</returns>
    string GenerateRandomPassword(int length = 12, bool requireUpper = true, bool requireLower = true, bool requireDigit = true, bool requireSymbol = true);

    /// <summary>
    /// Generates a URI-safe token of specified byte length.
    /// </summary>
    /// <param name="byteLength">Length of the token to generate</param>
    /// <returns>Token escaped for Base64Url encoding (RFC 4648)</returns>
    string GenerateUriSafeToken(int byteLength = 32);

    /// <summary>
    /// Generates a SHA256 hash of the provided token.
    /// </summary>
    /// <param name="token">Token to be hashed</param>
    /// <returns>Hash value of passed token</returns>
    string HashTokenSha256(string token);
}
