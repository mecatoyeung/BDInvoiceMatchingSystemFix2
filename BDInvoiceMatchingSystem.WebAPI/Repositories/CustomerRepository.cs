using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BDInvoiceMatchingSystem.WebAPI.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        // Additional methods specific to Product can be added here
    }
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }
        public new async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Set<Customer>().Include("CustomerApproximateMappings").FirstOrDefaultAsync(c => c.ID == id);
        }
    }
}
