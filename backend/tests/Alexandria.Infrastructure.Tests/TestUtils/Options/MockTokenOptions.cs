using System.Security.Cryptography;
using Alexandria.Infrastructure.Common.Options;
using Microsoft.Extensions.Options;

namespace Alexandria.Infrastructure.Tests.TestUtils.Options;

public class MockTokenOptions : IOptions<TokenOptions>
{
    public TokenOptions Value { get; } = new TokenOptions
    {
        PrivateKey = GenerateRandomBase64(128)
    };

    private static string GenerateRandomBase64(int length)
    {
        var randomBytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }
}