using ErrorOr;

namespace Alexandria.Domain.EntryAggregate.Errors;

public static class CommentErrors
{
    public static readonly Error EmptyData = Error.Validation(
        $"{nameof(Comment)}.EmptyData",
        "Data was empty.");
    
    public static readonly Error InvalidId = Error.Validation(
        $"{nameof(Comment)}.InvalidId",
        "Id was not valid.");

    public static readonly Error InvalidData = Error.Validation(
        $"{nameof(Comment)}.InvalidData",
        "Data was not valid");
    
    public static readonly Error AlreadyDeleted = Error.Validation(
        $"{nameof(Comment)}.AlreadyDeleted",
        "Comment was already deleted.");
}