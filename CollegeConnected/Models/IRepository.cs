using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollegeConnected.Models
{
    interface IRepository<T>
    {
        T Get(object id);
        void Attach(T entity);
        IQueryable<T> GetAll();
        void Insert(T entity);
        void Delete(T entity);
        void SubmitChanges();
    }
}
