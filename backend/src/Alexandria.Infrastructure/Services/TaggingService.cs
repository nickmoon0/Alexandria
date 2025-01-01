using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Entities.Tag;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Infrastructure.Persistence;
using Alexandria.Infrastructure.Persistence.Models.Tagging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Infrastructure.Services;

public class TaggingService : ITaggingService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<TaggingService> _logger;
    private readonly AppDbContext _dbContext;

    public TaggingService(
        IDateTimeProvider dateTimeProvider,
        ILogger<TaggingService> logger,
        AppDbContext dbContext)
    {
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<ErrorOr<Success>> TagEntity<T>(T entity, Tag tag) where T : Entity
    {
        var typeName = typeof(T).Name;
        
        _logger.LogInformation("Adding Tag \'{TagName}\' to Entity Type \'{EntityType}\' with ID \'{ID}\'",
            tag.Name, typeName, entity.Id);

        var tagging = Tagging.Create(tag.Id, typeName, entity.Id);

        var existingTags = await _dbContext.Taggings
            .Where(t => t.TagId == tag.Id && t.EntityId == entity.Id)
            .ToListAsync();
        if (existingTags.Count != 0)
        {
            return TaggingErrors.TaggingExists;
        }
        
        await _dbContext.Taggings.AddAsync(tagging);
        await _dbContext.SaveChangesAsync();

        return Result.Success;
    }

    public async Task<ErrorOr<Deleted>> RemoveTag<T>(T entity, Tag tag) where T : Entity
    {
        var typeName = typeof(T).Name;
        var entityId = entity.Id;
        
        _logger.LogInformation("Removing Tag \'{TagName}\' from Entity Type \'{EntityType}\' with ID \'{ID}\'",
            tag.Name, typeName, entity.Id);
        
        var tagging = await _dbContext.Taggings
            .Where(t => t.EntityType == typeName && t.EntityId == entityId)
            .SingleOrDefaultAsync();

        if (tagging == null) return Error.NotFound();
        
        tagging.Delete(_dateTimeProvider);
        await _dbContext.SaveChangesAsync();
        
        return Result.Deleted;
    }
    
    public async Task<ErrorOr<IReadOnlyList<Tag>>> GetEntityTags<TEntity>(TEntity entity, CancellationToken cancellationToken) 
        where TEntity : Entity
    {
        var entityName = typeof(TEntity).Name;

        var tagIds = await _dbContext.Taggings
            .Where(x => x.EntityType == entityName && x.EntityId == entity.Id)
            .Select(x => x.TagId)
            .ToListAsync(cancellationToken);

        var tags = await _dbContext.Tags
            .Where(tag => tagIds.Contains(tag.Id))
            .ToListAsync(cancellationToken);

        return tags;
    }

    public async Task<IReadOnlyDictionary<Guid, IEnumerable<Tag>>> GetEntitiesTags<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken) where TEntity : Entity
    {
        var entityIds = entities.Select(e => e.Id).ToList();

        var taggings = await _dbContext.Taggings
            .Where(x => entityIds.Contains(x.EntityId))
            .ToListAsync(cancellationToken);

        var tagIds = taggings.Select(tagging => tagging.TagId).Distinct().ToList();

        var tags = await _dbContext.Tags
            .Where(tag => tagIds.Contains(tag.Id))
            .ToListAsync(cancellationToken);

        var tagDict = tags.ToDictionary(tag => tag.Id);

        var result = taggings
            .GroupBy(tagging => tagging.EntityId)
            .ToDictionary(
                group => group.Key,
                group => group.Select(tagging => tagDict[tagging.TagId]));

        return result;
    }

}