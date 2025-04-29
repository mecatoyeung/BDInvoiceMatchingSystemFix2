using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Models;

namespace BDInvoiceMatchingSystem.WebAPI.Repositories
{
    public interface IFileSourceRepository : IGenericRepository<FileSource>
    {
        // Additional methods specific to Product can be added here
    }
    public class FileSourceRepository : GenericRepository<FileSource>, IFileSourceRepository
    {
        public FileSourceRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
