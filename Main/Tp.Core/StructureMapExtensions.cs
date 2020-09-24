using StructureMap.Configuration.DSL;

namespace Tp.Core
{
    public static class StructureMapExtensions
    {
        public static void ForwardTypeSafe<TFrom, TTo>(this Registry container)
            where TFrom : class, TTo
            where TTo : class
        {
            container.Forward<TFrom, TTo>();
        }
    }
}
