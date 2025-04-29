namespace BDInvoiceMatchingSystem.WebAPI.Forms
{
    public class AccountChangePasswordForm
    {
        public string UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
