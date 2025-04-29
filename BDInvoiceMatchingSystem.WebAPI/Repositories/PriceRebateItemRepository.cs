using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BDInvoiceMatchingSystem.WebAPI.Repositories
{
    public interface IPriceRebateItemRepository : IGenericRepository<PriceRebateItem>
    {
        
    }
    public class PriceRebateItemRepository : GenericRepository<PriceRebateItem>, IPriceRebateItemRepository
    {
        public PriceRebateItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public new async Task<IEnumerable<PriceRebateItem>> GetByConditions(Expression<Func<PriceRebateItem, bool>> whereConditions)
        {
            return await _context.PriceRebateItems.Where(whereConditions).AsQueryable().ToListAsync();
        }
    }
}
