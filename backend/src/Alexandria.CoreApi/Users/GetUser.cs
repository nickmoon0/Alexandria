using Alexandria.Application.Common.Roles;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.Application.Users.Queries;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Users.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Users;

public abstract class GetUser : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id:guid}", Handle)
        .WithSummary("Retrieves a new user")
        .WithName(nameof(GetUser))
        .RequireAuthorization<User>();
    
    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var query = new GetUserQuery(id);
        var result = await mediator.Send(query);

        if (result.IsError)
        {
            return result.ToHttpResponse();
        }

        var userResult = result.Value;
        var response = new UserDto
        {
            Id = userResult.Id,
            FirstName = userResult.Name.FirstName,
            LastName = userResult.Name.LastName,
            MiddleNames = userResult.Name.MiddleNames,
        };
        
        return Results.Ok(response);
    }
}