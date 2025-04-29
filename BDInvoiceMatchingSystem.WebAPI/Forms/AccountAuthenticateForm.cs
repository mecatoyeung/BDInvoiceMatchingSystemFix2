namespace BDInvoiceMatchingSystem.WebAPI.Forms
{
    public class AccountAuthenticateForm
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
    }
}
