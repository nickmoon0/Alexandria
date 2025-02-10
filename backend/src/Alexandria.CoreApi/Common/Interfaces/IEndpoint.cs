namespace Alexandria.CoreApi.Common.Interfaces;

public interface IEndpoint
{
    public static abstract void Map(IEndpointRouteBuilder app);
}