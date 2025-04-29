using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Models;

namespace BDInvoiceMatchingSystem.WebAPI.Repositories
{
    public interface ICustomerApproximateMappingRepository : IGenericRepository<CustomerApproximateMapping>
    {
        // Additional methods specific to Product can be added here
    }
    public class CustomerApproximateMappingRepository : GenericRepository<CustomerApproximateMapping>, ICustomerApproximateMappingRepository
    {
        public CustomerApproximateMappingRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
