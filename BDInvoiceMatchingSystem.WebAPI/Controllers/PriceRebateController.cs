using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using ExcelDataReader;
using BDInvoiceMatchingSystem.WebAPI.Models;
using BDInvoiceMatchingSystem.WebAPI.Data;
using System.Reflection.Metadata;
using BDInvoiceMatchingSystem.WebAPI.Forms;
using BDInvoiceMatchingSystem.WebAPI.Repositories;
using AutoMapper;
using BDInvoiceMatchingSystem.WebAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;

using BDInvoiceMatchingSystem.WebAPI.Hubs;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
using BDInvoiceMatchingSystem.WebAPI.Helpers;
using MySqlX.XDevAPI.Common;

namespace BDInvoiceMatchingSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("PriceRebates")]
    public class PriceRebateController : ControllerBase
    {

        private readonly MySettings _mySettings;
        private readonly ILogger<PriceRebateController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<ProgressHub> _hubContext;

        public PriceRebateController(
            IOptions<MySettings> mySettings, 
            ILogger<PriceRebateController> logger, 
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _mySettings = mySettings.Value;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceRebate>>> GetPriceRebates()
        {
            var priceRebates = (await _unitOfWork.PriceRebates.GetListAsync()).ToList();
            var vms = _mapper.Map<List<PriceRebateListViewModel>>(priceRebates);
            return Ok(vms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PriceRebate>> GetPriceRebate(long id)
        {
            var priceRebate = await _unitOfWork.PriceRebates.GetByIdAsync(id);
            return Ok(priceRebate);
        }

        [HttpGet("{id}/Items")]
        public async Task<ActionResult<IEnumerable<PriceRebate>>> GetPriceRebateItems(long id)
        {
            var priceRebateItems = (await _unitOfWork.PriceRebateItems.GetByConditions(p => p.PriceRebateID == id)).ToList();
            var vms = _mapper.Map<List<PriceRebateItemListViewModel>>(priceRebateItems);
            return Ok(vms);
        }

        [HttpPost("UploadExcel")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadExcelFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var priceRebateSetting = new PriceRebateSetting();
            try
            {
                priceRebateSetting = (await _unitOfWork.PriceRebateSetting.GetAllAsync()).First();
            } catch (Exception ex) {
                return BadRequest("Please setup price rebate setting first.");
            }
            
            var priceRebate = new PriceRebate();

            //priceRebate.AllItemsAreMatched = false;
            priceRebate.ExcelFilename = file.FileName;
            priceRebate.UploadedDateTime = DateTime.Now;

            var filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            priceRebate.Filename = filename;
            var newExcelFilePath = Path.Combine(_mySettings.PriceRebateFolderPath, filename);

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var tempFilePath = Path.Combine(Path.GetTempPath(), file.FileName);

            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            FileHelper.CopyFile(tempFilePath, newExcelFilePath);

            using (var excelMemoryStream = new MemoryStream())
            {
                await file.CopyToAsync(excelMemoryStream);

                // priceRebate.ExcelFile = excelMemoryStream.ToArray();
                // priceRebate.ExcelFilename = file.FileName;

                excelMemoryStream.Position = 0;

                using (var reader = ExcelReaderFactory.CreateReader(excelMemoryStream))
                {
                    var result = reader.AsDataSet();
                    var dataTable = result.Tables[0];

                    priceRebate.TotalUploadRow = dataTable.Rows.Count;
                    priceRebate.CurrentUploadRow = 0;
                }
            }

            await _unitOfWork.PriceRebates.AddAsync(priceRebate);
            await _unitOfWork.CompleteAsync();

            return Ok(priceRebate);
        }

        [HttpGet("{id}/DownloadExcel")]
        public async Task<ActionResult<DocumentFromCashew>> GetExcel(long id)
        {
            var priceRebate = await _unitOfWork.PriceRebates.GetByIdAsync(id);

            var filePath = Path.Combine(_mySettings.PriceRebateFolderPath, priceRebate.Filename);

            if (System.IO.File.Exists(filePath))
            {
                var contentType = "application/octet-stream"; // You can specify the correct content type based on the file type

                return PhysicalFile(filePath, contentType, priceRebate.ExcelFilename);
            }
            else
            {
                return NotFound("File not found.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePriceRebate(long id)
        {
            var priceRebate = await _unitOfWork.PriceRebates.GetByIdAsync(id);
            if (priceRebate == null)
            {
                return NotFound();
            }

            /*var priceRebateItems = await _unitOfWork.PriceRebateItems.GetByConditions(i => i.PriceRebateID == id);
            var matchingIds = priceRebateItems.Select(i => i.MatchingID);

            var matchings = await _unitOfWork.Matchings.GetByConditions(m => matchingIds.Contains(m.ID));
            var documentFromCashewItemIds = matchings.SelectMany(m => m.DocumentFromCashewItems.Select(i => i.ID)).ToList();
            var documentFromCashewItems = await _unitOfWork.DocumentFromCashewItems.GetByConditions(i => documentFromCashewItemIds.Contains(i.ID));
            foreach (var documentFromCashewItem in documentFromCashewItems)
            {
                documentFromCashewItem.Matched = false;
                documentFromCashewItem.MatchingID = null;
                _unitOfWork.DocumentFromCashewItems.Update(documentFromCashewItem);
            }*/
            _unitOfWork.PriceRebates.RemoveMatchings(id);
            //_unitOfWork.Matchings.DeleteByConditions(i => matchingIds.Contains(i.ID));
            await _unitOfWork.CompleteAsync();

            _unitOfWork.Matchings.DeleteByConditions(i => i.PriceRebateItems.Count == 0 && i.DocumentFromCashewItems.Count == 0);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
