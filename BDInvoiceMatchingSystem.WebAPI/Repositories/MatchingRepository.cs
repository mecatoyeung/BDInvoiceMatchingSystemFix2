using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BDInvoiceMatchingSystem.WebAPI.Repositories
{
    public interface IMatchingRepository : IGenericRepository<MatchingViewModel>
    {
        Task<IEnumerable<MatchingViewModel>> GetByConditions(Expression<Func<MatchingViewModel, bool>> whereConditions);
    }
    public class MatchingRepository : GenericRepository<MatchingViewModel>, IMatchingRepository
    {
        public MatchingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public new async Task<IEnumerable<MatchingViewModel>> GetByConditions(Expression<Func<MatchingViewModel, bool>> whereConditions)
        {
            return await _context.Matchings.
                Include(p => p.PriceRebateItems).
                Include(p => p.DocumentFromCashewItems).
                Where(whereConditions).
                ToListAsync();
        }
    }
}
