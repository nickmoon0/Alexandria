using ErrorOr;

namespace Alexandria.Domain.CharacterAggregate;

public static class CharacterErrors
{
    public static readonly Error DescriptionTooLong = Error.Validation(
        $"{nameof(Character)}.DescriptionTooLong",
        "Description cannot exceed maximum length.");

    public static readonly Error InvalidUserId = Error.Validation(
        $"{nameof(Character)}.InvalidUserId",
        "User ID must be valid Guid");

    public static readonly Error CannotDeleteUsersCharacter = Error.Forbidden(
        $"{nameof(Character)}.CannotDeleteUsersCharacter",
        "Cannot delete a character that belongs to a user");
}