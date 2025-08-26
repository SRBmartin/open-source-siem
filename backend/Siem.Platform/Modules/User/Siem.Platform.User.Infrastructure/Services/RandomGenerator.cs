using Siem.Platform.User.Application.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace Siem.Platform.User.Infrastructure.Services;

public class RandomGenerator : IRandomGenerator
{
    private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string Lower = "abcdefghijkmnpqrstuvwxyz";
    private const string Digits = "23456789";
    private const string Symbols = "@#$%&*?-_+";

    public string GenerateRandomPassword(int length = 12, bool requireUpper = true, bool requireLower = true, bool requireDigit = true, bool requireSymbol = true)
    {
        if (length < 8) length = 8;

        var pools = new List<string>();
        if (requireUpper) pools.Add(Upper);
        if (requireLower) pools.Add(Lower);
        if (requireDigit) pools.Add(Digits);
        if (requireSymbol) pools.Add(Symbols);

        if (pools.Count == 0)
            pools.Add(Upper + Lower + Digits);

        var all = string.Concat(pools);
        var bytes = RandomNumberGenerator.GetBytes(length);
        var sb = new StringBuilder(length);

        foreach (var pool in pools)
            sb.Append(pool[GetIndex(pool.Length)]);

        while (sb.Length < length)
            sb.Append(all[GetIndex(all.Length)]);

        return Shuffle(sb.ToString());

        int GetIndex(int max) => RandomNumberGenerator.GetInt32(max);
    }

    private static string Shuffle(string input)
    {
        var chars = input.ToCharArray();
        for (int i = chars.Length - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
        return new string(chars);
    }

    public string GenerateUriSafeToken(int byteLength = 32)
    {
        var bytes = RandomNumberGenerator.GetBytes(byteLength);
        var base64 = Convert.ToBase64String(bytes);

        return base64.Replace('+', '-')
                     .Replace('/', '_')
                     .TrimEnd('=');
    }

    public string HashTokenSha256(string token)
    {
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
