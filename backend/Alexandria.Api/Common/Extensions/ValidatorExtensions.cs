using FluentValidation.Results;

namespace Alexandria.Api.Common.Extensions;

public static class ValidatorExtensions
{
    public static string JoinErrorMessages(this ValidationResult result)
    {
        var errors = result.Errors.Select(x => x.ErrorMessage);
        var errorString = string.Join(Environment.NewLine, errors);
        return errorString;
    }
}