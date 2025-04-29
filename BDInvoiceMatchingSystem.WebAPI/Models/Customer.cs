using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using BDInvoiceMatchingSystem.WebAPI.Enums;

namespace BDInvoiceMatchingSystem.WebAPI.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? CustomerCode { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? CustomerName { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "NVARCHAR(1023)")]
        public string? CustomerAddress { get; set; } = String.Empty;

        public ICollection<CustomerApproximateMapping> CustomerApproximateMappings { get; set; } = new List<CustomerApproximateMapping>();
    }
}
