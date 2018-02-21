using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.Core.Annotations;

namespace Tp.Core
{
    public static class Run
    {
        private static readonly IExecutor Instance = ObjectFactory.GetInstance<IExecutor>();

        [NotNull]
        public static Tuple<T1, T2> InParallel<T1, T2>([NotNull] Func<T1> f1, [NotNull] Func<T2> f2)
        {
            return Instance.Execute(f1, f2);
        }

        public static IReadOnlyCollection<T> InParallel<T>(params Func<T>[] funcs)
        {
            return Instance.Execute(funcs);
        }
    }

    public interface IExecutor
    {
        [NotNull]
        Tuple<T1, T2> Execute<T1, T2>([NotNull] Func<T1> f1, [NotNull] Func<T2> f2);
        IReadOnlyCollection<T> Execute<T>([NotNull] [ItemNotNull] params Func<T>[] funcs);
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
