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

    public async Task<ErrorOr<Success>> AddAsync(Tag tag, CancellationToken cancellationToken)
    {
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
}