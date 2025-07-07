using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Reflection;

namespace Web.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            if (value == null) return string.Empty;

            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute != null ? attribute.Description : value.ToString();
        }

        public static List<SelectListItem> ToSelectList<TEnum>(this Enum @enum, object selectedValue = null) where TEnum : Enum
        {
            var type = typeof(TEnum);
            var values = Enum.GetValues(type).Cast<TEnum>();

            return values
                .Select(e => new SelectListItem
                {
                    Text = (e as Enum).GetDescription(),
                    Value = Convert.ToInt32(e).ToString(),
                    Selected = selectedValue != null && Convert.ToInt32(e).Equals(Convert.ToInt32(selectedValue))
                })
                .ToList();
        }
    }
}
