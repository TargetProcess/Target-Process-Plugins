namespace Tp.Core
{
    public interface IValueHolder<TValue>
    {
        TValue Get();
        void Set(TValue value);
        void Clear();
    }
}
