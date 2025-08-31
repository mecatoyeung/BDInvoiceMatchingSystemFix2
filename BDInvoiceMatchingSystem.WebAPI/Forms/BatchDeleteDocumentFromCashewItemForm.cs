namespace BDInvoiceMatchingSystem.WebAPI.Forms
{
    public class BatchDeleteDocumentFromCashewItemForm
    {
        public List<long> Ids { get; set; } = new List<long>();
    }
}
