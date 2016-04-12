namespace Tp.Integration.Messages.Entities
{
	public interface ICustomFieldConfigHolder
	{
		string CalculationModel { get; }
		string Units { get; }
		bool? CalculationModelContainsCollections { get; }
		string DefaultValue { get; }
	}
}
