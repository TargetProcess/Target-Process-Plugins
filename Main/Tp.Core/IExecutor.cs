using System;
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

		public static Tuple<T, T> InParallel<T, U>(Func<U> f1, Func<U> f2, Func<U, T> f)
		{
			return InParallel(() =>f(f1()), () => f(f2()));
		}
	}
	
	
	public interface IExecutor
	{
		Tuple<T1, T2> Execute<T1, T2>(Func<T1> f1, Func<T2> f2);
	}



	public class SequentialExecutor : IExecutor
	{
		public Tuple<T1, T2> Execute<T1, T2>(Func<T1> f1, Func<T2> f2)
		{
			return Tuple.Create(f1(), f2());
		}
	}
}