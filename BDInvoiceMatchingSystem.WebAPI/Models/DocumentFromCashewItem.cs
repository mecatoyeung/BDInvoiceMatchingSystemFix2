using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BDInvoiceMatchingSystem.WebAPI.Enums;

namespace BDInvoiceMatchingSystem.WebAPI.Models
{
    [Table("DocumentFromCashewItems")]
    public class DocumentFromCashewItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        public long DocumentFromCashewID { get; set; }

        [ForeignKey(nameof(DocumentFromCashewID))]
        public virtual DocumentFromCashew? DocumentFromCashew { get; set; }

        [Column(TypeName = "VARCHAR(255)")]
        public string StockCode { get; set; } = String.Empty;

        [Column(TypeName = "NVARCHAR(255)")]
        public string Description { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR(255)")]
        public string? LotNo { get; set; } = String.Empty;
        public int Quantity { get; set; }

        [Column(TypeName = "VARCHAR(255)")]
        public string UnitOfMeasure { get; set; } = String.Empty;
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public bool Matched { get; set; }
        public long? MatchingID { get; set; }
        public virtual Matching? Matching { get; set; }
    }
}
