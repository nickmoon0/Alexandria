using Alexandria.Domain.Common;
using FluentAssertions;

namespace Alexandria.Domain.Tests.CommonTests;

public class EntityBaseTests
{
    private class TestEntity : Entity
    {
        public TestEntity(Guid id) : base(id) { }
    }

    [Fact]
    public void Constructor_ShouldSetId_WhenIdIsProvided()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var entity = new TestEntity(id);

        // Assert
        entity.Id.Should().Be(id);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenIdsAreEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act
        var areEqual = entity1.Equals(entity2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenIdsAreDifferent()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act
        var areEqual = entity1.Equals(entity2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenOtherObjectIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);

        // Act
        var areEqual = entity.Equals(null);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenTypesAreDifferent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);
        var otherObject = new { Id = id }; // Different type

        // Act
        var areEqual = entity.Equals(otherObject);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValue_WhenCalledMultipleTimes()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);
        // Act
        var hashCode1 = entity.GetHashCode();
        var hashCode2 = entity.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValue_ForEntitiesWithSameId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act
        var hashCode1 = entity1.GetHashCode();
        var hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_ShouldReturnDifferentValues_ForEntitiesWithDifferentIds()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act
        var hashCode1 = entity1.GetHashCode();
        var hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }
}