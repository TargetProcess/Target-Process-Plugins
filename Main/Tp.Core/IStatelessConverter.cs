using System;

namespace Tp.Core
{
	public interface IStatelessConverter<in TSrc, out TDst, in TState>
	{
		TDst Convert(TSrc val, TState state);
	}

	public interface IStatelessConverter<in TState>
	{
		object Convert(object val, Type valType, TState state);
	}
}
