using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Features.People.Endpoints;

public class GetPerson : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id:guid}", Handle)
        .WithSummary("Retrieves a person with the given ID")
        .WithName(nameof(GetPerson))
        .Produces<Ok<Person>>();

    private static async Task<Results<Ok, NotFound>> Handle([FromRoute] Guid id)
    {
        return TypedResults.Ok();
    }

}