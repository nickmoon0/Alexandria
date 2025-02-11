namespace Alexandria.FileApi.Common;

public interface IEndpoint
{
    public static abstract void Map(IEndpointRouteBuilder app);
}