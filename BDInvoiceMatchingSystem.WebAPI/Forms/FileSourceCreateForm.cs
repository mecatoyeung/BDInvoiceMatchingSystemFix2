using BDInvoiceMatchingSystem.WebAPI.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BDInvoiceMatchingSystem.WebAPI.Forms
{
    public class FileSourceCreateForm
    {
        public int ID { get; set; }

        public string? FolderPath { get; set; }

        public EnumDocumentClass DocumentClass { get; set; }

        public string? DocumentNoColName { get; set; }

        public string? DocumentDateColName { get; set; }

        public string? DeliveryDateColName { get; set; }

        public string? CustomerCodeColName { get; set; }

        public string? CustomerNameColName { get; set; }

        public string? CustomerAddressColName { get; set; }

        public string? StockCodeColName { get; set; }

        public string? DescriptionColName { get; set; }

        public string? LotNoColName { get; set; }

        public string? QuantityColName { get; set; }

        public string? UnitOfMeasureColName { get; set; }
        public string? UnitPriceColName { get; set; }

        public string? SubtotalColName { get; set; }

        public string? FilenameColName { get; set; }

        public bool FirstRowIsHeader { get; set; }

        public int IntervalInSeconds { get; set; }
    }
}
