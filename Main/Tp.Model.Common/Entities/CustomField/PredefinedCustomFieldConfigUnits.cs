using System.Linq;

namespace Tp.Model.Common.Entities.CustomField
{
    public static class PredefinedCustomFieldConfigUnits
    {
        public static readonly string[] Money =
        {
            "€", "$", "£", "¥", "₤", "₹", "RUB", "BYR", "%"
        };

        public static readonly string[] Calculated = Money.Concat("‰", "days", "hours", "points").ToArray();
    }
}
