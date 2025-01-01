using ErrorOr;

namespace Alexandria.Application.Common;

public class ApplicationErrors
{
    public static Error BadQueryError = Error.Validation(
        code: $"{nameof(ApplicationErrors)}.${nameof(BadQueryError)}",
        description: "Invalid query");
}