using ErrorOr;

namespace Alexandria.Domain.CharacterAggregate;

public static class CharacterErrors
{
    public static readonly Error DescriptionTooLong = Error.Validation(
        $"{nameof(Character)}.DescriptionTooLong",
        "Description cannot exceed maximum length.");
}