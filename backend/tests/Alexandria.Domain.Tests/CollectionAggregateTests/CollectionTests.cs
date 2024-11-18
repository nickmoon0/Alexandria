using Alexandria.Domain.CollectionAggregate;
using Alexandria.Domain.Tests.TestUtils.Services;
using ErrorOr;
using FluentAssertions;

namespace Alexandria.Domain.Tests.CollectionAggregateTests;

public class CollectionTests
{
    [Fact]
    public void Create_WithValidName_ShouldReturnCollection()
    {
        // Arrange
        var name = new string('A', 100); // Valid name with exactly 100 characters

        // Act
        var result = Collection.Create(name);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithInvalidName_ShouldReturnError()
    {
        // Arrange
        const string name = ""; // Invalid name

        // Act
        var result = Collection.Create(name);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(CollectionErrors.InvalidName);
    }

    [Fact]
    public void Create_WithNameOverMaximumLength_ShouldReturnError()
    {
        // Arrange
        var name = new string('A', 101);

        // Act
        var result = Collection.Create(name);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(CollectionErrors.InvalidName);
    }

    [Fact]
    public void Rename_WithValidName_ShouldUpdateCollectionName()
    {
        // Arrange
        var initialName = new string('A', 100);
        var newName = new string('B', 100);
        var collection = Collection.Create(initialName).Value;

        // Act
        var result = collection.Rename(newName);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void Rename_WithInvalidName_ShouldReturnError()
    {
        // Arrange
        var initialName = new string('A', 100);
        const string newName = ""; // Invalid name
        var collection = Collection.Create(initialName).Value;

        // Act
        var result = collection.Rename(newName);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(CollectionErrors.InvalidName);
    }

    [Fact]
    public void AddDocument_WithValidDocumentId_ShouldAddToCollection()
    {
        // Arrange
        var name = new string('A', 100);
        var collection = Collection.Create(name).Value;
        var documentId = Guid.NewGuid();

        // Act
        var result = collection.AddDocument(documentId);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void AddDocument_WithEmptyDocumentId_ShouldReturnError()
    {
        // Arrange
        var name = new string('A', 100);
        var collection = Collection.Create(name).Value;
        var documentId = Guid.Empty; // Invalid document ID

        // Act
        var result = collection.AddDocument(documentId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(CollectionErrors.InvalidDocumentId);
    }

    [Fact]
    public void RemoveDocument_WithExistingDocumentId_ShouldRemoveFromCollection()
    {
        // Arrange
        var name = new string('A', 100);
        var collection = Collection.Create(name).Value;
        var documentId = Guid.NewGuid();
        collection.AddDocument(documentId);

        // Act
        var result = collection.RemoveDocument(documentId);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void RemoveDocument_WithNonExistingDocumentId_ShouldReturnError()
    {
        // Arrange
        var name = new string('A', 100);
        var collection = Collection.Create(name).Value;
        var documentId = Guid.NewGuid();

        // Act
        var result = collection.RemoveDocument(documentId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(CollectionErrors.DocumentIdNotFound);
    }
    
        [Fact]
    public void Delete_NotAlreadyDeletedCollection_ShouldReturnDeleted()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);

        var collection = Collection.Create("Test Collection").Value;

        // Act
        var result = collection.Delete(mockDateTimeProvider);

        // Assert
        result.IsError.Should().BeFalse();
        collection.DeletedAtUtc.Should().Be(now);
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ShouldReturnError()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);

        var collection = Collection.Create("Test Collection").Value;

        // Act
        collection.Delete(mockDateTimeProvider); // Initial delete
        var result = collection.Delete(mockDateTimeProvider); // Attempt to delete again

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

        var collection = Collection.Create("Test Collection").Value;

        collection.Delete(mockDateTimeProvider); // Mark as deleted

        // Act
        var result = collection.RecoverDeleted();

        // Assert
        result.IsError.Should().BeFalse();
        collection.DeletedAtUtc.Should().BeNull();
    }

    [Fact]
    public void RecoverDeleted_WhenNotDeleted_ShouldReturnError()
    {
        // Arrange
        var mockDateTimeProvider = new TestDateTimeProvider(DateTime.UtcNow);

        var collection = Collection.Create("Test Collection").Value;

        // Act
        var result = collection.RecoverDeleted();

        // Assert
        result.IsError.Should().BeTrue();
        collection.DeletedAtUtc.Should().BeNull();
    }
}