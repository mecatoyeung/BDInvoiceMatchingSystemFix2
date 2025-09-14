using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using BDInvoiceMatchingSystem.WebAPI.Enums;

namespace BDInvoiceMatchingSystem.WebAPI.Models
{
    public class FileSource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        [Column(TypeName  = "NVARCHAR(1023)")]
        public string? FolderPath { get; set; }

        [Required] public EnumDocumentClass DocumentClass { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? DocumentNoColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? DocumentDateColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? DeliveryDateColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? CustomerCodeColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? CustomerNameColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? CustomerAddressColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? StockCodeColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? SKUColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? DescriptionColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? LotNoColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? QuantityColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? UnitOfMeasureColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? UnitPriceColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? SubtotalColName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? FilenameColName { get; set; }

        [Required]
        public bool FirstRowIsHeader { get; set; }

        [Required]
        public int IntervalInSeconds { get; set; }

        [Required]
        public DateTime NextCaptureDateTime { get; set; }
    }
}
