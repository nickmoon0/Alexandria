using Alexandria.Api.Common;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Api.Users.DTOs;
using Alexandria.Application.Users.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Users;

public abstract class GetUser : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id:guid}", Handle)
        .WithSummary("Retrieves a new user")
        .WithName(nameof(GetUser))
        .RequireAuthorization(nameof(User));

    private record Response(UserDto User);
    
    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var query = new GetUserQuery { UserId = id };
        var result = await mediator.Send(query);

        if (result.IsError)
        {
            return TypedResults.NotFound();
        }

        var userResult = result.Value;
        var response = new Response(new UserDto
        {
            Id = userResult.Id,
            FirstName = userResult.FirstName,
            LastName = userResult.LastName,
            MiddleNames = userResult.MiddleNames,
        });
        
        return TypedResults.Ok(response);
    }
}