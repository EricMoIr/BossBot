using DataManager.XML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataManager.XML
{
    //T is the original type of objects
    //U is the model made off the original type
    public class XMLRepository<TOriginal> : IRepository<TOriginal> where TOriginal : class//, IModel> : IRepository<TOriginal> where IModel : IModel<TOriginal> where TOriginal : class
    {
        private List<TOriginal> elements;
        private string path;

        public XMLRepository(string path)
        {
            this.path = path;
            elements = XMLFileManager.ReadFile<List<TOriginal>>(path);
        }

        public void Delete(TOriginal entityToDelete)
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

        public bool ExistsPredicate(Predicate<TOriginal> predicate)
        {
            throw new NotImplementedException();
        }

        public TOriginal Find(Predicate<TOriginal> predicate)
        {
            elements = XMLFileManager.ReadFile<List<TOriginal>>(path);
            return elements.Find(predicate);
        }

        public IEnumerable<TOriginal> Get(
            Expression<Func<TOriginal, bool>> filter = null,
            Func<IQueryable<TOriginal>, IOrderedQueryable<TOriginal>> orderBy = null,
            string includeProperties = "")
        {
            return elements;
        }

        public TOriginal GetById(params object[] id)
        {
            throw new NotImplementedException();
        }

        public void Insert(TOriginal entityToAdd)
        {
            elements.Add(entityToAdd);
            XMLFileManager.WriteFile(path, elements);
        }

        public void Update(TOriginal entityToUpdate)
        {
            Delete(entityToUpdate);
            Insert(entityToUpdate);
        }
    }
}