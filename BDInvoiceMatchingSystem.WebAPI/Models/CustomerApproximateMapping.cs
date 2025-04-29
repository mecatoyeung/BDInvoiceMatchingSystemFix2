using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using BDInvoiceMatchingSystem.WebAPI.Enums;

namespace BDInvoiceMatchingSystem.WebAPI.Models
{
    public class CustomerApproximateMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        public long CustomerID { get; set; }

        [ForeignKey(nameof(CustomerID))]
        public virtual Customer? Customer { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? ApproximateValue { get; set; }
    }
}
