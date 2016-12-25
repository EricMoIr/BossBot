using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataManager.XML
{
    public class XMLRepository<T> : IRepository<T> where T : class
    {
        private List<T> elements;
        private string path;

        public XMLRepository(string path)
        {
            this.path = path;
            elements = XMLFileManager.ReadFile<List<T>>(path);
        }

        public void Delete(T entityToDelete)
        {
            elements.Remove(entityToDelete);
            XMLFileManager.WriteFile(path, elements);
        }

        public void Delete(object id)
        {
            throw new NotImplementedException();
        }

        public bool Exists(params object[] id)
        {
            throw new NotImplementedException();
        }

        public bool ExistsPredicate(Predicate<T> predicate)
        {
            throw new NotImplementedException();
        }

        public T Find(Predicate<T> predicate)
        {
            elements = XMLFileManager.ReadFile<List<T>>(path);
            return elements.Find(predicate);
        }

        public IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            string includeProperties = "")
        {
            return elements;
        }

        public T GetById(params object[] id)
        {
            throw new NotImplementedException();
        }

        public void Insert(T entity)
        {
            elements.Add(entity);
            XMLFileManager.WriteFile(path, entity);
        }

        public void Update(T entityToUpdate)
        {
            Delete(entityToUpdate);
            Insert(entityToUpdate);
        }
    }
}