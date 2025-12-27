using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication10.DAO
{
    public interface IRepoDao<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}