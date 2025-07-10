using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Reflection;

namespace Web.Helpers
{
    public static class EnumHelper
    {
        public static List<SelectListItem> ToSelectListItems<TEnum>(TEnum? selectedValue = null) where TEnum : struct, Enum
        {
            var items = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Text = GetDescription(e),
                    Value = Convert.ToInt32(e).ToString(),
                    Selected = selectedValue.HasValue && EqualityComparer<TEnum>.Default.Equals(e, selectedValue.Value)
                })
                .ToList();

            return items;
        }

        private static string GetDescription<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            FieldInfo field = typeof(TEnum).GetField(value.ToString());

            var attribute = field?
                .GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description ?? value.ToString();
        }
    }
}