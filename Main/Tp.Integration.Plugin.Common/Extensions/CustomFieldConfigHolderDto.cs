using Tp.Integration.Messages.Entities;

namespace Tp.Integration.Plugin.Common.Extensions
{
    public class CustomFieldConfigHolderDto : ICustomFieldConfigHolder
    {
        public string CalculationModel { get; set; }
        public string Units { get; set; }
        public bool? CalculationModelContainsCollections { get; set; }
        public string DefaultValue { get; set; }
    }
}
