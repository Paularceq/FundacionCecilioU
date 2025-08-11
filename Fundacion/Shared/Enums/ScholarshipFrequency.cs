using System.ComponentModel;

namespace Shared.Enums
{
    public enum ScholarshipFrequency
    {
        [Description("Único")]
        OneTime = 1,

        [Description("Mensual")]
        Monthly = 2,

        [Description("Trimestral")]
        Quarterly = 3,

        [Description("Semestral")]
        Semiannual = 4,

        [Description("Anual")]
        Annual = 5
    }
}
