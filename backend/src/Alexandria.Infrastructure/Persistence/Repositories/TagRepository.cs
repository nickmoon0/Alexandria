using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Entities.Tag;
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
}