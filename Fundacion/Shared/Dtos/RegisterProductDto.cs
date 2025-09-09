using Shared.Enums;

namespace Shared.Dtos
{
    public class RegisterProductDto
    {
        public string Name { get; set; } = string.Empty;
        public UnitOfMeasure? UnitOfMeasure { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal InitialQuantity { get; set; }
        public string Comment { get; set; }
    }
}
