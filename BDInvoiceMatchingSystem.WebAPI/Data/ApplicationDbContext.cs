using BDInvoiceMatchingSystem.WebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

namespace BDInvoiceMatchingSystem.WebAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerApproximateMapping> CustomerApproximateMappings { get; set; }
        public DbSet<FileSource> FileSources { get; set; }
        public DbSet<DocumentFromCashew> DocumentsFromCashew { get; set; }
        public DbSet<DocumentFromCashewItem> DocumentFromCashewItems { get; set; }
        public DbSet<PriceRebateSetting> PriceRebateSetting { get; set; }
        public DbSet<PriceRebate> PriceRebates { get; set; }
        public DbSet<PriceRebateItem> PriceRebateItems { get; set; }
        public DbSet<Matching> Matchings { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.CustomerCode)
                .HasDatabaseName("IX_Customer_CustomerCode");
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.CustomerCode)
                .HasDatabaseName("IX_Customer_CustomerName");

            modelBuilder.Entity<DocumentFromCashew>()
                .HasIndex(c => c.DocumentClass)
                .HasDatabaseName("IX_DocumentFromCashew_DocumentClass");
            modelBuilder.Entity<DocumentFromCashew>()
                .HasIndex(c => c.DocumentCreatedFrom)
                .HasDatabaseName("IX_DocumentFromCashew_DocumentCreatedFrom");
            modelBuilder.Entity<DocumentFromCashew>()
                .HasIndex(c => c.DocumentNo)
                .HasDatabaseName("IX_DocumentFromCashew_DocumentNo");
            modelBuilder.Entity<DocumentFromCashew>()
                .HasIndex(c => c.CSVFilename)
                .HasDatabaseName("IX_DocumentFromCashew_CSVFilename");
            modelBuilder.Entity<DocumentFromCashew>()
                .HasIndex(c => c.CustomerCode)
                .HasDatabaseName("IX_DocumentFromCashew_CustomerCode");
            modelBuilder.Entity<DocumentFromCashew>()
                .HasIndex(c => c.CustomerName)
                .HasDatabaseName("IX_DocumentFromCashew_CustomerName");
            modelBuilder.Entity<DocumentFromCashew>()
                .HasIndex(c => c.DeliveryDate)
                .HasDatabaseName("IX_DocumentFromCashew_DeliveryDate");
            modelBuilder.Entity<DocumentFromCashew>()
                .HasIndex(c => c.DocumentDate)
                .HasDatabaseName("IX_DocumentFromCashew_DocumentDate");
            modelBuilder.Entity<DocumentFromCashew>()
                .HasIndex(c => c.UploadedDateTime)
                .HasDatabaseName("IX_DocumentFromCashew_UploadedDateTime");

            modelBuilder.Entity<DocumentFromCashewItem>()
                .HasIndex(c => c.StockCode)
                .HasDatabaseName("IX_DocumentFromCashewItem_StockCode"); 
            modelBuilder.Entity<DocumentFromCashewItem>()
                .HasIndex(c => c.LotNo)
                .HasDatabaseName("IX_DocumentFromCashewItem_LotNo");

            modelBuilder.Entity<PriceRebate>()
                .HasIndex(c => c.ExcelFilename)
                .HasDatabaseName("IX_PriceRebate_ExcelFilename");
            modelBuilder.Entity<PriceRebate>()
                .HasIndex(c => c.UploadedDateTime)
                .HasDatabaseName("IX_PriceRebate_UploadedDateTime");

            modelBuilder.Entity<PriceRebateItem>()
                .HasIndex(c => c.PriceRebateID)
                .HasDatabaseName("IX_PriceRebateItem_PriceRebateID");

            modelBuilder.Entity<Matching>()
                .HasMany(p => p.PriceRebateItems)
                .WithOne(p => p.Matching)
                .IsRequired(false)
                .HasForeignKey(p => p.MatchingID);

            modelBuilder.Entity<Matching>()
                .HasMany(p => p.DocumentFromCashewItems)
                .WithOne(p => p.Matching)
                .IsRequired(false)
                .HasForeignKey(p => p.MatchingID);

        }
    }
}
