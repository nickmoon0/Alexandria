using Alexandria.Domain.EntryAggregate.Errors;
using Alexandria.Domain.Tests.TestConstants;
using Alexandria.Domain.Tests.TestUtils.Factories;
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
        public void AddCharacter_ValidId_ShouldAddCharacter()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var characterId = Guid.NewGuid();

            // Act
            var result = entry.AddCharacter(characterId);

            // Assert
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public void AddCharacter_EmptyId_ShouldReturnInvalidCharacterIdError()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;

            // Act
            var result = entry.AddCharacter(Guid.Empty);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Should().Be(DocumentErrors.InvalidCharacterId);
        }

        [Fact]
        public void AddCharacter_DuplicateId_ShouldReturnCharacterIdAlreadyPresentError()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var characterId = Guid.NewGuid();
            entry.AddCharacter(characterId);

            // Act
            var result = entry.AddCharacter(characterId);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Should().Be(DocumentErrors.CharacterIdAlreadyPresent);
        }

        [Fact]
        public void RemoveCharacter_ValidId_ShouldRemoveCharacter()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var characterId = Guid.NewGuid();
            entry.AddCharacter(characterId);

            // Act
            var result = entry.RemoveCharacter(characterId);

            // Assert
            result.IsError.Should().BeFalse();
        }

        [Fact]
        public void RemoveCharacter_EmptyId_ShouldReturnInvalidCharacterIdError()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;

            // Act
            var result = entry.RemoveCharacter(Guid.Empty);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Should().Be(DocumentErrors.InvalidCharacterId);
        }

        [Fact]
        public void RemoveCharacter_IdNotPresent_ShouldReturnCharacterIdNotPresentError()
        {
            // Arrange
            var entry = EntryFactory.CreateEntry().Value;
            var characterId = Guid.NewGuid();

            // Act
            var result = entry.RemoveCharacter(characterId);

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
}