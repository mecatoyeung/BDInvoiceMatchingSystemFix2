using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BDInvoiceMatchingSystem.WebAPI.Enums;
using BDInvoiceMatchingSystem.WebAPI.ViewModels;

namespace BDInvoiceMatchingSystem.WebAPI.Models
{
    public class DocumentFromCashewItemPagination
    {
        public IEnumerable<DocumentFromCashewItem> PageData { get; set; } = [];
        public int TotalRecords { get; set; }
    }
}
