using ErrorOr;

namespace Alexandria.Domain.Common.ValueObjects.Name;

public static class NameErrors
{
    public static readonly Error InvalidFirstName = Error.Validation(
        $"{nameof(Name)}.InvalidFirstName",
        "First name must be between specified lengths.");

    public static readonly Error InvalidLastName = Error.Validation(
        $"{nameof(Name)}.InvalidLastName",
        "Last name must between specified lengths.");
    public static readonly Error InvalidMiddleNames = Error.Validation(
        $"{nameof(Name)}.InvalidMiddleNames",
        "Middle names must be between specified lengths.");
}