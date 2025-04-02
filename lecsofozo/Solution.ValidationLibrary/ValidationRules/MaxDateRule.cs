namespace Solution.ValidationLibrary.ValidationRules;

public class MaxDateRule<T>(DateTime maxDate) : IValidationRule<T>
{
    public string ValidationMessage { get; set; } = $"Value can't be larger then {maxDate}.";

    public bool Check(object value)
    {
        if (!DateTime.TryParse(value?.ToString(), out DateTime data))
        {
            return false;
        }

        return data <= maxDate;
    }
}
