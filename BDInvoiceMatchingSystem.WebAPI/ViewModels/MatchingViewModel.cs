using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BDInvoiceMatchingSystem.WebAPI.ViewModels
{
    public class MatchingViewModel
    {
        public long ID { get; set; }
        public string? Name { get; set; }
        public ICollection<PriceRebateItemListViewModel> PriceRebateItems { get; set; } = new List<PriceRebateItemListViewModel>();
        public ICollection<DocumentFromCashewItemViewModel> DocumentFromCashewItems { get; set; } = new List<DocumentFromCashewItemViewModel>();
    }
}
