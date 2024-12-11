using ErrorOr;

namespace Alexandria.Domain.Common.Entities.Tag;

public static class TagErrors
{
    public static readonly Error InvalidName = Error.Validation(
        $"{nameof(TagErrors)}.InvalidName",
        "Specified name is invalid.");
}