namespace BDInvoiceMatchingSystem.WebAPI.Forms
{
    public class MatchWithDocumentFromCashewForm
    {
        public long[]? PriceRebateItems { get; set; } = [];
        public long[]? DocumentFromCashewItems { get; set; } = [];
    }
}
