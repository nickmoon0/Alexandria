using ErrorOr;

namespace Alexandria.Domain.Common.Entities.Tag;

public static class TagErrors
{
    public static readonly Error InvalidName = Error.Validation(
        $"{nameof(Tag)}.InvalidName",
        "Specified name is invalid.");
    
    public static readonly Error TagNotFound = Error.NotFound(
        $"{nameof(Tag)}.TagNotFound",
        "Tag not found.");

    public static readonly Error TagAlreadyExists = Error.Conflict(
        $"{nameof(Tag)}.TagAlreadyExists",
        "Tag already exists.");
}