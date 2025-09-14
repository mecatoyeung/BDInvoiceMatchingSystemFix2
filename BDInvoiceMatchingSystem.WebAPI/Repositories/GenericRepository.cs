using BDInvoiceMatchingSystem.WebAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;

namespace BDInvoiceMatchingSystem.WebAPI.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        ApplicationDbContext Database { get; }
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(long id);
        Task<IEnumerable<T>> GetByConditions(Expression<Func<T, bool>> whereConditions);
        Task AddAsync(T entity);
        void Add(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void DeleteByConditions(Expression<Func<T, bool>> whereConditions);
        Task<bool> Any(Expression<Func<T, bool>> whereConditions);
        Task<long> Count(Expression<Func<T, bool>> whereConditions);
        void ExecuteRawSql(string sql);
        Task<bool> AnyAsync(Expression<Func<T, bool>> entity, CancellationToken cancellationToken = default);
    }

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationDbContext Database { get
            {
                return _context;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync<T>();
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetByConditions(Expression<Func<T, bool>> whereConditions)
        {
            return await _context.Set<T>().Where(whereConditions).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void DeleteByConditions(Expression<Func<T, bool>> whereConditions)
        {
            var toBeRemoved = _context.Set<T>().Where(whereConditions);
            _context.Set<T>().RemoveRange(toBeRemoved);
        }

        public async Task<bool> Any(Expression<Func<T, bool>> whereConditions)
        {
            return await _context.Set<T>().AnyAsync(whereConditions);
        }

        public async Task<long> Count(Expression<Func<T, bool>> whereConditions)
        {
            return await _context.Set<T>().CountAsync(whereConditions);
        }

        public void ExecuteRawSql(string sql)
        {
            _context.Database.ExecuteSqlRaw(sql);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> entity, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().AnyAsync(entity, cancellationToken);
        }
    }
}
