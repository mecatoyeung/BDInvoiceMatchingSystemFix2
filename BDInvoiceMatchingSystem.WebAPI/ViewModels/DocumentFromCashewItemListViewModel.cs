using BDInvoiceMatchingSystem.WebAPI.Enums;
using BDInvoiceMatchingSystem.WebAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BDInvoiceMatchingSystem.WebAPI.ViewModels
{
    public class DocumentFromCashewItemListViewModel
    {
        public long? ID { get; set; }

        public long? DocumentFromCashewID { get; set; }

        public virtual DocumentFromCashewViewModel? DocumentFromCashew { get; set; }
        public string? StockCode { get; set; }
        public string? Description { get; set; }
        public string? LotNo { get; set; }
        public int Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Subtotal { get; set; }
        public bool Matched { get; set; }
        public long? MatchingID { get; set; }
    }
}
