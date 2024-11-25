using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Application.Users.Commands.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Users;

public class CreateUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new user in the database")
        .RequireAuthorization(nameof(Admin));
    
    private record Request(string FirstName, string LastName, string? MiddleNames);
    private record Response(Guid Id);

    private static async Task<Results<CreatedAtRoute<Response>, BadRequest>> Handle(
        [FromBody] Request request,
        [FromServices] IMediator mediator)
    {
        var command = new CreateUserCommand(request.FirstName, request.LastName, request.MiddleNames);
        var result = await mediator.Send(command);

        if (result.IsError)
        {
            return TypedResults.BadRequest();
        }

        var response = new Response(result.Value.UserId);
        return TypedResults.CreatedAtRoute(response, nameof(GetUser), response);
    }
}