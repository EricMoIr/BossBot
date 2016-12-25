using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    public interface IRepository<TEntity> : IRepositoryGets<TEntity> where TEntity : class
    {
        void Insert(TEntity entity);

        void Update(TEntity entityToUpdate);

        void Delete(object id);

        void Delete(TEntity entityToDelete);
    }
}
