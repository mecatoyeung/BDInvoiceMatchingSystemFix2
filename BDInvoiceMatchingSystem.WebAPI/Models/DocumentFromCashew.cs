using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BDInvoiceMatchingSystem.WebAPI.Enums;

namespace BDInvoiceMatchingSystem.WebAPI.Models
{
    public class DocumentFromCashew
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        public EnumDocumentClass DocumentClass { get; set; }

        [Required]
        public EnumDocumentCreatedFrom DocumentCreatedFrom { get; set; }

        [Required]
        public string? PDFFilename { get; set; } = String.Empty;

        [Required]
        public string? CSVFilename { get; set; } = String.Empty;

        [Required]
        public string? DocumentNo { get; set; } = String.Empty;

        public DateTime? DocumentDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string? CustomerCode { get; set; } = String.Empty;

        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string? CustomerName { get; set; } = String.Empty;

        [Column(TypeName = "VARCHAR(1023)")]
        public string? CustomerAddress { get; set; } = String.Empty;

        public byte[]? PDFFile { get; set; }

        public byte[]? CSVFile { get; set; }

        [Required]
        public DateTime UploadedDateTime { get; set; }

        public ICollection<DocumentFromCashewItem> DocumentFromCashewItems { get; set; } = new List<DocumentFromCashewItem>();
    }
}
