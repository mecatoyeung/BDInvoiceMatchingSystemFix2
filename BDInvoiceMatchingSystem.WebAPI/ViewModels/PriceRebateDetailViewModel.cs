using System.ComponentModel.DataAnnotations;
using BDInvoiceMatchingSystem.WebAPI.Models;

namespace BDInvoiceMatchingSystem.WebAPI.ViewModels
{
    public class PriceRebateDetailListViewModel
    {
        public PriceRebate PriceRebate { get; set; } = new PriceRebate();
        public long MatchedCount { get; set; }
        public long UnmatchedCount { get; set; }
    }
}
