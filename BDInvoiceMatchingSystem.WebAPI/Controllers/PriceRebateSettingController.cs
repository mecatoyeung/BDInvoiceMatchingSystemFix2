using Microsoft.AspNetCore.Mvc;

using BDInvoiceMatchingSystem.WebAPI.Models;
using BDInvoiceMatchingSystem.WebAPI.Repositories;
using BDInvoiceMatchingSystem.WebAPI.Data;
using Microsoft.AspNetCore.Authorization;

namespace BDInvoiceMatchingSystem.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("PriceRebateSetting")]
    public class PriceRebateSettingController : ControllerBase
    {

        private readonly ILogger<PriceRebateController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PriceRebateSettingController(ILogger<PriceRebateController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetPriceRebateSetting()
        {
            var settingInDb = await InitializeSettingIfNeeded();

            return Ok(settingInDb);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePriceRebateSetting(PriceRebateSetting setting)
        {
            var settingInDb = await InitializeSettingIfNeeded();

            settingInDb.DocumentNoHeaderName = setting.DocumentNoHeaderName;
            settingInDb.StockCodeHeaderName = setting.StockCodeHeaderName;
            settingInDb.SKUHeaderName = setting.SKUHeaderName;
            settingInDb.DescriptionHeaderName = setting.DescriptionHeaderName;
            settingInDb.QuantityHeaderName = setting.QuantityHeaderName;
            settingInDb.UnitPriceHeaderName = setting.UnitPriceHeaderName;
            settingInDb.SubtotalInUSDHeaderName = setting.SubtotalInUSDHeaderName;

            _unitOfWork.PriceRebateSetting.Update(settingInDb);
            await _unitOfWork.CompleteAsync();
            return Ok(settingInDb);
        }

        private async Task<PriceRebateSetting> InitializeSettingIfNeeded()
        {
            var settingsInDb = await _unitOfWork.PriceRebateSetting.GetAllAsync();

            var settingInDb = new PriceRebateSetting();
            if (settingsInDb.Count() == 0)
            {
                settingInDb = new PriceRebateSetting
                {
                    DocumentNoHeaderName = "Inv #",
                    StockCodeHeaderName = "Item",
                    SKUHeaderName = "SKU",
                    DescriptionHeaderName = "Description",
                    QuantityHeaderName = "Sales Qty (BX)",
                    UnitPriceHeaderName = "End-Customer HK$/BX",
                    SubtotalInUSDHeaderName = "Amt to UI (USD)"
                };
                await _unitOfWork.PriceRebateSetting.AddAsync(settingInDb);
                await _unitOfWork.CompleteAsync();
            } else
            {
                settingInDb = settingsInDb.First();
            }
            return settingInDb;
        }
    }
}
