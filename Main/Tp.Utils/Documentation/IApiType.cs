namespace Tp.Utils.Documentation
{
    /// <summary>
    /// A type marker indicating that a type implementing this interface should be reported as another type in API documentation.
    /// For example, custom collection class may be documented as IList or JsonContractResult{T} may be documented as just its underlying type T.
    /// 
    /// Using interfaces for documentation purpose seems ugly but there is no beatiful way to reference generic type parameters from type-level attributes.
    /// </summary>
    /// <typeparam name="TActual">Actual type to be documented.</typeparam>
    // ReSharper disable once UnusedTypeParameter
    public interface IApiType<TActual>
    {
    }
}
