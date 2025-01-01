namespace Alexandria.Api.Common.Extensions;

public static class StringExtensions
{
    public static TEnum ParseToEnumFlags<TEnum>(this string? delimitedValues, char delimiter = '|')
        where TEnum : struct, Enum
    {
        if (string.IsNullOrEmpty(delimitedValues))
        {
            return default;
        }

        var options = default(TEnum);
        var values = delimitedValues.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

        foreach (var value in values)
        {
            if (Enum.TryParse<TEnum>(value, true, out var parsedOption))
            {
                options = (TEnum)(object)((int)(object)options | (int)(object)parsedOption);
            }
        }

        return options;
    }
}