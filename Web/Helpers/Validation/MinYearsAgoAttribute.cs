using System.ComponentModel.DataAnnotations;

namespace Web.Helpers.Validation;

public class MinYearsAgoAttribute : ValidationAttribute
{
    private readonly int _yearsAgo;

    public MinYearsAgoAttribute(int yearsAgo, string errorMessage)
    {
        _yearsAgo = yearsAgo;
        ErrorMessage = errorMessage;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime dateValue)
        {
            var minDate = DateTime.Now.AddYears(-_yearsAgo);
            if (dateValue < minDate)
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        return ValidationResult.Success;
    }
}