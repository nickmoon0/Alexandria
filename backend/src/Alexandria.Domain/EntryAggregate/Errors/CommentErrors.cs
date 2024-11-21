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
}