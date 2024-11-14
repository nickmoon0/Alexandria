using Alexandria.Api.Domain;

namespace Alexandria.UnitTests.DomainTests;

public class PersonTests
{
[Fact]
        public void Create_WithValidFirstAndLastName_ReturnsSuccessResult()
        {
            // Arrange
            const string firstName = "John";
            const string lastName = "Doe";

            // Act
            var result = Person.Create(firstName, lastName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(firstName, result.Value.FirstName);
            Assert.Equal(lastName, result.Value.LastName);
            Assert.Null(result.Value.MiddleNames);
            Assert.Null(result.Value.Description);
            Assert.Empty(result.Value.Documents);
        }

        [Fact]
        public void Create_WithValidFirstLastNameAndMiddleNames_ReturnsSuccessResult()
        {
            // Arrange
            const string firstName = "John";
            const string lastName = "Doe";
            const string middleNames = "Michael";

            // Act
            var result = Person.Create(firstName, lastName, middleNames);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(firstName, result.Value.FirstName);
            Assert.Equal(lastName, result.Value.LastName);
            Assert.Equal(middleNames, result.Value.MiddleNames);
            Assert.Null(result.Value.Description);
            Assert.Empty(result.Value.Documents);
        }

        [Fact]
        public void Create_WithValidFirstLastNameAndDescription_ReturnsSuccessResult()
        {
            // Arrange
            const string firstName = "Jane";
            const string lastName = "Doe";
            const string description = "Software Developer";

            // Act
            var result = Person.Create(firstName, lastName, description: description);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(firstName, result.Value.FirstName);
            Assert.Equal(lastName, result.Value.LastName);
            Assert.Null(result.Value.MiddleNames);
            Assert.Equal(description, result.Value.Description);
            Assert.Empty(result.Value.Documents);
        }

        [Fact]
        public void Create_WithAllValidParameters_ReturnsSuccessResult()
        {
            // Arrange
            const string firstName = "Jane";
            const string lastName = "Smith";
            const string middleNames = "Alexandra";
            const string description = "Data Analyst";

            // Act
            var result = Person.Create(firstName, lastName, middleNames, description);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(firstName, result.Value.FirstName);
            Assert.Equal(lastName, result.Value.LastName);
            Assert.Equal(middleNames, result.Value.MiddleNames);
            Assert.Equal(description, result.Value.Description);
            Assert.Empty(result.Value.Documents);
        }

        [Fact]
        public void Create_WithEmptyFirstName_ReturnsFailureResult()
        {
            // Arrange
            const string firstName = "";
            const string lastName = "Doe";

            // Act
            var result = Person.Create(firstName, lastName);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Create_WithEmptyLastName_ReturnsFailureResult()
        {
            // Arrange
            const string firstName = "John";
            const string lastName = "";

            // Act
            var result = Person.Create(firstName, lastName);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Create_WithNullFirstName_ReturnsFailureResult()
        {
            // Arrange
            string? firstName = null;
            const string lastName = "Doe";

            // Act
            var result = Person.Create(firstName!, lastName);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Create_WithNullLastName_ReturnsFailureResult()
        {
            // Arrange
            const string firstName = "John";
            string? lastName = null;

            // Act
            var result = Person.Create(firstName, lastName!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
        }
}