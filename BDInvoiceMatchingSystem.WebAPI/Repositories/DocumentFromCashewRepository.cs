using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Models;

namespace BDInvoiceMatchingSystem.WebAPI.Repositories
{
    public interface IDocumentFromCashewRepository : IGenericRepository<DocumentFromCashew>
    {
        // Additional methods specific to Product can be added here
    }
    public class DocumentFromCashewRepository : GenericRepository<DocumentFromCashew>, IDocumentFromCashewRepository
    {
        public DocumentFromCashewRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
