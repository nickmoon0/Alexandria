using ErrorOr;

namespace Alexandria.Domain.DocumentAggregate;

public static class DocumentErrors
{
    public static readonly Error InvalidDocumentName = Error.Validation(
        $"{nameof(Document)}.InvalidDocumentName",
        "Document name must be between specified lengths");

    public static readonly Error EmptyDocumentData = Error.Validation(
        $"{nameof(Document)}.EmptyDocumentData",
        "Document data must not be empty");

    public static readonly Error InvalidOwnerId = Error.Validation(
        $"{nameof(Document)}.InvalidOwnerId",
        "OwnerId must be valid Guid");

    public static readonly Error InvalidCharacterId = Error.Validation(
        $"{nameof(Document)}.InvalidCharacterId",
        "CharacterId must be valid Guid");
    
    public static readonly Error CharacterIdAlreadyPresent = Error.Conflict(
        $"{nameof(Document)}.CharacterIdAlreadyPresent",
        "CharacterId is already present");
    
    public static readonly Error CharacterIdNotPresent = Error.Conflict(
        $"{nameof(Document)}.CharacterIdNotPresent",
        "CharacterId is not present");
}