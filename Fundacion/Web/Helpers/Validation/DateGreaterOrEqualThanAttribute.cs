using System.ComponentModel.DataAnnotations;

namespace Web.Helpers.Validation;

public class DateGreaterOrEqualThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public DateGreaterOrEqualThanAttribute(string comparisonProperty, string errorMessage)
    {
        _comparisonProperty = comparisonProperty;
        ErrorMessage = errorMessage;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var currentValue = value as DateTime?;
        var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

        if (comparisonProperty == null)
        {
            return new ValidationResult($"La propiedad {_comparisonProperty} no existe.");
        }

        var comparisonValue = comparisonProperty.GetValue(validationContext.ObjectInstance) as DateTime?;

        if (currentValue.HasValue && comparisonValue.HasValue && currentValue < comparisonValue)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}