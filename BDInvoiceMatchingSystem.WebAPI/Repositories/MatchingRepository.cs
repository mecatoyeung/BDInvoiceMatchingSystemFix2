using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BDInvoiceMatchingSystem.WebAPI.Repositories
{
    public interface IMatchingRepository : IGenericRepository<Matching>
    {
        Task<IEnumerable<Matching>> GetByConditions(Expression<Func<Matching, bool>> whereConditions);
    }
    public class MatchingRepository : GenericRepository<Matching>, IMatchingRepository
    {
        public MatchingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public new async Task<IEnumerable<Matching>> GetByConditions(Expression<Func<Matching, bool>> whereConditions)
        {
            return await _context.Matchings.
                Include(p => p.PriceRebateItems).
                Include(p => p.DocumentFromCashewItems).
                Where(whereConditions).
                ToListAsync();
        }
    }
}
