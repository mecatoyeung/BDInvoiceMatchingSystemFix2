using BDInvoiceMatchingSystem.WebAPI.Enums;

namespace BDInvoiceMatchingSystem.WebAPI.Forms
{
    public class DocumentFromCashewItemPaginationForm
    {
        public List<string> FieldName { get; set; } = new List<string>();
        public List<string> FieldType { get; set; } = new List<string>();
        public List<string> FilterType { get; set; } = new List<string>();
        public List<string> FilterValue { get; set; } = new List<string>();
    }
}
