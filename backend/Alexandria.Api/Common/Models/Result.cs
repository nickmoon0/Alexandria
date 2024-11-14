namespace Alexandria.Api.Common.Models;

public class Result<TResult>
{
    public bool IsSuccess { get; init; }
    public TResult? Value { get; init; }
    public Error? Error { get; init; }
    
    private Result() { }

    public static Result<TResult> CreateSuccessResult(TResult value) => new()
    {
        IsSuccess = true,
        Value = value
    };

    public static Result<TResult> CreateErrorResult(Error error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}