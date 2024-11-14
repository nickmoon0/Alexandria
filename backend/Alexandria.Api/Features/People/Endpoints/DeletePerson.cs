using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Alexandria.Api.Features.People.Endpoints;

public class DeletePerson : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id:guid}", Handle)
        .WithSummary("Deletes a person with the given ID")
        .WithName(nameof(DeletePerson))
        .Produces<Ok>()
        .Produces<NotFound>();

    private static async Task<Results<Ok, NotFound>> Handle(
        [FromRoute] Guid id,
        [FromServices] AppDbContext context)
    {
        var person = await context.People.FindAsync(id);
        if (person is null) return TypedResults.NotFound();

        await context.People.ExecuteUpdateAsync(x => x
                .SetProperty(p => p.IsDeleted, true)
                .SetProperty(p => p.DeletedAtUtc, DateTime.UtcNow));
        await context.SaveChangesAsync();
        
        return TypedResults.Ok();
    }
}