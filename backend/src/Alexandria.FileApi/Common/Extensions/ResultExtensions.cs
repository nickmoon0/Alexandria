using ErrorOr;

namespace Alexandria.FileApi.Common.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResponse<T>(this ErrorOr<T> result) =>
        result.Match(
            Results.Ok,
            errors => ErrorMapping.MapErrorToHttpResponse(errors.First())
        );
}