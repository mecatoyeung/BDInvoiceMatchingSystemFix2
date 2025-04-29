namespace BDInvoiceMatchingSystem.WebAPI.Forms
{
    public class DocumentFromCashewItemSearchByDocumentNosForm
    {
        public List<string> DocumentNos { get; set; } = new List<string>();
        public bool ShowAllUnmatched { get; set; } = false;
    }
}
