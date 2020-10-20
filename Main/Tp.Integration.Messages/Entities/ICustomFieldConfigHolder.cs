using System;
using Tp.Core.Annotations;

namespace Tp.Integration.Messages.Entities
{
    public interface IFormattableCustomFieldConfig
    {
        [CanBeNull] string FormatSpecifier { get; }
        [CanBeNull] FormatInfo FormatInfo { get; }
    }

    public interface ICustomFieldConfigHolder : IFormattableCustomFieldConfig
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
