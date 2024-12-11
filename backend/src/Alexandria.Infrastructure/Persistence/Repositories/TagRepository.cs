using Alexandria.Application.Common.Interfaces;
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

    public async Task<ErrorOr<Success>> AddAsync(Tag tag, CancellationToken cancellationToken)
    {
        var existingTag = await _context.Tags.SingleOrDefaultAsync(x => x.Name == tag.Name, cancellationToken) != null;
        if (existingTag)
        {
            return TagErrors.TagAlreadyExists;    
        }
        
        await _context.Tags.AddAsync(tag, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success;
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

    public async Task<ErrorOr<IEnumerable<Tag>>> GetAllTags(CancellationToken cancellationToken)
    {
        var tags = await _context.Tags.ToListAsync(cancellationToken);
        return tags;
    }
}