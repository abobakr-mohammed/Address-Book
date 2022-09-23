using BusinessTier.Consts;
using DataTier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessTier.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> GetAll();
        Task<T> GetByIdAsync(int id);
        Task<T> GetByNameAsync(string name);

        //T Find(Expression<Func<T, bool>> match, int? take, int? skip, Expression<Func<T, object>> OrderBy = null, string orderByDirection = OrderBy.Ascending, string[] includes = null);
        Task<T> InsertAsync(T entity);
        T Insert(T entity);
        T Update(T entity);
        void Delete(T entity);
    }
}
