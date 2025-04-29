using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BDInvoiceMatchingSystem.WebAPI.Models
{
    public class PriceRebateSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? DocumentNoHeaderName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? StockCodeHeaderName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? DescriptionHeaderName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? QuantityHeaderName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? UnitPriceHeaderName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? SubtotalInUSDHeaderName { get; set; }
    }
}
