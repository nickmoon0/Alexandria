using ErrorOr;

namespace Alexandria.Domain.EntryAggregate.Errors;

public static class EntryErrors
{
    public static readonly Error InvalidName = Error.Validation(
        $"{nameof(Entry)}.InvalidName",
        "Name provided is invalid.");
    
    public static readonly Error InvalidId = Error.Validation(
        $"{nameof(Entry)}.InvalidId",
        "Id provided is invalid.");
    
    public static readonly Error AlreadyDeleted = Error.Validation(
        $"{nameof(Entry)}.AlreadyDeleted",
        "Entry is already deleted.");
    
    public static readonly Error NotDeleted = Error.Validation(
        $"{nameof(Entry)}.NotDeleted",
        "Entry is not deleted.");
    
    public static readonly Error NotFound = Error.NotFound(
        $"{nameof(Entry)}.NotFound)",
        "Entry is not found.");
    
    public static readonly Error DocumentNull = Error.Validation(
        $"{nameof(Entry)}.{nameof(DocumentNull)}",
        "Document was be null");
}