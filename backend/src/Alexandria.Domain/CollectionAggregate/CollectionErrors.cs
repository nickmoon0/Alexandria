using ErrorOr;

namespace Alexandria.Domain.CollectionAggregate;

public static class CollectionErrors
{
    public static readonly Error InvalidName = Error.Validation(
        $"{nameof(Collection)}.InvalidName",
        "Collection name must be between specified length");

    public static readonly Error InvalidUserId = Error.Validation(
        $"{nameof(Collection)}.InvalidUserId",
        "User Id must be valid Guid");
    
    public static readonly Error InvalidDocumentId = Error.Validation(
        $"{nameof(Collection)}.InvalidDocumentId",
        "Document Id must be a valid Guid");
    
    public static readonly Error DocumentIdNotFound = Error.Validation(
        $"{nameof(Collection)}.DocumentIdNotFound",
        "Document Id not found");
}