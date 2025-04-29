using BDInvoiceMatchingSystem.WebAPI.Models;
using BDInvoiceMatchingSystem.WebAPI.Repositories;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BDInvoiceMatchingSystem.WebAPI.Data
{
    public interface IUnitOfWork : IDisposable
    {
        ICustomerRepository Customers { get; }
        ICustomerApproximateMappingRepository CustomerApproximateMappings { get; }
        IFileSourceRepository FileSources { get; }
        IDocumentFromCashewRepository DocumentsFromCashew {get; }
        IDocumentFromCashewItemRepository DocumentFromCashewItems { get; }
        IPriceRebateSettingRepository PriceRebateSetting { get; }
        IPriceRebateRepository PriceRebates { get; }
        IPriceRebateItemRepository PriceRebateItems { get; }
        IMatchingRepository Matchings { get; }
        EntityEntry Entry(object entity);
        Task<int> CompleteAsync();

        public class UnitOfWork
        {
        }
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICustomerRepository Customers { get; private set; }
        public ICustomerApproximateMappingRepository CustomerApproximateMappings { get; private set; }
        public IFileSourceRepository FileSources { get; private set; }
        public IDocumentFromCashewRepository DocumentsFromCashew { get; private set; }
        public IDocumentFromCashewItemRepository DocumentFromCashewItems { get; private set; }
        public IPriceRebateSettingRepository PriceRebateSetting { get; private set; }
        public IPriceRebateRepository PriceRebates { get; private set; }
        public IPriceRebateItemRepository PriceRebateItems { get; private set; }
        public IMatchingRepository Matchings { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Customers = new CustomerRepository(_context);
            CustomerApproximateMappings = new CustomerApproximateMappingRepository(_context);
            FileSources = new FileSourceRepository(_context);
            DocumentsFromCashew = new DocumentFromCashewRepository(_context);
            DocumentFromCashewItems = new DocumentFromCashewItemRepository(_context);
            PriceRebateSetting = new PriceRebateSettingRepository(_context);
            PriceRebates = new PriceRebateRepository(_context);
            PriceRebateItems = new PriceRebateItemRepository(_context);
            Matchings = new MatchingRepository(_context);
        }

        public EntityEntry Entry(object entity)
        {
            return _context.Entry(entity);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
