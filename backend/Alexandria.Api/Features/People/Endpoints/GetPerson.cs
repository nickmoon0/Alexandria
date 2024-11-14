using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Domain;
using Alexandria.Api.Features.People.DTOs;
using Alexandria.Api.Infrastructure.Data;
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

    private static async Task<Results<Ok<GetPersonResponse>, NotFound>> Handle(
        [FromRoute] Guid id,
        [FromServices] AppDbContext context)
    {
        var person = await context.People.FindAsync(id);
        if (person is null) return TypedResults.NotFound();

        var response = new GetPersonResponse(new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName!,
            LastName = person.LastName!,
            MiddleNames = person.MiddleNames,
            Description = person.Description,
        });
        
        return TypedResults.Ok(response);
    }

    private record GetPersonResponse(PersonDto Person);
}