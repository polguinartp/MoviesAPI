using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetCollectionAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
        Task<T> GetByIdAsync(int id, string includeProperties = "");
        Task<T> AddAsync(T showtimeEntity);
        Task<T> UpdateAsync(T showtimeEntity);
        Task DeleteAsync(int id);
    }
}
