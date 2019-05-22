using System.Collections.Generic;

namespace Tp.Plugins.Toolkit.Repositories
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        T Find(int? id);
    }
}
