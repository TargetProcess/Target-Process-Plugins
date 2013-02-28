using System;

namespace Tp.Core
{
	public interface IConverter<in TSrc, out TDst>
	{
		TDst Convert(TSrc val);
	}

	public interface IConverter
	{
		object Convert(object val, Type valType);
	}
}