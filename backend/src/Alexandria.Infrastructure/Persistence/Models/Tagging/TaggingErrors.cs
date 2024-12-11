using ErrorOr;
namespace Alexandria.Infrastructure.Persistence.Models.Tagging;

public static class TaggingErrors
{
    public static readonly Error TaggingExists = Error.Conflict(
        $"{nameof(Tagging)}.{nameof(TaggingExists)}",
        "Tagging already exists between entity and tag.");
}