using System.ComponentModel;

namespace Shared.Enums
{
    public enum ExpenseType
    {
        [Description("Otro")]
        Other,

        [Description("Alquiler")]
        Rent,

        [Description("Mantenimiento")]
        Maintenance,

        [Description("Suministros")]
        Supplies,

        [Description("Salarios")]
        Salaries
    }
}
