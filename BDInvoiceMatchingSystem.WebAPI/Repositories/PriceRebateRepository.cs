using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Models;
using BDInvoiceMatchingSystem.WebAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace BDInvoiceMatchingSystem.WebAPI.Repositories
{
    public interface IPriceRebateRepository : IGenericRepository<PriceRebate>
    {
        Task<PriceRebate> GetByIdAsync(long id);
        Task<IEnumerable<PriceRebate>> GetListAsync();

        void RemoveMatchings(long id);
    }
    public class PriceRebateRepository : GenericRepository<PriceRebate>, IPriceRebateRepository
    {
        public PriceRebateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PriceRebate> GetByIdAsync(long id)
        {
            var rec = await _context.PriceRebates.Include(pr => pr.PriceRebateItems).FirstAsync(p => p.ID == id);
            rec.AllItemsAreMatched = rec.PriceRebateItems.All(i => i.Matched);
            return rec;
        }

        public async Task<IEnumerable<PriceRebate>> GetListAsync()
        {
            return await _context.PriceRebates.AsQueryable().Select(p => new PriceRebate
            {
                ID = p.ID, 
                ExcelFilename = p.ExcelFilename,
                Filename = p.Filename,
                CurrentUploadRow = p.CurrentUploadRow,
                TotalUploadRow = p.TotalUploadRow,
                UploadError = p.UploadError,
                AllItemsAreMatched = p.PriceRebateItems.All(i => i.Matched),
                UploadedDateTime = p.UploadedDateTime,
            }).ToListAsync();
        }

        public void RemoveMatchings(long id)
        {
            var sql = String.Format(@"update DocumentFromCashewItems d inner join Matchings m on m.ID = d.MatchingID 
                    inner join PriceRebateItems pi on pi.MatchingID = m.ID 
                    inner join PriceRebates p on p.ID = pi.PriceRebateID 
                    set d.MatchingID = null,  d.Matched = FALSE
                    where p.ID = {0};",
                id);
            _context.Database.ExecuteSqlRaw(
                String.Format(@"update DocumentFromCashewItems d inner join Matchings m on m.ID = d.MatchingID 
                    inner join PriceRebateItems pi on pi.MatchingID = m.ID 
                    inner join PriceRebates p on p.ID = pi.PriceRebateID 
                    set d.MatchingID = null,  d.Matched = FALSE
                    where p.ID = {0};",
                id));
            _context.Database.ExecuteSqlRaw(
                String.Format(@"update DocumentFromCashewItems d inner join Matchings m on m.ID = d.MatchingID 
                    inner join PriceRebateItems pi on pi.MatchingID = m.ID 
                    inner join PriceRebates p on p.ID = pi.PriceRebateID 
                    set pi.MatchingID = null,  pi.Matched = FALSE
                    where p.ID = {0};",
                id));
            /*_context.Database.ExecuteSqlRaw(
                String.Format(@"delete m
                    from matchings m 
                    inner join pricerebateitems pi on pi.MatchingID = m.ID 
                    where pi.PriceRebateID = {0};", id));*/
            _context.Database.ExecuteSqlRaw(
                String.Format(@"delete pr
                    from PriceRebates pr
                    where pr.ID = {0};",
                    id));
        }
    }
}
