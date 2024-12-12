using ErrorOr;

namespace Alexandria.Domain.EntryAggregate.Errors;

public static class DocumentErrors
{
    public static readonly Error InvalidDocumentName = Error.Validation(
        $"{nameof(Document)}.InvalidDocumentName",
        "Document name must be between specified lengths");

    public static readonly Error EmptyData = Error.Validation(
        $"{nameof(Document)}.EmptyData",
        "Data must not be empty");

    public static readonly Error InvalidEntryId = Error.Validation(
        $"{nameof(Document)}.InvalidEntryId",
        "EntryId cannot be null or empty");
    
    public static readonly Error InvalidUserId = Error.Validation(
        $"{nameof(Document)}.InvalidUserId",
        "User Id must be valid Guid");

    public static readonly Error InvalidCharacterId = Error.Validation(
        $"{nameof(Document)}.InvalidCharacterId",
        "CharacterId must be valid Guid");
    
    public static readonly Error InvalidFileExtension = Error.Validation(
        $"{nameof(Document)}.InvalidFileExtension",
        "File extension provided is not supported");
    
    public static readonly Error CharacterIdAlreadyPresent = Error.Conflict(
        $"{nameof(Document)}.CharacterIdAlreadyPresent",
        "CharacterId is already present");
    
    public static readonly Error CharacterIdNotPresent = Error.Conflict(
        $"{nameof(Document)}.CharacterIdNotPresent",
        "CharacterId is not present");
    
    public static readonly Error DocumentNull = Error.Validation(
        $"{nameof(Document)}.{nameof(DocumentNull)}",
        "Document was be null");
}