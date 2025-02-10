using Alexandria.Application.Common.Constants;
using Alexandria.Application.Common.Interfaces;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Common.Roles;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Tokens;

public abstract class GetToken : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("", Handle)
        .WithSummary("Returns a token to upload/download files from the FileApi")
        .WithName(nameof(GetToken))
        .RequireAuthorization<User>();

    private static async Task<IResult> Handle([FromServices] ITokenService tokenService)
    {
        return Results.Ok(tokenService.GenerateToken(
            Guid.Parse("4cf512ab-7afb-4a4f-a41c-96c529d83559"), 
            30,
            [FilePermissions.Read]));
    }
}