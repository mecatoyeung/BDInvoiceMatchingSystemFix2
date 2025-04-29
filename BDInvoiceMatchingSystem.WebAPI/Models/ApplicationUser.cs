using Microsoft.AspNetCore.Identity;

using BDInvoiceMatchingSystem.WebAPI.Enums;

namespace BDInvoiceMatchingSystem.WebAPI.Models
{
    public class ApplicationUser: IdentityUser
    {
        public EnumAccountType AccountType { get; set; }
    }
}
