using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataManager
{
    public interface IRepositoryGets<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Get(
                Expression<Func<TEntity, bool>> filter = null,
                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                string includeProperties = "");

        bool Exists(params object[] id);

        bool ExistsPredicate(Predicate<TEntity> predicate);

        TEntity GetById(params object[] id);

        TEntity Find(Predicate<TEntity> predicate);
    }
}