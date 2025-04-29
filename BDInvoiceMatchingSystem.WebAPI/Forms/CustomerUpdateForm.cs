namespace BDInvoiceMatchingSystem.WebAPI.Forms
{
    public class CustomerUpdateForm
    {
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string[]? ApproximateNames { get; set; }
    }
}
