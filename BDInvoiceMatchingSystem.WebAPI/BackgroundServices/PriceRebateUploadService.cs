using AutoMapper;
using CsvHelper;

using BDInvoiceMatchingSystem.WebAPI.Models;
using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Helpers;
using System.Globalization;
using Microsoft.Extensions.Options;
using ExcelDataReader;
using System;
using System.Diagnostics;
using static BDInvoiceMatchingSystem.WebAPI.Data.IUnitOfWork;

namespace BDInvoiceMatchingSystem.WebAPI.BackgroundServices
{
    public class PriceRebateUploadService : BackgroundService
    {
        private readonly MySettings _mySettings;
        private readonly ILogger<FileCaptureService> _logger;
        private readonly string _folderPath;
        private readonly IServiceProvider _serviceProvider;
        private static bool _running = false;

        public PriceRebateUploadService(
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
                
                await PriceRebateUpload();

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            //_logger.LogInformation("File Capture Service is stopping.");
        }

        private async Task PriceRebateUpload()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                    var priceRebate = (await unitOfWork.PriceRebates.GetByConditions(pr => pr.CurrentUploadRow != pr.TotalUploadRow)).FirstOrDefault();

                    if (priceRebate == null)
                    {
                        return;
                    }

                    var priceRebateSetting = new PriceRebateSetting();
                    try
                    {
                        priceRebateSetting = (await unitOfWork.PriceRebateSetting.GetAllAsync()).First();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    var excelFilePath = Path.Combine(_mySettings.PriceRebateFolderPath, priceRebate.Filename);

                    if (File.Exists(excelFilePath))
                    {
                        try
                        {
                            var priceRebateItems = new List<PriceRebateItem>();

                            byte[] fileBytes = File.ReadAllBytes(excelFilePath);
                            MemoryStream excelMemoryStream = new MemoryStream(fileBytes);

                            using (var reader = ExcelReaderFactory.CreateReader(excelMemoryStream))
                            {
                                var result = reader.AsDataSet();
                                var dataTable = result.Tables[0];

                                var documentNoColIndex = 0;
                                var stockCodeColIndex = 0;
                                var descriptionColIndex = 0;
                                var skuColIndex = 0;
                                var unitOfMeasureColIndex = 0;
                                var quantityColIndex = 0;
                                var unitPriceColIndex = 0;
                                var subtotalInUSDColIndex = 0;

                                for (int col = 0; col < dataTable.Columns.Count; col++)
                                {
                                    string columnName = dataTable.Columns[col].ColumnName;
                                    string columnValue = dataTable.Rows[0][col].ToString() ?? string.Empty;

                                    if (columnValue == priceRebateSetting.DocumentNoHeaderName)
                                    {
                                        documentNoColIndex = col;
                                    }
                                    if (columnValue == priceRebateSetting.StockCodeHeaderName)
                                    {
                                        stockCodeColIndex = col;
                                    }
                                    if (columnValue == priceRebateSetting.SKUHeaderName)
                                    {
                                        skuColIndex = col;
                                    }
                                    if (columnValue == priceRebateSetting.DescriptionHeaderName)
                                    {
                                        descriptionColIndex = col;
                                    }
                                    if (columnValue == priceRebateSetting.QuantityHeaderName)
                                    {
                                        quantityColIndex = col;
                                    }
                                    if (columnValue == priceRebateSetting.UnitPriceHeaderName)
                                    {
                                        unitPriceColIndex = col;
                                    }
                                    if (columnValue == priceRebateSetting.SubtotalInUSDHeaderName)
                                    {
                                        subtotalInUSDColIndex = col;
                                    }
                                }

                                var currentUploadPosition = priceRebate.CurrentUploadRow;
                                for (int row = currentUploadPosition + 1; row < dataTable.Rows.Count; row++) // Starting from 1 to skip header row
                                {
                                    var quantity = 0;
                                    var unitPrice = 0m;
                                    var subtotalInUSD = 0m;
                                    try
                                    {
                                        quantity = int.Parse(dataTable.Rows[row][quantityColIndex].ToString() ?? "0");
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    try
                                    {
                                        unitPrice = decimal.Parse(dataTable.Rows[row][unitPriceColIndex].ToString() ?? "0");
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    try
                                    {
                                        subtotalInUSD = decimal.Parse(dataTable.Rows[row][subtotalInUSDColIndex].ToString() ?? "0");
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    var item = new PriceRebateItem
                                    {
                                        PriceRebateID = priceRebate.ID,
                                        DocumentNo = dataTable.Rows[row][documentNoColIndex].ToString(),
                                        StockCode = dataTable.Rows[row][stockCodeColIndex].ToString(),
                                        SKU = dataTable.Rows[row][skuColIndex].ToString(),
                                        Description = dataTable.Rows[row][descriptionColIndex].ToString(),
                                        Quantity = quantity,
                                        UnitPrice = unitPrice,
                                        SubtotalInUSD = subtotalInUSD,
                                        SubtotalInHKD = subtotalInUSD * new decimal(7.8),
                                    };
                                    priceRebateItems.Add(item);

                                    currentUploadPosition++;

                                    if (currentUploadPosition % 100 == 0)
                                    {
                                        priceRebate.CurrentUploadRow = currentUploadPosition;
                                        unitOfWork.PriceRebates.Update(priceRebate);
                                        unitOfWork.PriceRebateItems.AddRange(priceRebateItems);
                                        priceRebateItems.Clear();
                                        await unitOfWork.CompleteAsync();
                                    }
                                }
                                priceRebate.CurrentUploadRow = currentUploadPosition;
                                unitOfWork.PriceRebates.Update(priceRebate);
                                unitOfWork.PriceRebateItems.AddRange(priceRebateItems);
                                priceRebateItems.Clear();
                                await unitOfWork.CompleteAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            priceRebate.UploadError = ex.ToString();
                            unitOfWork.PriceRebates.Update(priceRebate);
                            await unitOfWork.CompleteAsync();
                            return;
                        }
                    }
                    priceRebate.CurrentUploadRow = priceRebate.TotalUploadRow;
                    unitOfWork.PriceRebates.Update(priceRebate);
                    await unitOfWork.CompleteAsync();
                }
                _running = false;
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
