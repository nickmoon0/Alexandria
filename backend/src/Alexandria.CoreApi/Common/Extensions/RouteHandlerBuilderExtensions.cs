using Alexandria.Application.Common.Roles;

namespace Alexandria.CoreApi.Common.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder RequireAuthorization<TRole>(this RouteHandlerBuilder builder) where TRole : Role => 
        builder.RequireAuthorization(typeof(TRole).Name);
}