using Alexandria.Common.Tests.Factories;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Tests.TestUtils.Factories;
using Alexandria.Domain.Tests.TestUtils.Services;
using Alexandria.Infrastructure.Persistence;
using Alexandria.Infrastructure.Services;
using Alexandria.Infrastructure.Tests.TestUtils.Builders;
using ErrorOr;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Alexandria.Infrastructure.Tests.ServicesTests;

public class TaggingServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly TaggingService _taggingService;

    public TaggingServiceTests()
    {
        var dateTimeProvider = new TestDateTimeProvider(DateTime.UtcNow);
        _dbContext = new DbContextBuilder<AppDbContext>().Build().Value;
        _taggingService = new TaggingService(dateTimeProvider, TestLoggerFactory.CreateLogger<TaggingService>(), _dbContext);
    }

    [Fact]
    public async Task TagEntity_WithValidEntityAndTag_ShouldAddTaggingToDatabase()
    {
        // Arrange
        var entry = EntryFactory.CreateEntry().Value;
        var tag = new TagBuilder()
            .WithName(Guid.NewGuid().ToString())
            .Build()
            .Value;

        await _dbContext.Entries.AddAsync(entry);
        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = await _taggingService.TagEntity(entry, tag);

        // Assert
        result.IsError.Should().BeFalse();
        _dbContext.Taggings.Should().ContainSingle(t => t.EntityId == entry.Id && t.TagId == tag.Id);
    }

    [Fact]
    public async Task RemoveTag_WithExistingTagging_ShouldRemoveTaggingFromDatabase()
    {
        // Arrange
        var user = UserFactory.CreateUser().Value;
        await _dbContext.Users.AddAsync(user);
        
        var character = CharacterFactory.CreateCharacter(createdById: user.Id).Value;
        await _dbContext.Characters.AddAsync(character);
        
        var tag = new TagBuilder()
            .WithName(Guid.NewGuid().ToString())
            .Build()
            .Value;
        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();
        
        // Act
        await _taggingService.TagEntity(character, tag);
        var result = await _taggingService.RemoveTag(character, tag);

        // Assert
        result.IsError.Should().BeFalse();
        _dbContext.Taggings.Should().NotContain(t => t.TagId == tag.Id && t.EntityId == character.Id);
    }

    [Fact]
    public async Task RemoveTag_WithNonExistingTagging_ShouldReturnNotFound()
    {
        // Arrange
        var entry = EntryFactory.CreateEntry().Value;
        var tag = new TagBuilder()
            .WithName(Guid.NewGuid().ToString())
            .Build()
            .Value;

        // Act
        var result = await _taggingService.RemoveTag(entry, tag);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Error.NotFound());
    }
    
    [Fact]
    public async Task TagEntity_WithDuplicateIndex_ShouldThrowException()
    {
        // Arrange
        var user = UserFactory.CreateUser().Value;
        await _dbContext.Users.AddAsync(user);
        
        var character = CharacterFactory.CreateCharacter(createdById: user.Id).Value;
        await _dbContext.Characters.AddAsync(character);
        
        var tag = new TagBuilder()
            .WithName(Guid.NewGuid().ToString())
            .Build()
            .Value;
        await _dbContext.Tags.AddAsync(tag);
        
        // Add the first tagging (this should succeed)
        await _taggingService.TagEntity(character, tag);

        // Act
        var action = async () => await _taggingService.TagEntity(character, tag);

        // Assert
        await action.Should().ThrowAsync<DbUpdateException>();
    }

}