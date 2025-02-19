using Alexandria.Domain.EntryAggregate.Errors;
using Alexandria.Domain.Tests.TestUtils.Factories;
using Alexandria.Domain.Tests.TestUtils.Services;
using ErrorOr;
using FluentAssertions;

namespace Alexandria.Domain.Tests.EntryAggregateTests;

public class EntryTests
{
    [Fact]
        public void Create_ValidInput_ShouldReturnEntry()
        {
            // Act
            var result = EntryFactory.CreateEntry();

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public void Create_EmptyName_ShouldReturnInvalidNameError()
        {
            // Act
            var result = EntryFactory.CreateEntry(name: "  ");

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Should().Be(EntryErrors.InvalidName);
        }

        [Fact]
        public void Create_EmptyCreatedById_ShouldReturnInvalidIdError()
        {
            // Act
            var result = EntryFactory.CreateEntry(createdById: Guid.Empty);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Should().Be(EntryErrors.InvalidId);
        }

        [Fact]
        public void AddCharacter_ValidCharacter_ShouldAddCharacter()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var character = CharacterFactory.CreateCharacter().Value;

            // Act
            var result = entry.AddCharacter(character);

            // Assert
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public void AddCharacter_DuplicateCharacter_ShouldReturnCharacterIdAlreadyPresentError()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var character = CharacterFactory.CreateCharacter().Value;
            entry.AddCharacter(character);

            // Act
            var result = entry.AddCharacter(character);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Should().Be(DocumentErrors.CharacterIdAlreadyPresent);
        }

        [Fact]
        public void RemoveCharacter_ValidCharacter_ShouldRemoveCharacter()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var character = CharacterFactory.CreateCharacter().Value;
            entry.AddCharacter(character);

            // Act
            var result = entry.RemoveCharacter(character);

            // Assert
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public void RemoveCharacter_CharacterNotPresent_ShouldReturnCharacterIdNotPresentError()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var character = CharacterFactory.CreateCharacter().Value;

            // Act
            var result = entry.RemoveCharacter(character);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Should().Be(DocumentErrors.CharacterIdNotPresent);
        }

        [Fact]
        public void AddComment_ValidComment_ShouldAddComment()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var comment = CommentFactory.CreateComment().Value;

            // Act
            var result = entry.AddComment(comment);

            // Assert
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public void AddComment_DuplicateComment_ShouldReturnConflictError()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var comment = CommentFactory.CreateComment().Value;
            entry.AddComment(comment);

            // Act
            var result = entry.AddComment(comment);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Conflict);
        }

        [Fact]
        public void RemoveComment_ValidComment_ShouldRemoveComment()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var comment = CommentFactory.CreateComment().Value;
            entry.AddComment(comment);

            // Act
            var result = entry.RemoveComment(comment);

            // Assert
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public void RemoveComment_CommentNotPresent_ShouldReturnNotFoundError()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var comment = CommentFactory.CreateComment().Value;

            // Act
            var result = entry.RemoveComment(comment);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }
        
        [Fact]
        public void Delete_ValidState_ShouldMarkEntryAsDeleted()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var deletionTime = DateTime.UtcNow;
            var mockDateTimeProvider = new TestDateTimeProvider(deletionTime);

            // Act
            var result = entry.Delete(mockDateTimeProvider);

            // Assert
            result.IsError.Should().BeFalse();
            entry.DeletedAtUtc.Should().Be(deletionTime);
        }

        [Fact]
        public void Delete_AlreadyDeleted_ShouldReturnAlreadyDeletedError()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var mockDateTimeProvider = new TestDateTimeProvider();
            entry.Delete(mockDateTimeProvider);

            // Act
            var result = entry.Delete(mockDateTimeProvider);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Should().Be(EntryErrors.AlreadyDeleted);
        }

        [Fact]
        public void RecoverDeleted_ValidState_ShouldRecoverEntry()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var mockDateTimeProvider = new TestDateTimeProvider();
            entry.Delete(mockDateTimeProvider);

            // Act
            var result = entry.RecoverDeleted();

            // Assert
            result.IsError.Should().BeFalse();
            entry.DeletedAtUtc.Should().BeNull();
        }

        [Fact]
        public void RecoverDeleted_NotDeleted_ShouldReturnNotDeletedError()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;

            // Act
            var result = entry.RecoverDeleted();

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Should().Be(EntryErrors.NotDeleted);
        }
}