using ErrorOr;

namespace Alexandria.FileApi.Common;

public static class ErrorMapping
{
    public static IResult MapErrorToHttpResponse(Error error) =>
        error.Type switch
        {
            ErrorType.Validation => Results.BadRequest(new { error.Description, error.Code }),
            ErrorType.NotFound => Results.NotFound(new { error.Description, error.Code }),
            ErrorType.Conflict => Results.Conflict(new { error.Description, error.Code }),
            ErrorType.Forbidden => Results.Forbid(),
            ErrorType.Unauthorized => Results.Unauthorized(),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
        };
}