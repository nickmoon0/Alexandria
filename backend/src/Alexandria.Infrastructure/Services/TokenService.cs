using System.Security.Cryptography;
using System.Text;
using Alexandria.Application.Common.Constants;
using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Infrastructure.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Alexandria.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<TokenService> _logger;
    private readonly TokenOptions _tokenOptions;
    
    private const char TOKEN_SEPARATOR = '|';
    private const char PERMISSIONS_SEPARATOR = ',';
    
    public TokenService(IDateTimeProvider dateTimeProvider, ILogger<TokenService> logger, IOptions<TokenOptions> tokenOptions)
    {
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _tokenOptions = tokenOptions.Value;
    }
    
    public string GenerateToken(Guid documentId, int expiryMinutes, params FilePermissions[] filePermissions)
    {
        var expiryTimestamp = new DateTimeOffset(_dateTimeProvider.UtcNow.AddMinutes(expiryMinutes))
            .ToUnixTimeSeconds();
        
        var dataToSign = GenerateDataToSign(documentId, filePermissions, expiryTimestamp);
        var signature = ComputeSignature(dataToSign);
        
        return $"{dataToSign}{TOKEN_SEPARATOR}{signature}";
    }

    public bool ValidateToken(string token, out Guid? documentId, out FilePermissions[]? filePermissions)
    {
        documentId = null;
        filePermissions = null;
        
        if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
        {
            _logger.LogInformation("Attempted to validate null or empty token");
            return false;
        }

        var parts = token.Split(TOKEN_SEPARATOR);
        if (parts.Length != 4)
        {
            _logger.LogInformation("Attempted to validate token with incorrect length");
            return false;
        }

        // Decompose token into individual parts
        if (!Guid.TryParse(parts[0], out var tempDocId))
        {
            _logger.LogInformation("Attempted to validate token with invalid guid for Document ID");
            return false;
        }
        var tempFilePermissions = parts[1]
            .Split(PERMISSIONS_SEPARATOR)
            .Select(x =>
            {
                Enum.TryParse<FilePermissions>(x, true, out var result);
                return result;
            })
            .Where(x => x != FilePermissions.None)
            .ToList();
        var expiryTimestamp = long.Parse(parts[2]);
        var providedSignature = parts[3];

        // Check Expiry and Signature validity
        var dateTimeNow = new DateTimeOffset(_dateTimeProvider.UtcNow).ToUnixTimeSeconds();
        if (dateTimeNow > expiryTimestamp)
        {
            _logger.LogInformation("Attempted to validate expired token");
            return false;
        }

        var dataToSign = GenerateDataToSign(tempDocId, [..tempFilePermissions], expiryTimestamp);
        var expectedSignature = ComputeSignature(dataToSign);
        if (expectedSignature != providedSignature)
        {
            _logger.LogInformation("Attempted to validate token that has been tampered with (signature does not match)");
            return false;
        }
        
        documentId = tempDocId;
        filePermissions = [..tempFilePermissions];
        return true;
    }

    private static string GenerateDataToSign(Guid documentId, FilePermissions[] filePermissions, long expiryTimestamp)
    {
        var filePermissionsString = filePermissions.Length <= 0 ? 
            "" : 
            filePermissions
                .Select(permission => permission.ToString())
                .Aggregate((prev, next) => $"{prev}{PERMISSIONS_SEPARATOR}{next}");
        
        return $"{documentId}{TOKEN_SEPARATOR}{filePermissionsString}{TOKEN_SEPARATOR}{expiryTimestamp}";
    }
        
    private string ComputeSignature(string dataToSign)
    {
        using var hmac = new HMACSHA3_512(Encoding.UTF8.GetBytes(_tokenOptions.PrivateKey));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign)));
        return signature;
    }
}