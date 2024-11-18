using Alexandria.Domain.DocumentAggregate;
using Alexandria.Domain.Tests.TestUtils.Factories;
using Alexandria.Domain.Tests.TestUtils.Services;
using ErrorOr;
using FluentAssertions;

namespace Alexandria.Domain.Tests.DocumentAggregateTests;

public class DocumentTests
{
    [Fact]
    public void Document_WithValidInputs_ShouldReturnDocument()
    {
        // Arrange
        const string documentName = "ValidDocument";
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var imagePath = "test/image.jpg";
        var ownerId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dateTimeProvider = new TestDateTimeProvider(now);
        
        // Act
        var result = Document.Create(documentName, data, imagePath, ownerId, dateTimeProvider);

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
    public void Document_WithEmptyData_ShouldReturnError()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var result = DocumentFactory.CreateDocument(data: data);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.EmptyDocumentData);
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
        result.Errors.Should().Contain(DocumentErrors.InvalidOwnerId);
    }

    [Fact]
    public void Document_WithMultipleInvalidInputs_ShouldReturnAllErrors()
    {
        // Arrange
        const string documentName = ""; // Invalid
        var data = Array.Empty<byte>(); // Invalid
        var createdById = Guid.Empty; // Invalid

        // Act
        var result = DocumentFactory.CreateDocument(name: documentName, data: data, createdById: createdById);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidDocumentName);
        result.Errors.Should().Contain(DocumentErrors.EmptyDocumentData);
        result.Errors.Should().Contain(DocumentErrors.InvalidOwnerId);
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
    public void UpdateDescription_WithValidDescription_ShouldUpdateDescription()
    {
        // Arrange
        const string initialDescription = "InitialDescription";
        const string newDescription = "This is a new description";
        var document = DocumentFactory.CreateDocument(description: initialDescription).Value;

        // Act
        var result = document.UpdateDescription(newDescription);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void UpdateDescription_WithNullDescription_ShouldSetDescriptionToNull()
    {
        // Arrange
        const string initialDescription = "InitialDescription";
        var document = DocumentFactory.CreateDocument(description: initialDescription).Value;
        
        // Act
        var result = document.UpdateDescription(null);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void UpdateDescription_WithWhitespace_ShouldSetDescriptionToNull()
    {
        // Arrange
        const string initialDescription = "InitialDescription";
        const string whitespaceDescription = "   ";

        var document = DocumentFactory.CreateDocument(description: initialDescription).Value;

        // Act
        var result = document.UpdateDescription(whitespaceDescription);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void AddCharacter_WithValidCharacterId_ShouldAddToDocument()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var document = DocumentFactory.CreateDocument().Value;

        // Act
        var result = document.AddCharacter(characterId);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void AddCharacter_WithEmptyCharacterId_ShouldReturnError()
    {
        // Arrange
        var characterId = Guid.Empty; // Invalid character ID
        var document = DocumentFactory.CreateDocument().Value;

        // Act
        var result = document.AddCharacter(characterId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidCharacterId);
    }

    [Fact]
    public void AddCharacter_WithDuplicateCharacterId_ShouldReturnError()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var document = DocumentFactory.CreateDocument().Value;
        document.AddCharacter(characterId); // Add once

        // Act
        var result = document.AddCharacter(characterId); // Attempt to add again

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.CharacterIdAlreadyPresent);
    }

    [Fact]
    public void RemoveCharacter_WithExistingCharacterId_ShouldRemoveFromDocument()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var document = DocumentFactory.CreateDocument().Value;
        document.AddCharacter(characterId); // Add first

        // Act
        var result = document.RemoveCharacter(characterId);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void RemoveCharacter_WithNonExistingCharacterId_ShouldReturnError()
    {
        // Arrange
        var document = DocumentFactory.CreateDocument().Value;
        var characterId = Guid.NewGuid(); // Never added

        // Act
        var result = document.RemoveCharacter(characterId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.CharacterIdNotPresent);
    }

    [Fact]
    public void RemoveCharacter_WithEmptyCharacterId_ShouldReturnError()
    {
        // Arrange
        var document = DocumentFactory.CreateDocument().Value;
        var characterId = Guid.Empty; // Invalid character ID

        // Act
        var result = document.RemoveCharacter(characterId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidCharacterId);
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