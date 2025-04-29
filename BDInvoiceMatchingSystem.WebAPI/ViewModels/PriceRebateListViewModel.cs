using System.ComponentModel.DataAnnotations;

namespace BDInvoiceMatchingSystem.WebAPI.ViewModels
{
    public class PriceRebateListViewModel
    {
        public long ID { get; set; }

        public string? ExcelFilename { get; set; } = String.Empty;

        public string? Filename { get; set; } = String.Empty;

        public bool AllItemsAreMatched { get; set; }

        public int CurrentUploadRow { get; set; } = 0;

        public int TotalUploadRow { get; set; } = 0;

        public string UploadError { get; set; } = String.Empty;

        public DateTime UploadedDateTime { get; set; }
    }
}
