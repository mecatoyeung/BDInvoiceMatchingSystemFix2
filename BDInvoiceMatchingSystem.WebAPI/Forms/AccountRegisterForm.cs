using BDInvoiceMatchingSystem.WebAPI.Enums;

namespace BDInvoiceMatchingSystem.WebAPI.Forms
{
    public class AccountRegisterForm
    {
        public string Email { get; set; } = string.Empty;
        public EnumAccountType AccountType { get; set; } = EnumAccountType.User;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
    }
}
