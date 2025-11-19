using System.ComponentModel;

namespace Shared.Enums
{
    public enum MovementType
    {
        [Description("Ingreso")]
        Inbound = 1,

        [Description("Retiro")]
        Outbound = 2
    }
}
