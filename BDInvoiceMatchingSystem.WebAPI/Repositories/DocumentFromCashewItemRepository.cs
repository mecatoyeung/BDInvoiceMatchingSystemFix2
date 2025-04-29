using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Forms;
using BDInvoiceMatchingSystem.WebAPI.Models;
using BDInvoiceMatchingSystem.WebAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace BDInvoiceMatchingSystem.WebAPI.Repositories
{
    public interface IDocumentFromCashewItemRepository : IGenericRepository<DocumentFromCashewItem>
    {
        Task<DocumentFromCashewItemPagination> GetPagination(DocumentFromCashewItemPaginationForm form);
        Task<IEnumerable<DocumentFromCashewItem>> GetByDocumentNosAsync(List<string> documentNos);
        Task<IEnumerable<DocumentFromCashewItem>> GetByConditions(Expression<Func<DocumentFromCashewItem, bool>> whereConditions);
    }
    public class DocumentFromCashewItemRepository : GenericRepository<DocumentFromCashewItem>, IDocumentFromCashewItemRepository
    {
        public DocumentFromCashewItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<DocumentFromCashewItemPagination> GetPagination(DocumentFromCashewItemPaginationForm form)
        {
            var queryable = _context.DocumentFromCashewItems.AsQueryable();
            
            var totalRecords = queryable.Count();
            for (var i=0; i<form.FieldName.Count; i++)
            {
                var fieldName = form.FieldName[i];
                var filterValue = form.FilterValue[i];
                if (fieldName == "documentFromCashew.pdfFilename")
                {
                    queryable = queryable.Where(i => i.DocumentFromCashew.PDFFilename.Contains(filterValue));
                }
                if (fieldName == "documentFromCashew.csvFilename")
                {
                    queryable = queryable.Where(i => i.DocumentFromCashew.CSVFilename.Contains(filterValue));
                }
                if (fieldName == "documentFromCashew.customerCode")
                {
                    queryable = queryable.Where(i => i.DocumentFromCashew.CustomerCode.Contains(filterValue));
                }
                if (fieldName == "documentFromCashew.customerName")
                {
                    queryable = queryable.Where(i => i.DocumentFromCashew.CustomerName.Contains(filterValue));
                }
                if (fieldName == "documentFromCashew.customerAddress")
                {
                    queryable = queryable.Where(i => i.DocumentFromCashew.CustomerAddress.Contains(filterValue));
                }
                if (fieldName == "documentFromCashew.documentNo")
                {
                    queryable = queryable.Where(i => i.DocumentFromCashew.DocumentNo.Contains(filterValue));
                }
                if (fieldName == "stockCode")
                {
                    queryable = queryable.Where(i => i.StockCode.Contains(filterValue));
                }
                if (fieldName == "description")
                {
                    queryable = queryable.Where(i => i.Description.Contains(filterValue));
                }
                if (fieldName == "lotNo")
                {
                    queryable = queryable.Where(i => i.LotNo.Contains(filterValue));
                }
            }
            var pagedData = queryable.Select(d => new DocumentFromCashewItem
            {
                ID = d.ID,
                DocumentFromCashew = new DocumentFromCashew
                {
                    ID = d.DocumentFromCashew.ID,
                    DocumentNo = d.DocumentFromCashew.DocumentNo,
                    DocumentClass = d.DocumentFromCashew.DocumentClass,
                    DocumentCreatedFrom = d.DocumentFromCashew.DocumentCreatedFrom,
                    DocumentDate = d.DocumentFromCashew.DocumentDate,
                    CustomerCode = d.DocumentFromCashew.CustomerCode,
                    CustomerName = d.DocumentFromCashew.CustomerName,
                    CustomerAddress = d.DocumentFromCashew.CustomerAddress,
                    DeliveryDate = d.DocumentFromCashew.DeliveryDate,
                    PDFFilename = d.DocumentFromCashew.PDFFilename,
                    CSVFilename = d.DocumentFromCashew.CSVFilename,
                    UploadedDateTime = d.DocumentFromCashew.UploadedDateTime
                },
                StockCode = d.StockCode,
                Description = d.Description,
                LotNo = d.LotNo,
                Quantity = d.Quantity,
                UnitOfMeasure = d.UnitOfMeasure,
                UnitPrice = d.UnitPrice,
                Subtotal = d.Subtotal,
                Matched = d.Matched,
                MatchingID = d.MatchingID
            }).AsEnumerable();

            return new DocumentFromCashewItemPagination
            {
                PageData = pagedData,
                TotalRecords = totalRecords,
            };
        }

        public async Task<IEnumerable<DocumentFromCashewItem>> GetByDocumentNosAsync(List<string> documentNos)
        {
            return await _context.DocumentFromCashewItems.
                Include(p => p.DocumentFromCashew).
                Select(d => new DocumentFromCashewItem
                {
                    ID = d.ID,
                    DocumentFromCashew = new DocumentFromCashew
                    {
                        ID = d.DocumentFromCashew.ID,
                        DocumentNo = d.DocumentFromCashew.DocumentNo,
                        DocumentClass = d.DocumentFromCashew.DocumentClass,
                        DocumentCreatedFrom = d.DocumentFromCashew.DocumentCreatedFrom,
                        DocumentDate = d.DocumentFromCashew.DocumentDate,
                        CustomerCode = d.DocumentFromCashew.CustomerCode,
                        CustomerName = d.DocumentFromCashew.CustomerName,
                        CustomerAddress = d.DocumentFromCashew.CustomerAddress,
                        DeliveryDate = d.DocumentFromCashew.DeliveryDate,
                        PDFFilename = d.DocumentFromCashew.PDFFilename,
                        CSVFilename = d.DocumentFromCashew.CSVFilename,
                        UploadedDateTime = d.DocumentFromCashew.UploadedDateTime
                    },
                    StockCode = d.StockCode,
                    Description = d.Description,
                    LotNo = d.LotNo,
                    Quantity = d.Quantity,
                    UnitOfMeasure = d.UnitOfMeasure,
                    UnitPrice = d.UnitPrice,
                    Subtotal = d.Subtotal,
                    Matched = d.Matched,
                    MatchingID = d.MatchingID
                }).
                Where(p => p.DocumentFromCashew.DocumentNo == null ? false : documentNos.Contains(p.DocumentFromCashew.DocumentNo)).
                AsQueryable().ToListAsync();
        }

        public new async Task<IEnumerable<DocumentFromCashewItem>> GetByConditions(Expression<Func<DocumentFromCashewItem, bool>> whereConditions)
        {
            return await _context.DocumentFromCashewItems.
                Include(p => p.DocumentFromCashew).Where(whereConditions).
                ToListAsync();
        }
    }
}
