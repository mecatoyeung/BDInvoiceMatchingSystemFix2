using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BDInvoiceMatchingSystem.WebAPI.Enums;

namespace BDInvoiceMatchingSystem.WebAPI.Models
{
    public class PriceRebateItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        public long PriceRebateID { get; set; }

        [ForeignKey(nameof(PriceRebateID))]
        public virtual PriceRebate? PriceRebate { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string? DocumentNo { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string? StockCode { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? Description { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal SubtotalInUSD { get; set; }

        [Required]
        public decimal SubtotalInHKD { get; set; }

        [Required]
        public bool Matched { get; set; } = false;

        [Required]
        public bool AutoMatchCompleted { get; set; } = false;

        public long? MatchingID { get; set; }
        public virtual MatchingViewModel? Matching { get; set; }
    }
}
