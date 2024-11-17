using ErrorOr;

namespace Alexandria.Domain.Common.ValueObjects.Name;

public class Name : ValueObject
{
    private readonly string _firstName;
    private readonly string _lastName;
    private readonly string? _middleNames;
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return _firstName;
        yield return _lastName;
        yield return _middleNames;
    }

    private Name(string firstName, string lastName, string? middleNames)
    {
        _firstName = firstName;
        _lastName = lastName;
        _middleNames = middleNames;
    }

    public static ErrorOr<Name> Create(string firstName, string lastName, string? middleNames)
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