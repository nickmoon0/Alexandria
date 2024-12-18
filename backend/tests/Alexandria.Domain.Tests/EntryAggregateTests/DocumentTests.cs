using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.EntryAggregate.Errors;
using Alexandria.Domain.Tests.TestUtils.Factories;
using Alexandria.Domain.Tests.TestUtils.Services;
using ErrorOr;
using FluentAssertions;

namespace Alexandria.Domain.Tests.EntryAggregateTests;

public class DocumentTests
{
    [Fact]
    public void Document_WithValidInputs_ShouldReturnDocument()
    {
        // Arrange
        var entry = EntryFactory.CreateEntry().Value;
        const string documentName = "ValidDocument";
        const string fileExtension = ".jpg";
        var data = new MemoryStream([1, 2, 3, 4, 5]);
        var imagePath = "test/image.jpg";
        var ownerId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dateTimeProvider = new TestDateTimeProvider(now);
        
        // Act
        var result = Document.Create(entry.Id, documentName, fileExtension, imagePath, ownerId, dateTimeProvider);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void Document_WithEmptyDocumentName_ShouldReturnError()
    {
        // Arrange
        const string documentName = "";

        // Act
        var result = DocumentFactory.CreateDocument(name: documentName);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidDocumentName);
    }

    [Fact]
    public void Document_WithTooLongDocumentName_ShouldReturnError()
    {
        // Arrange
        var documentName = new string('A', 101); // 101 characters

        // Act
        var result = DocumentFactory.CreateDocument(name: documentName);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidDocumentName);
    }
    

    [Fact]
    public void Document_WithEmptyOwnerId_ShouldReturnError()
    {
        // Arrange
        var createdById = Guid.Empty;

        // Act
        var result = DocumentFactory.CreateDocument(createdById: createdById);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidUserId);
    }

    [Fact]
    public void Document_WithMultipleInvalidInputs_ShouldReturnAllErrors()
    {
        // Arrange
        const string documentName = ""; // Invalid
        var createdById = Guid.Empty; // Invalid

        // Act
        var result = DocumentFactory.CreateDocument(name: documentName, createdById: createdById);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidDocumentName);
        result.Errors.Should().Contain(DocumentErrors.InvalidUserId);
    }
    
    [Fact]
    public void Rename_WithValidName_ShouldUpdateDocumentName()
    {
        // Arrange
        const string initialName = "InitialDocument";
        const string newName = "RenamedDocument";

        var document = DocumentFactory.CreateDocument(name: initialName).Value;

        // Act
        var result = document.Rename(newName);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void Rename_WithEmptyName_ShouldReturnError()
    {
        // Arrange
        const string initialName = "InitialDocument";
        const string newName = ""; // Invalid

        var document = DocumentFactory.CreateDocument(name: initialName).Value;

        // Act
        var result = document.Rename(newName);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidDocumentName);
    }

    [Fact]
    public void Rename_WithTooLongName_ShouldReturnError()
    {
        // Arrange
        const string initialName = "InitialDocument";
        var newName = new string('A', 101); // 101 characters, invalid

        var document = DocumentFactory.CreateDocument(name: initialName).Value;

        // Act
        var result = document.Rename(newName);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidDocumentName);
    }

    [Fact]
    public void Rename_WithWhitespaceName_ShouldReturnError()
    {
        // Arrange
        const string initialName = "InitialDocument";
        const string newName = "   "; // Invalid

        var document = DocumentFactory.CreateDocument(name: initialName).Value;

        // Act
        var result = document.Rename(newName);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidDocumentName);
    }
    
    [Fact]
    public void Delete_NotAlreadyDeletedDocument_ShouldReturnDeleted()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);

        var document = DocumentFactory.CreateDocument(dateTimeProvider: mockDateTimeProvider).Value;

        // Act
        var result = document.Delete(mockDateTimeProvider);

        // Assert
        result.IsError.Should().BeFalse();
        document.DeletedAtUtc.Should().Be(now);
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ShouldReturnError()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);

        var document = DocumentFactory.CreateDocument(dateTimeProvider: mockDateTimeProvider).Value;

        // Act
        document.Delete(mockDateTimeProvider); // Initial delete
        var result = document.Delete(mockDateTimeProvider); // Attempt to delete again

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.Failure());
    }

    [Fact]
    public void RecoverDeleted_WhenDeleted_ShouldReturnSuccess()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);

        var document = DocumentFactory.CreateDocument(dateTimeProvider: mockDateTimeProvider).Value;

        document.Delete(mockDateTimeProvider); // Mark as deleted

        // Act
        var result = document.RecoverDeleted();

        // Assert
        result.IsError.Should().BeFalse();
        document.DeletedAtUtc.Should().BeNull();
    }

    [Fact]
    public void RecoverDeleted_WhenNotDeleted_ShouldReturnError()
    {
        // Arrange
        var mockDateTimeProvider = new TestDateTimeProvider(DateTime.Now);
        var document = DocumentFactory.CreateDocument(dateTimeProvider: mockDateTimeProvider).Value;

        // Act
        var result = document.RecoverDeleted();

        // Assert
        result.IsError.Should().BeTrue();
        document.DeletedAtUtc.Should().BeNull();
    }
}