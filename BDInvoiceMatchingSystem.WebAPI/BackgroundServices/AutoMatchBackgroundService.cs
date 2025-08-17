// ReportGeneratorService.cs
using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Forms;
using BDInvoiceMatchingSystem.WebAPI.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BDInvoiceMatchingSystem.WebAPI.BackgroundServices
{
  public class AutoMatchBackgroundService : BackgroundService
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AutoMatchBackgroundService> _logger;

    public AutoMatchBackgroundService(
            ILogger<AutoMatchBackgroundService> logger,
            IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("Auto match background job started");
      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          // Do work here
          await AutoMatch();
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Auto match background job failed");
        }

        // wait before next run (use config in real apps)
        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
      }
    }

    public async Task AutoMatch()
    {

      try
      {
        using (var scope = _serviceProvider.CreateScope())
        {
          var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
          var priceRebates = await unitOfWork.PriceRebates.GetByConditions(pr => pr.Status == PriceRebateType.QUEUED);

          if (priceRebates.Count() == 0)
          {
            return;
          }

          var priceRebate = priceRebates.FirstOrDefault();

          priceRebate.Status = PriceRebateType.PROCCESSING;
          unitOfWork.PriceRebates.Update(priceRebate);
          await unitOfWork.CompleteAsync();

          var priceRebateItems = await unitOfWork.PriceRebateItems.GetByConditions(pbi =>
              pbi.PriceRebateID == priceRebate.ID &&
              !pbi.Matched);
          Console.WriteLine("Start of AutoMatch");
          var index = 0;
          foreach (var priceRebateItem in priceRebateItems)
          {
            Console.WriteLine($"AutoMatch Index: {index}");
            var correspondingDocumentFromCashewItems = (await unitOfWork.DocumentFromCashewItems.GetByConditions(ci =>
                ci.DocumentFromCashew.DocumentNo == priceRebateItem.DocumentNo &&
                ci.StockCode == priceRebateItem.StockCode &&
                ci.Quantity == priceRebateItem.Quantity &&
                !ci.Matched)).ToList();
            if (correspondingDocumentFromCashewItems.Count > 0)
            {
              priceRebateItem.Matched = true;
              unitOfWork.PriceRebateItems.Update(priceRebateItem);

              foreach (var documentFromCashewItem in correspondingDocumentFromCashewItems)
              {
                priceRebateItem.AutoMatchCompleted = true;
                documentFromCashewItem.Matched = true;
                unitOfWork.DocumentFromCashewItems.Update(documentFromCashewItem);
              }
            }
            else
            {
              priceRebateItem.AutoMatchCompleted = true;
              unitOfWork.PriceRebateItems.Update(priceRebateItem);
            }
            index++;
            if (index % 100 == 0)
            {
              priceRebate.AutoMatchProgress = index / priceRebateItems.Count();
              unitOfWork.PriceRebates.Update(priceRebate);
              await unitOfWork.CompleteAsync();
              break;
            }
          }

          var allMatched = true;
          if (await unitOfWork.PriceRebateItems.Any(i => !i.Matched))
          {
            allMatched = false;
          }

          priceRebate.AllItemsAreMatched = allMatched;
          if (allMatched)
          {
            priceRebate.Status = PriceRebateType.COMPLETED;
          }
          else
          {
            priceRebate.Status = PriceRebateType.QUEUED;
          }
          unitOfWork.PriceRebates.Update(priceRebate);

          await unitOfWork.CompleteAsync();
        }
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error in AutoMatch: {ex.Message}, StackTrace: {ex.StackTrace}");
      }
    }
  }
}