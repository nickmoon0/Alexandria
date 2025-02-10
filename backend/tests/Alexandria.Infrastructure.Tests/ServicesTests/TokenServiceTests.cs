using Alexandria.Application.Common.Constants;
using Alexandria.Common.Tests.Services;
using Alexandria.Infrastructure.Services;
using Alexandria.Domain.Tests.TestUtils.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Alexandria.Infrastructure.Tests.ServicesTests;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly ILogger<TokenService> _mockLogger;

    private readonly Guid _validDocumentId = Guid.NewGuid();
    private const int VALID_EXPIRY_MINUTES = 10;
    private static readonly DateTime FixedTestDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    public TokenServiceTests()
    {
        var testDateTimeProvider = new TestDateTimeProvider(FixedTestDate);
        _mockLogger = new TestLogger<TokenService>();

        _tokenService = new TokenService(testDateTimeProvider, _mockLogger);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidToken()
    {
        // Act
        var token = _tokenService.GenerateToken(_validDocumentId, VALID_EXPIRY_MINUTES, FilePermissions.Read, FilePermissions.Write);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var parts = token.Split('|');
        parts.Should().HaveCount(4);
        parts[0].Should().Be(_validDocumentId.ToString()); // Ensure GUID is correctly formatted
        parts[1].Should().Contain("Read,Write"); // Ensure permissions are included
    }

    [Fact]
    public void ValidateToken_ShouldReturnTrueForValidToken()
    {
        // Arrange
        var expectedPermissions = new[] { FilePermissions.Read, FilePermissions.Write };
        var token = _tokenService.GenerateToken(_validDocumentId, VALID_EXPIRY_MINUTES, expectedPermissions);

        // Act
        var result = _tokenService.ValidateToken(token, out var extractedDocumentId, out var extractedPermissions);

        // Assert
        result.Should().BeTrue();
        extractedDocumentId.Should().Be(_validDocumentId);
        extractedPermissions.Should().BeEquivalentTo(expectedPermissions);
    }

    [Fact]
    public void ValidateToken_ShouldReturnFalseForExpiredToken()
    {
        // Arrange
        var expiredDateTimeProvider = new TestDateTimeProvider(FixedTestDate.AddMinutes(-5));
        var expiredTokenService = new TokenService(expiredDateTimeProvider, _mockLogger);
        var token = expiredTokenService.GenerateToken(_validDocumentId, -5, FilePermissions.Read);

        // Act
        var result = _tokenService.ValidateToken(token, out var extractedDocumentId, out var extractedPermissions);

        // Assert
        result.Should().BeFalse();
        extractedDocumentId.Should().BeNull();
        extractedPermissions.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_ShouldReturnFalseForTamperedToken()
    {
        // Arrange
        var token = _tokenService.GenerateToken(_validDocumentId, VALID_EXPIRY_MINUTES, FilePermissions.Read);

        // Tamper with the token by modifying the document ID
        var parts = token.Split('|');
        parts[0] = Guid.NewGuid().ToString(); // Change document ID
        var tamperedToken = string.Join('|', parts);

        // Act
        var result = _tokenService.ValidateToken(tamperedToken, out var extractedDocumentId, out var extractedPermissions);

        // Assert
        result.Should().BeFalse();
        extractedDocumentId.Should().BeNull();
        extractedPermissions.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ValidateToken_ShouldReturnFalseForEmptyOrNullToken(string? invalidToken)
    {
        // Act
        var result = _tokenService.ValidateToken(invalidToken!, out var extractedDocumentId, out var extractedPermissions);

        // Assert
        result.Should().BeFalse();
        extractedDocumentId.Should().BeNull();
        extractedPermissions.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_ShouldReturnFalseForMalformedToken()
    {
        // Arrange
        var malformedToken = "Invalid|Format"; // Only two parts instead of three

        // Act
        var result = _tokenService.ValidateToken(malformedToken, out var extractedDocumentId, out var extractedPermissions);

        // Assert
        result.Should().BeFalse();
        extractedDocumentId.Should().BeNull();
        extractedPermissions.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_ShouldCorrectlyExtractPermissions()
    {
        // Arrange
        var expectedPermissions = new[] { FilePermissions.Read, FilePermissions.Write };
        var token = _tokenService.GenerateToken(_validDocumentId, VALID_EXPIRY_MINUTES, expectedPermissions);

        // Act
        var result = _tokenService.ValidateToken(token, out var extractedDocumentId, out var extractedPermissions);

        // Assert
        result.Should().BeTrue();
        extractedDocumentId.Should().Be(_validDocumentId);
        extractedPermissions.Should().BeEquivalentTo(expectedPermissions);
    }

    [Fact]
    public void ValidateToken_ShouldHandleSinglePermissionCorrectly()
    {
        // Arrange
        var token = _tokenService.GenerateToken(_validDocumentId, VALID_EXPIRY_MINUTES, FilePermissions.Write);

        // Act
        var result = _tokenService.ValidateToken(token, out var extractedDocumentId, out var extractedPermissions);

        // Assert
        result.Should().BeTrue();
        extractedDocumentId.Should().Be(_validDocumentId);
        extractedPermissions.Should().BeEquivalentTo([FilePermissions.Write]);
    }

    [Fact]
    public void ValidateToken_ShouldHandleNoPermissionsCorrectly()
    {
        // Arrange
        var token = _tokenService.GenerateToken(_validDocumentId, VALID_EXPIRY_MINUTES);

        // Act
        var result = _tokenService.ValidateToken(token, out var extractedDocumentId, out var extractedPermissions);

        // Assert
        result.Should().BeTrue();
        extractedDocumentId.Should().Be(_validDocumentId);
        extractedPermissions.Should().BeEquivalentTo(Array.Empty<FilePermissions>());
    }

    [Fact]
    public void ValidateToken_ShouldReturnFalseForInvalidGuid()
    {
        // Arrange: Tamper with documentId so it is not a valid GUID
        var token = _tokenService.GenerateToken(_validDocumentId, VALID_EXPIRY_MINUTES, FilePermissions.Read);
        var parts = token.Split('|');
        parts[0] = "InvalidGuidString";
        var tamperedToken = string.Join('|', parts);

        // Act
        var result = _tokenService.ValidateToken(tamperedToken, out var extractedDocumentId, out var extractedPermissions);

        // Assert
        result.Should().BeFalse();
        extractedDocumentId.Should().BeNull();
        extractedPermissions.Should().BeNull();
    }
}