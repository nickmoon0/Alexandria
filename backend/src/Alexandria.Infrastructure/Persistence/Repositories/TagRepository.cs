using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Entities.Tag;
using Alexandria.Infrastructure.Persistence.Models.Tagging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Alexandria.Infrastructure.Persistence.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _context;

    public TagRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Tag>> FindByIdAsync(Guid tagId, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags.FindAsync([tagId], cancellationToken);
        if (tag == null)
        {
            return TagErrors.TagNotFound;
        }

        return tag;
    }

    public async Task<ErrorOr<IReadOnlyList<Tag>>> GetEntityTags<TEntity>(TEntity entity, CancellationToken cancellationToken) 
        where TEntity : Entity
    {
        var entityName = typeof(TEntity).Name;

        var tagIds = await _context.Taggings
            .Where(x => x.EntityType == entityName && x.EntityId == entity.Id)
            .Select(x => x.TagId)
            .ToListAsync(cancellationToken);

        var tags = await _context.Tags
            .Where(tag => tagIds.Contains(tag.Id))
            .ToListAsync(cancellationToken);

        return tags;
    }

    public async Task<ErrorOr<Success>> AddEntityTag<TEntity>(TEntity entity, Tag tag, CancellationToken cancellationToken) 
        where TEntity : Entity
    {
        var entityName = typeof(TEntity).Name;
        var tagging = Tagging.Create(tag.Id, entityName, entity.Id);
        
        var existingTags = await _context.Taggings
            .Where(x => x.EntityId == entity.Id && x.TagId == tag.Id)
            .ToListAsync(cancellationToken);
        if (existingTags.Count != 0)
        {
            return TaggingErrors.TaggingExists;
        }
        
        await _context.Taggings.AddAsync(tagging, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}