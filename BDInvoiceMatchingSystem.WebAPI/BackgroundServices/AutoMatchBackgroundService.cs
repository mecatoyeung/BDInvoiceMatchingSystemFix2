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

    public static List<(List<PriceRebateItem> A, List<DocumentFromCashewItem> B)> PairSubsets(List<PriceRebateItem> arrayA, List<DocumentFromCashewItem> arrayB)
    {
      var usedA = new HashSet<long>();
      var usedB = new HashSet<long>();
      var result = new List<(List<PriceRebateItem>, List<DocumentFromCashewItem>)>();

      var subsetsA = GetSubsets(arrayA);
      var subsetsB = GetSubsets(arrayB);

      foreach (var subA in subsetsA.OrderBy(s => s.Count))
      {
        if (subA.Any(i => usedA.Contains(i.ID))) continue;
        int sumA = subA.Sum(a => a.Quantity);

        foreach (var subB in subsetsB.OrderBy(s => s.Count))
        {
          if (subB.Any(i => usedB.Contains(i.ID))) continue;
          if (subB.Sum(b => b.Quantity) == sumA)
          {
            result.Add((subA, subB));
            foreach (var i in subA) usedA.Add(i.ID);
            foreach (var i in subB) usedB.Add(i.ID);
            break;
          }
        }
      }

      return result;
    }

    private static List<List<PriceRebateItem>> GetSubsets(List<PriceRebateItem> priceRebateItems)
    {
      var subsets = new List<List<PriceRebateItem>>();
      int n = priceRebateItems.Count;
      for (int i = 1; i < (1 << n); i++)
      {
        var subset = new List<PriceRebateItem>();
        for (int j = 0; j < n; j++)
        {
          if ((i & (1 << j)) != 0)
            subset.Add(priceRebateItems[j]);
        }
        subsets.Add(subset);
      }
      return subsets;
    }

    private static List<List<DocumentFromCashewItem>> GetSubsets(List<DocumentFromCashewItem> documentFromCashewItems)
    {
      var subsets = new List<List<DocumentFromCashewItem>>();
      int n = documentFromCashewItems.Count;
      for (int i = 1; i < (1 << n); i++)
      {
        var subset = new List<DocumentFromCashewItem>();
        for (int j = 0; j < n; j++)
        {
          if ((i & (1 << j)) != 0)
            subset.Add(documentFromCashewItems[j]);
        }
        subsets.Add(subset);
      }
      return subsets;
    }

    public async Task AutoMatch()
    {

      /*try
      {*/
      using (var scope = _serviceProvider.CreateScope())
      {
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var priceRebates = await unitOfWork.PriceRebates.GetByConditions(pr => pr.Status == PriceRebateStatus.QUEUED);

        if (priceRebates.Count() == 0)
        {
          return;
        }

        var priceRebate = priceRebates.FirstOrDefault();

        priceRebate.Status = PriceRebateStatus.PROCCESSING;
        unitOfWork.PriceRebates.Update(priceRebate);
        await unitOfWork.CompleteAsync();

        unitOfWork.PriceRebateItems.ExecuteRawSql(
                String.Format(@"UPDATE PriceRebateItems SET AutoMatchCompleted = 0 WHERE PriceRebateID = {0}",
                priceRebate.ID));

        var priceRebateItems = await unitOfWork.PriceRebateItems.GetByConditions(pbi =>
            pbi.PriceRebateID == priceRebate.ID &&
            !pbi.Matched &&
            !pbi.AutoMatchCompleted);

        var documentFromCashewItems = (await unitOfWork.DocumentFromCashewItems.GetByConditions(di =>
            !di.Matched)).ToList();

        var groupsOfDocumentNoAndStockCode = priceRebateItems
          .Select(m => new { m.DocumentNo, m.StockCode })
          .Distinct().ToList();

        foreach (var groupOfDocumentNoAndStockCode in groupsOfDocumentNoAndStockCode)
        {
          var filteredPriceRebateItems = priceRebateItems.Where(pri => pri.DocumentNo == groupOfDocumentNoAndStockCode.DocumentNo &&
            pri.StockCode == groupOfDocumentNoAndStockCode.StockCode).ToList();

          var filteredDocumentFromCashewItems = documentFromCashewItems.Where(di =>
            di.DocumentFromCashew.DocumentNo == groupOfDocumentNoAndStockCode.DocumentNo &&
            di.StockCode == groupOfDocumentNoAndStockCode.StockCode &&
              !di.Matched).ToList();

          foreach (var filteredPriceRebateItem in filteredPriceRebateItems)
          {
            filteredPriceRebateItem.AutoMatchCompleted = true;
            unitOfWork.PriceRebateItems.Update(filteredPriceRebateItem);
          }

          await unitOfWork.CompleteAsync();

          if (filteredDocumentFromCashewItems.Count() == 0)
          {
            continue;
          }

          var pairedSubsets = PairSubsets(filteredPriceRebateItems, filteredDocumentFromCashewItems);

          foreach (var pairedSubset in pairedSubsets)
          {
            var priceRebateItemsInSubset = pairedSubset.A;
            var documentFromCashewItemsInSubset = pairedSubset.B;

            var matching = new Matching();
            matching.Name = "Matching";

            await unitOfWork.Matchings.AddAsync(matching);
            await unitOfWork.CompleteAsync();

            foreach (var priceRebateItem in priceRebateItemsInSubset)
            {
              priceRebateItem.Matched = true;
              priceRebateItem.MatchingID = matching.ID;
              unitOfWork.PriceRebateItems.Update(priceRebateItem);
            }

            foreach (var documentFromCashewItem in documentFromCashewItemsInSubset)
            {
              documentFromCashewItem.Matched = true;
              documentFromCashewItem.MatchingID = matching.ID;
              unitOfWork.DocumentFromCashewItems.Update(documentFromCashewItem);
            }

            await unitOfWork.CompleteAsync();
          }

          /*foreach (var filteredPriceRebateItem in filteredPriceRebateItems)
          {
            filteredPriceRebateItem.AutoMatchCompleted = true;
            unitOfWork.PriceRebateItems.Update(filteredPriceRebateItem);
          }*/
          unitOfWork.PriceRebateItems.ExecuteRawSql(String.Format("UPDATE PriceRebateItems SET AutoMatchCompleted = TRUE WHERE DocumentNo = '{0}' AND StockCode = '{1}'",
            groupOfDocumentNoAndStockCode.DocumentNo,
            groupOfDocumentNoAndStockCode.StockCode));

          await unitOfWork.CompleteAsync();
        }

        await unitOfWork.CompleteAsync();

        var allMatched = true;
        if (await unitOfWork.PriceRebateItems.Any(i => i.PriceRebateID == priceRebate.ID && !i.AutoMatchCompleted))
        {
          allMatched = false;
        }

        priceRebate.AllItemsAreMatched = allMatched;
        if (allMatched)
        {
          priceRebate.Status = PriceRebateStatus.COMPLETED;
        }
        unitOfWork.PriceRebates.Update(priceRebate);

        await unitOfWork.CompleteAsync();
      }
      /*}
      catch (Exception ex)
      {
        _logger.LogError($"Error in AutoMatch: {ex.Message}, StackTrace: {ex.StackTrace}");
      }*/
    }
  }
}