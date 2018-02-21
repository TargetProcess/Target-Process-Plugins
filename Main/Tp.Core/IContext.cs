namespace Tp.Core.Interfaces
{
    public interface IReadOnlyContext
    {
        object GetValue(string name);
        bool Contains(string name);
    }

    public interface IContext : IReadOnlyContext
    {
        void SetValue(string name, object value);
        void Remove(string name);
        string Print();
    }
}
