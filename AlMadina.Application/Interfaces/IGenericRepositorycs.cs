using System.Linq.Expressions;

namespace AlMadina.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(Guid id);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        IQueryable<T> GetAllQueryable();

        IQueryable<T> AsQueryable();

        Task<int> CountAsync();

        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate);
    }
}