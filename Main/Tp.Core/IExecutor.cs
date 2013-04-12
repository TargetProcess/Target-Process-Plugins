using System;
using StructureMap;

namespace Tp.Core
{
	public static class Executor
	{
		private static readonly IExecutor Instance = ObjectFactory.GetInstance<IExecutor>();
		
		public static Tuple<T1, T2> Execute<T1, T2>(Func<T1> f1, Func<T2> f2)
		{
			return Instance.Execute(f1, f2);
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