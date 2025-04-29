using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BDInvoiceMatchingSystem.WebAPI.Enums;

namespace BDInvoiceMatchingSystem.WebAPI.ViewModels
{
    public class DocumentFromCashewViewModel
    {
        public long ID { get; set; }

        public EnumDocumentClass DocumentClass { get; set; }

        public EnumDocumentCreatedFrom DocumentCreatedFrom { get; set; }

        public string? PDFFilename { get; set; } = String.Empty;

        public string? CSVFilename { get; set; } = String.Empty;

        public string? DocumentNo { get; set; } = String.Empty;

        public DateTime? DocumentDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public string? CustomerCode { get; set; } = String.Empty;

        public string? CustomerName { get; set; } = String.Empty;

        public string? CustomerAddress { get; set; } = String.Empty;

        public byte[]? PDFFile { get; set; }

        public byte[]? CSVFile { get; set; }

        public DateTime UploadedDateTime { get; set; }

        public ICollection<DocumentFromCashewItemViewModel> DocumentFromCashewItems { get; set; } = new List<DocumentFromCashewItemViewModel>();
    }
}
