using ErrorOr;

namespace Alexandria.Domain.Common.ValueObjects.Name;

public class Name : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }
    public string? MiddleNames { get; }
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        yield return MiddleNames;
    }

    private Name(string firstName, string lastName, string? middleNames)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleNames = middleNames;
    }

    public static ErrorOr<Name> Create(string firstName, string lastName, string? middleNames = null)
    {
        const int firstNameMaxLength = 15;
        const int lastNameMaxLength = 15;
        const int middleNamesMaxLength = 30;
        var errors = new List<Error>();
        
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrEmpty(firstName) || 
            firstName.Length > firstNameMaxLength)
        {
            errors.Add(NameErrors.InvalidFirstName);
        }

        if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrEmpty(lastName) ||
            lastName.Length > lastNameMaxLength)
        {
            errors.Add(NameErrors.InvalidLastName);
        }


        if (string.IsNullOrEmpty(middleNames)) middleNames = null;
        if (!string.IsNullOrEmpty(middleNames) && middleNames.Length > middleNamesMaxLength)
        {
            errors.Add(NameErrors.InvalidMiddleNames);
        }

        if (errors.Count != 0)
        {
            return errors;
        }

        return new Name(firstName, lastName, middleNames);
    }
}