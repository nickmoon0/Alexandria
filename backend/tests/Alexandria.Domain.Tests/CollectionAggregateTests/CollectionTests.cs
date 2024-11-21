using Alexandria.Domain.CollectionAggregate;
using Alexandria.Domain.Tests.TestUtils.Factories;
using Alexandria.Domain.Tests.TestUtils.Services;
using ErrorOr;
using FluentAssertions;

namespace Alexandria.Domain.Tests.CollectionAggregateTests;

public class CollectionTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnCollection()
    {
        // Arrange
        var name = new string('A', 100); // Valid name with exactly 100 characters
        var createdById = Guid.NewGuid();
        var dateTimeProvider = new TestDateTimeProvider();
        
        // Act
        var result = Collection.Create(name, createdById, dateTimeProvider);

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
        var result = CollectionFactory.CreateCollection(name: name);

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
        var result = CollectionFactory.CreateCollection(name: name);

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
        var collection = CollectionFactory.CreateCollection(name: initialName).Value;

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
        var collection = CollectionFactory.CreateCollection(name: initialName).Value;

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
        var collection = CollectionFactory.CreateCollection().Value;
        var documentId = Guid.NewGuid();

        // Act
        var result = collection.AddEntry(documentId);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void AddDocument_WithEmptyDocumentId_ShouldReturnError()
    {
        // Arrange
        var collection = CollectionFactory.CreateCollection().Value;
        var documentId = Guid.Empty; // Invalid document ID

        // Act
        var result = collection.AddEntry(documentId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(CollectionErrors.InvalidEntryId);
    }

    [Fact]
    public void RemoveDocument_WithExistingDocumentId_ShouldRemoveFromCollection()
    {
        // Arrange
        var collection = CollectionFactory.CreateCollection().Value;
        var documentId = Guid.NewGuid();
        collection.AddEntry(documentId);

        // Act
        var result = collection.RemoveEntry(documentId);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void RemoveDocument_WithNonExistingDocumentId_ShouldReturnError()
    {
        // Arrange
        var collection = CollectionFactory.CreateCollection().Value;
        var documentId = Guid.NewGuid();

        // Act
        var result = collection.RemoveEntry(documentId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(CollectionErrors.EntryIdNotFound);
    }
    
        [Fact]
    public void Delete_NotAlreadyDeletedCollection_ShouldReturnDeleted()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);

        var collection = CollectionFactory.CreateCollection().Value;

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

        var collection = CollectionFactory.CreateCollection().Value;

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

        var collection = CollectionFactory.CreateCollection().Value;

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
        var collection = CollectionFactory.CreateCollection().Value;

        // Act
        var result = collection.RecoverDeleted();

        // Assert
        result.IsError.Should().BeTrue();
        collection.DeletedAtUtc.Should().BeNull();
    }
}