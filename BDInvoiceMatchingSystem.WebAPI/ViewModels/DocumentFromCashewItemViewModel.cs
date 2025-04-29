using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BDInvoiceMatchingSystem.WebAPI.Enums;

namespace BDInvoiceMatchingSystem.WebAPI.ViewModels
{
    public class DocumentFromCashewItemViewModel
    {
        public long ID { get; set; }

        public long DocumentFromCashewID { get; set; }

        public virtual DocumentFromCashewViewModel? DocumentFromCashew { get; set; }

        public string StockCode { get; set; } = String.Empty;

        public string Description { get; set; } = String.Empty;

        public string? LotNo { get; set; } = String.Empty;
        public int Quantity { get; set; }

        public string UnitOfMeasure { get; set; } = String.Empty;
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public bool Matched { get; set; }
        public long? MatchingID { get; set; }
        public virtual MatchingViewModel? Matching { get; set; }
    }
}
