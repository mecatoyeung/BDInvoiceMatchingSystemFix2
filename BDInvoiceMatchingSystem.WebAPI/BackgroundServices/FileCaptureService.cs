using AutoMapper;
using CsvHelper;

using BDInvoiceMatchingSystem.WebAPI.Models;
using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Helpers;
using System.Globalization;
using Microsoft.Extensions.Options;

namespace BDInvoiceMatchingSystem.WebAPI.BackgroundServices
{
    public class FileCaptureService : BackgroundService
    {
        private readonly MySettings _mySettings;
        private readonly ILogger<FileCaptureService> _logger;
        private readonly string _folderPath;
        private readonly IServiceProvider _serviceProvider;

        public FileCaptureService(
            IOptions<MySettings> mySettings,
            ILogger<FileCaptureService> logger,
            IServiceProvider serviceProvider)
        {
            _mySettings = mySettings.Value;
            _logger = logger;
            _folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CapturedFiles");
            Directory.CreateDirectory(_folderPath);
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_logger.LogInformation("File Capture Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("File Capture Service is running.");

                await CaptureFiles();

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            //_logger.LogInformation("File Capture Service is stopping.");
        }

        private async Task CaptureFiles()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                    var fileSources = await unitOfWork.FileSources.GetAllAsync();
                    foreach (var fileSource in fileSources)
                    {
                        if (!(fileSource.NextCaptureDateTime < DateTime.Now)) continue;

                        var filepaths = Directory.GetFiles(fileSource.FolderPath);
                        foreach (var filepath in filepaths)
                        {
                            // Process the file (e.g., move to another directory, log its details, etc.)
                            if (filepath.EndsWith("pdf") || filepath.EndsWith("PDF"))
                            {
                                var pdfFilepath = filepath;
                                WaitForFile(pdfFilepath);
                                var filenameWithoutExtension = Path.GetFileNameWithoutExtension(pdfFilepath);
                                var csvFilepath = Path.Combine(Path.GetDirectoryName(pdfFilepath), filenameWithoutExtension + ".csv");

                                try
                                {
                                    if (!File.Exists(csvFilepath))
                                    {
                                        continue;
                                    }
                                    WaitForFile(csvFilepath);

                                    using var reader = new StreamReader(csvFilepath);
                                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                                    var records = csv.GetRecords<dynamic>().ToList();

                                    var headerAlreadySet = false;
                                    var document = new DocumentFromCashew();
                                    for (var i = 0; i < records.Count(); i++)
                                    {
                                        var record = records[i];
                                        var dictionary = record as IDictionary<string, object>;
                                        if (!headerAlreadySet)
                                        {
                                            using var pdfMemoryStream = new MemoryStream();
                                            await using (var pdfFileStream = new FileStream(pdfFilepath, FileMode.Open, FileAccess.Read))
                                            {
                                                await pdfFileStream.CopyToAsync(pdfMemoryStream);
                                            }
                                            //document.PDFFile = pdfMemoryStream.ToArray();
                                            document.PDFFilename = filenameWithoutExtension + ".pdf";
                                            var newPdfFilePath = Path.Combine(_mySettings.DocumentFromCashewFolderPath, document.PDFFilename);
                                            FileHelper.CopyFile(pdfFilepath, newPdfFilePath);

                                            using var csvMemoryStream = new MemoryStream();
                                            await using (var csvFileStream = new FileStream(csvFilepath, FileMode.Open, FileAccess.Read))
                                            {
                                                await csvFileStream.CopyToAsync(csvMemoryStream);

                                            }
                                            //document.CSVFile = csvMemoryStream.ToArray();
                                            document.CSVFilename = filenameWithoutExtension + ".csv";
                                            var newCsvFilePath = Path.Combine(_mySettings.DocumentFromCashewFolderPath, document.CSVFilename);
                                            FileHelper.CopyFile(csvFilepath, newCsvFilePath);

                                            document.DocumentClass = fileSource.DocumentClass;
                                            document.DocumentCreatedFrom = Enums.EnumDocumentCreatedFrom.File;
                                            document.DocumentNo = dictionary?.FirstOrDefault(d => d.Key == fileSource.DocumentNoColName).Value.ToString();
                                            try
                                            {
                                                document.DocumentDate = DateTime.Parse(dictionary?.FirstOrDefault(d => d.Key == fileSource.DocumentDateColName).Value.ToString());
                                            }
                                            catch
                                            {
                                                document.DocumentDate = null;
                                            }
                                            try
                                            {
                                                document.DeliveryDate = DateTime.Parse(dictionary?.FirstOrDefault(d => d.Key == fileSource.DeliveryDateColName).Value.ToString());
                                            }
                                            catch
                                            {
                                                document.DeliveryDate = null;
                                            }
                                            document.CustomerCode = dictionary?.FirstOrDefault(d => d.Key == fileSource.CustomerCodeColName).Value.ToString();
                                            document.CustomerName = dictionary?.FirstOrDefault(d => d.Key == fileSource.CustomerNameColName).Value.ToString();
                                            document.CustomerAddress = dictionary?.FirstOrDefault(d => d.Key == fileSource.CustomerAddressColName).Value.ToString();

                                            document.UploadedDateTime = DateTime.Now;

                                            headerAlreadySet = true;
                                        }

                                        var documentItem = new DocumentFromCashewItem();
                                        documentItem.DocumentFromCashewID = document.ID;
                                        documentItem.StockCode = dictionary?.FirstOrDefault(d => d.Key == fileSource.StockCodeColName).Value?.ToString();
                                        documentItem.Description = dictionary?.FirstOrDefault(d => d.Key == fileSource.DescriptionColName).Value?.ToString();
                                        documentItem.LotNo = dictionary?.FirstOrDefault(d => d.Key == fileSource.LotNoColName).Value?.ToString();
                                        try
                                        {
                                            documentItem.Quantity = Int32.Parse(dictionary?.FirstOrDefault(d => d.Key == fileSource.QuantityColName).Value?.ToString());
                                        }
                                        catch (Exception ex)
                                        {
                                            documentItem.Quantity = 0;
                                        }
                                        documentItem.UnitOfMeasure = dictionary?.FirstOrDefault(d => d.Key == fileSource.UnitOfMeasureColName).Value?.ToString();
                                        try
                                        {
                                            documentItem.UnitPrice = Decimal.Parse(dictionary?.FirstOrDefault(d => d.Key == fileSource.UnitPriceColName).Value?.ToString());
                                        }
                                        catch (Exception ex)
                                        {
                                            documentItem.UnitPrice = 0;
                                        }
                                        try
                                        {
                                            documentItem.Subtotal = Decimal.Parse(dictionary?.FirstOrDefault(d => d.Key == fileSource.SubtotalColName).Value?.ToString());
                                        }
                                        catch (Exception ex)
                                        {
                                            documentItem.Subtotal = 0;
                                        }

                                        if (documentItem.Quantity == 0)
                                        {
                                            try
                                            {
                                                documentItem.Quantity = Convert.ToInt32(documentItem.Subtotal / documentItem.UnitPrice);
                                            }
                                            catch
                                            {

                                            }
                                        }

                                        //await unitOfWork.DocumentsFromCashewItems.AddAsync(documentItem);
                                        document.DocumentFromCashewItems.Add(documentItem);
                                    }
                                    await unitOfWork.DocumentsFromCashew.AddAsync(document);
                                    await unitOfWork.CompleteAsync();

                                    csv.Dispose();

                                    File.Delete(pdfFilepath);
                                    File.Delete(csvFilepath);
                                } catch
                                {
                                    _logger.LogError(String.Format("An error occurred while capturing files. PDF: {0}, CSV: {1}", pdfFilepath, csvFilepath));
                                    File.Delete(pdfFilepath);
                                    File.Delete(csvFilepath);
                                }
                            }

                            //_logger.LogInformation($"Captured file: {filepath}");
                        }
                        fileSource.NextCaptureDateTime = DateTime.Now.AddSeconds(fileSource.IntervalInSeconds);
                        unitOfWork.FileSources.Update(fileSource);
                        await unitOfWork.CompleteAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while capturing files.");
            }
        }

        public static void WaitForFile(string filePath)
        {
            while (true)
            {
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        // If we can open the file for exclusive access, it means writing is complete
                        break;
                    }
                }
                catch (IOException)
                {
                    // Wait for a short interval before retrying
                    Thread.Sleep(100);
                }
            }
        }
    }
}
