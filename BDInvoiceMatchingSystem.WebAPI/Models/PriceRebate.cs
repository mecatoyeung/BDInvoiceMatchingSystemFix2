using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BDInvoiceMatchingSystem.WebAPI.Enums;

namespace BDInvoiceMatchingSystem.WebAPI.Models
{
    public class PriceRebate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(255)")]
        public string? ExcelFilename { get; set; } = String.Empty;

        public string? Filename { get; set;} = String.Empty;

        public byte[]? ExcelFile { get; set; }

        [NotMapped]
        public bool AllItemsAreMatched { get; set; }

        [Required]
        public DateTime UploadedDateTime { get; set; }

        public int CurrentUploadRow { get; set; }

        public int TotalUploadRow { get; set; }

        public string UploadError { get; set; } = String.Empty;

        public ICollection<PriceRebateItem> PriceRebateItems { get; set; } = new List<PriceRebateItem>();
    }
}
