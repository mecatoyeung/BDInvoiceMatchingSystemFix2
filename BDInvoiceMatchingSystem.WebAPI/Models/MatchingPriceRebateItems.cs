using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace BDInvoiceMatchingSystem.WebAPI.Models
{
    public class MatchingPriceRebateItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string? Name { get; set; }
        public ICollection<PriceRebateItem> PriceRebateItems { get; set; } = new List<PriceRebateItem>();
        public ICollection<DocumentFromCashewItem> DocumentFromCashewItems { get; set; } = new List<DocumentFromCashewItem>();
    }
}
