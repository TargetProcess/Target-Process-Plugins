using System;

namespace Tp.Integration.Messages.Entities
{
    public interface ICustomFieldConfigHolder
    {
        string CalculationModel { get; }
        string Units { get; }
    }

    public static class CustomFieldConfigHolderExtensions
    {
        public static bool IsCalculated(this ICustomFieldConfigHolder holder) =>
            !holder.CalculationModel.IsNullOrEmpty();
    }
}
