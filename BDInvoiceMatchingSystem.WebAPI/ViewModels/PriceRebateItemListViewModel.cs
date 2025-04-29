using System.ComponentModel.DataAnnotations;

namespace BDInvoiceMatchingSystem.WebAPI.ViewModels
{
    public class PriceRebateItemListViewModel
    {
        public long? ID { get; set; }

        public long? PriceRebateID { get; set; }

        public string? ExcelFilename { get; set; } = String.Empty;

        public bool AllItemsAreMatched { get; set; }

        public DateTime? UploadedDateTime { get; set; }

        public string? DocumentNo { get; set; }

        public string? StockCode { get; set; }

        public string? Description { get; set; }

        public string? UnitOfMeasure { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal SubtotalInUSD { get; set; }
        public decimal SubtotalInHKD { get; set; }
        public bool Matched { get; set; } = false;
        public long? MatchingID { get; set; }
    }
}
