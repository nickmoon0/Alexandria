using ErrorOr;

namespace Alexandria.Domain.Common.ValueObjects.Name;

public static class NameErrors
{
    public readonly static Error InvalidFirstName = Error.Validation(
        "Name.InvalidFirstName",
        "First name must be between specified lengths.");

    public readonly static Error InvalidLastName = Error.Validation(
        "Name.InvalidLastName",
        "Last name must between specified lengths.");
    public readonly static Error InvalidMiddleNames = Error.Validation(
        "Name.InvalidMiddleNames",
        "Middle names must be between specified lengths.");
}