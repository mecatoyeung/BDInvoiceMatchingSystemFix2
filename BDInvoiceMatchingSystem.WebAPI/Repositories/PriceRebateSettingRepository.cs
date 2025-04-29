using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Models;

namespace BDInvoiceMatchingSystem.WebAPI.Repositories
{
    public interface IPriceRebateSettingRepository : IGenericRepository<PriceRebateSetting>
    {
        // Additional methods specific to Product can be added here
    }
    public class PriceRebateSettingRepository : GenericRepository<PriceRebateSetting>, IPriceRebateSettingRepository
    {
        public PriceRebateSettingRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
