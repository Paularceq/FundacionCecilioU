using System.ComponentModel;

namespace Shared.Enums
{
    public enum UnitOfMeasure
    {
        [Description("Unidad individual")]
        Unit = 1,

        [Description("Kilogramo")]
        Kilogram = 2,

        [Description("Gramo")]
        Gram = 3,

        [Description("Litro")]
        Liter = 4,

        [Description("Mililitro")]
        Milliliter = 5,

        [Description("Metro")]
        Meter = 6,

        [Description("Centímetro")]
        Centimeter = 7,

        [Description("Caja")]
        Box = 8,

        [Description("Paquete")]
        Package = 9,

        [Description("Docena")]
        Dozen = 10 
    }
}