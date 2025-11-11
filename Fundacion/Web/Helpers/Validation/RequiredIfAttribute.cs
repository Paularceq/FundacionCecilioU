using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Web.Helpers.Validation
{
    public class RequiredIfAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _dependentProperty;
        private readonly string _targetValue;

        public RequiredIfAttribute(string dependentProperty, string targetValue)
        {
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dependentProperty = validationContext.ObjectType.GetProperty(_dependentProperty);
            if (dependentProperty == null)
            {
                return new ValidationResult($"La propiedad dependiente '{_dependentProperty}' no existe.");
            }

            var dependentValue = dependentProperty.GetValue(validationContext.ObjectInstance)?.ToString();

            if (dependentValue == _targetValue && value == null)
            {
                return new ValidationResult(ErrorMessage ?? $"El campo {validationContext.DisplayName} es obligatorio cuando {_dependentProperty} es '{_targetValue}'.");
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-requiredif", ErrorMessage);
            context.Attributes.Add("data-val-requiredif-dependentproperty", _dependentProperty);
            context.Attributes.Add("data-val-requiredif-targetvalue", _targetValue);
        }
    }
}