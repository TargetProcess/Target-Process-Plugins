namespace Tp.Core.Services
{
    public abstract class Service : IService
    {
        public abstract void Start();

        public abstract void Stop();

        public virtual int? Priority => null;
    }
}
