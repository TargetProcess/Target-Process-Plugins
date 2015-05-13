using System;
using Tp.Core;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.Entities
{
	public interface ICustomFieldInfo
	{
		string Name { get; }
		FieldTypeEnum FieldType { get; }
		string EntityFieldName { get; }
		string Value { get; }
		int? EntityTypeID { get; }
		string CalculationModel { get; }
	}

	public static class CustomFieldInfoExtensions
	{
		private static readonly DateTime DateMinValue = DateTime.MinValue;
		private const string TextNaValue = "n/a ";

		public static object GetNaValue(this ICustomFieldInfo customField)
		{
			object naValue = null;

			switch (customField.FieldType)
			{
				case FieldTypeEnum.Text:
					naValue = TextNaValue;
					break;
				case FieldTypeEnum.CheckBox:
					naValue = false;
					break;
				case FieldTypeEnum.Date:
					naValue = DateMinValue;
					break;
				case FieldTypeEnum.Number:
					naValue = Double.NaN;
					break;
			}
			return naValue;
		}

		public static bool IsNa(this ICustomFieldInfo customFieldValue, object value)
		{
			switch (customFieldValue.FieldType)
			{
				case FieldTypeEnum.Number:
					return value.MaybeAs<decimal?>().Select(x => x == Decimal.MinValue).GetOrDefault();
				case FieldTypeEnum.Date:
					return value.MaybeAs<DateTime?>().Select(x => x == DateMinValue).GetOrDefault();
				case FieldTypeEnum.Text:
					return value.MaybeAs<string>().Select(x=>x == TextNaValue).GetOrDefault();
			}
			return false;
		}
	}
}