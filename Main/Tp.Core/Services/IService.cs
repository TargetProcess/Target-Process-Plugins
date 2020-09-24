namespace Tp.Core.Services
{
    public interface IService
    {
        void Start();
        void Stop();
        int? Priority { get; }
    }
}
