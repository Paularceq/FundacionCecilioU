using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Web.Helpers.Validation
{
    public class MinCountAttribute : ValidationAttribute
    {
        private readonly int _min;

        public MinCountAttribute(int min = 1)
        {
            _min = min;
        }

        public override bool IsValid(object value)
        {
            if (value is IList list)
            {
                return list.Count >= _min;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.IsNullOrEmpty(ErrorMessage)
                ? $"El campo {name} debe tener al menos {_min} elemento(s)."
                : string.Format(ErrorMessage, name, _min);
        }
    }
}
