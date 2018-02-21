namespace Tp.Core
{
    public interface IConverter<in TSrc, out TDst>
    {
        TDst Convert(TSrc val);
    }
}
