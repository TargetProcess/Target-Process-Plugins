using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace Tp.Core
{
	public static class Run
	{
		private static readonly IExecutor Instance = ObjectFactory.GetInstance<IExecutor>();

		public static Tuple<T1, T2> InParallel<T1, T2>(Func<T1> f1, Func<T2> f2)
		{
			return Instance.Execute(f1, f2);
		}

		public static Tuple<T1, T2> InParallel<T1, T2>(Func<T1> f1, Func<T2> f2, IExecutor executor)
		{
			return executor.Execute(f1, f2);
		}

		public static Tuple<T, T> InParallel<T, U>(Func<U> f1, Func<U> f2, Func<U, T> f)
		{
			return InParallel(() => f(f1()), () => f(f2()));
		}

		public static IReadOnlyCollection<T> InParallel<T>(params Func<T>[] funcs)
		{
			return Instance.Execute(funcs);
		}
	}

	public interface IExecutor
	{
		Tuple<T1, T2> Execute<T1, T2>(Func<T1> f1, Func<T2> f2);
		IReadOnlyCollection<T> Execute<T>(params Func<T>[] funcs);
	}

	public class SequentialExecutor : IExecutor
	{
		public Tuple<T1, T2> Execute<T1, T2>(Func<T1> f1, Func<T2> f2)
		{
			return Tuple.Create(f1(), f2());
		}

		public IReadOnlyCollection<T> Execute<T>(params Func<T>[] funcs)
		{
			return funcs.Select(func => func()).ToArray();
		}
	}
}
