using BDInvoiceMatchingSystem.WebAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace BDInvoiceMatchingSystem.WebAPI.ViewModels
{
    public class DocumentFromCashewItemPaginationViewModel
    {
        public IEnumerable<DocumentFromCashewItemListViewModel> PageData { get; set; } = [];
        public int TotalRecords { get; set; }
    }
}
