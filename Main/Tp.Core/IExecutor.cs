using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StructureMap;
using Tp.Core.Annotations;

namespace Tp.Core
{
    public static class Run
    {
        private static readonly IExecutor Instance = ObjectFactory.GetInstance<IExecutor>();

        public static (T1, T2) InParallel<T1, T2>([NotNull] Func<T1> f1, [NotNull] Func<T2> f2)
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
        (T1 Result1, T2 Result2) Execute<T1, T2>([NotNull] Func<T1> f1, [NotNull] Func<T2> f2);
        (T1 Result1, T2 Result2, T3 Result3) Execute<T1, T2, T3>(Func<T1> f1, Func<T2> f2, Func<T3> f3);
        IReadOnlyCollection<T> Execute<T>([NotNull] [ItemNotNull] params Func<T>[] funcs);
    }

    public interface ICancellableExecutor
    {
        IReadOnlyCollection<T> ExecuteTillFailure<T>([NotNull] [ItemNotNull] Func<T>[] funcs, CancellationToken cancellationToken);
    }

    public class SequentialExecutor : IExecutor
    {
        public (T1 Result1, T2 Result2) Execute<T1, T2>(Func<T1> f1, Func<T2> f2)
        {
            return (f1(), f2());
        }

        public (T1 Result1, T2 Result2, T3 Result3) Execute<T1, T2, T3>(Func<T1> f1, Func<T2> f2, Func<T3> f3)
        {
            return (f1(), f2(), f3());
        }

        public IReadOnlyCollection<T> Execute<T>(params Func<T>[] funcs)
        {
            return funcs.Select(func => func()).ToArray();
        }
    }
}
