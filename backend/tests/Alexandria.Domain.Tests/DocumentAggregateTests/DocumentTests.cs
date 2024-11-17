using Alexandria.Domain.DocumentAggregate;
using Alexandria.Domain.Tests.TestUtils.Services;
using FluentAssertions;

namespace Alexandria.Domain.Tests.DocumentAggregateTests;

public class DocumentTests
{
    [Fact]
    public void CreateDocument_WithValidInputs_ShouldReturnDocument()
    {
        // Arrange
        const string documentName = "ValidDocument";
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var ownerId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dateTimeProvider = new TestDateTimeProvider(now);
        
        // Act
        var result = Document.Create(documentName, data, ownerId, dateTimeProvider);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.DocumentName.Should().Be(documentName);
        result.Value.Data.Should().BeEquivalentTo(data);
        result.Value.OwnerId.Should().Be(ownerId);
        result.Value.CreatedDateUtc.Should().Be(now);
    }

    [Fact]
    public void CreateDocument_WithEmptyDocumentName_ShouldReturnError()
    {
        // Arrange
        const string documentName = "";
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var ownerId = Guid.NewGuid();
        var dateTimeProvider = new TestDateTimeProvider();

        // Act
        var result = Document.Create(documentName, data, ownerId, dateTimeProvider);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidDocumentName);
    }

    [Fact]
    public void CreateDocument_WithTooLongDocumentName_ShouldReturnError()
    {
        // Arrange
        var documentName = new string('A', 101); // 101 characters
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var ownerId = Guid.NewGuid();
        var dateTimeProvider = new TestDateTimeProvider();

        // Act
        var result = Document.Create(documentName, data, ownerId, dateTimeProvider);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidDocumentName);
    }

    [Fact]
    public void CreateDocument_WithEmptyData_ShouldReturnError()
    {
        // Arrange
        const string documentName = "ValidDocument";
        var data = Array.Empty<byte>();
        var ownerId = Guid.NewGuid();
        var dateTimeProvider = new TestDateTimeProvider();

        // Act
        var result = Document.Create(documentName, data, ownerId, dateTimeProvider);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.EmptyDocumentData);
    }

    [Fact]
    public void CreateDocument_WithEmptyOwnerId_ShouldReturnError()
    {
        // Arrange
        const string documentName = "ValidDocument";
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var ownerId = Guid.Empty;
        var dateTimeProvider = new TestDateTimeProvider();

        // Act
        var result = Document.Create(documentName, data, ownerId, dateTimeProvider);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidOwnerId);
    }

    [Fact]
    public void CreateDocument_WithMultipleInvalidInputs_ShouldReturnAllErrors()
    {
        // Arrange
        const string documentName = ""; // Invalid
        var data = Array.Empty<byte>(); // Invalid
        var ownerId = Guid.Empty; // Invalid
        var dateTimeProvider = new TestDateTimeProvider();

        // Act
        var result = Document.Create(documentName, data, ownerId, dateTimeProvider);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidDocumentName);
        result.Errors.Should().Contain(DocumentErrors.EmptyDocumentData);
        result.Errors.Should().Contain(DocumentErrors.InvalidOwnerId);
    }
}