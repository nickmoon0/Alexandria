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
}