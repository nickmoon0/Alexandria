using ErrorOr;

namespace Alexandria.Domain.UserAggregate;

public static class UserErrors
{
    public static readonly Error AlreadyDeleted = Error.Validation(
        $"{nameof(User)}.AlreadyDeleted",
        "User is already deleted.");
    
    public static readonly Error NotDeleted = Error.Validation(
        $"{nameof(User)}.NotDeleted",
        "User is not deleted.");

    public static readonly Error NotFound = Error.NotFound(
        $"{nameof(User)}.NotFound",
        "User cannot be found.");
}