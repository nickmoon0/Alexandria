namespace Alexandria.Api.Common.Models;

public class Error
{
    public required string Description { get; init; }
    public required ErrorType Type { get; init; }
}

public enum ErrorType
{
    EntityValidationFailed,
    RequestValidationFailed,
}