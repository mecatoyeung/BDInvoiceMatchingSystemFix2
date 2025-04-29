namespace BDInvoiceMatchingSystem.WebAPI.ViewModels
{
    public class CustomerViewModel
    {
        public int ID { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string[]? ApproximateNames { get; set; }
    }
}
