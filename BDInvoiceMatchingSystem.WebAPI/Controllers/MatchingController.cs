using BDInvoiceMatchingSystem.WebAPI.Forms;
using BDInvoiceMatchingSystem.WebAPI.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

using BDInvoiceMatchingSystem.WebAPI.Data;
using BDInvoiceMatchingSystem.WebAPI.Models;
using BDInvoiceMatchingSystem.WebAPI.Repositories;
using BDInvoiceMatchingSystem.WebAPI.Enums;
using AutoMapper;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;
using static BDInvoiceMatchingSystem.WebAPI.Data.IUnitOfWork;
using Microsoft.AspNetCore.Authorization;

namespace BDInvoiceMatchingSystem.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Matching")]
    public class MatchingController : ControllerBase
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IMapper _mapper;
        public MatchingController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/matchings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchingViewModel>>> GetMatchings()
        {
            return Ok(await _unitOfWork.FileSources.GetAllAsync());
        }

        // GET: api/matchings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchingViewModel>> GetMatching(long id)
        {
            var matching = await _unitOfWork.Matchings.GetByIdAsync(id);

            if (matching == null)
            {
                return NotFound();
            }

            return Ok(matching);
        }

        [HttpGet("Multiple")]
        public async Task<ActionResult<MatchingViewModel>> GetMatchings([FromQuery] List<long> documentFromCashewIds)
        {
            var matchings = await _unitOfWork.Matchings.GetByConditions(m => documentFromCashewIds.Contains(m.ID));

            if (matchings == null)
            {
                return NotFound();
            }

            return Ok(matchings);
        }

        [HttpPost("Match")]
        public async Task<ActionResult<DocumentFromCashew>> Match([FromBody] MatchWithDocumentFromCashewForm form)
        {
            if (form.PriceRebateItems == null || form.PriceRebateItems.Count() == 0)
            {
                return BadRequest(new { Message = "Please provide price rebate items" });
            }
            if (form.DocumentFromCashewItems == null || form.DocumentFromCashewItems.Count() == 0)
            {
                return BadRequest(new { Message = "Please provide documents from cashew" });
            }

            var priceRebateItems = (await _unitOfWork.PriceRebateItems.GetByConditions(m => form.PriceRebateItems.ToList().Contains(m.ID))).ToList();
            if (priceRebateItems.Count() == 0)
            {
                return BadRequest(new { Message = "No price rebate items matched" });
            }

            var documentFromCashewItems = (await _unitOfWork.DocumentFromCashewItems.GetByConditions(m => form.DocumentFromCashewItems.ToList().Contains(m.ID))).ToList();
            if (priceRebateItems.Count() == 0)
            {
                return BadRequest(new { Message = "No document from cashew items matched" });
            }

            var priceRebateItemsDocumentNosAreTheSame = priceRebateItems.All(p => p.DocumentNo == priceRebateItems.First().DocumentNo);
            if (!priceRebateItemsDocumentNosAreTheSame)
            {
                return BadRequest(new { Message = "Not all document nos selected are the same." });
            }

            foreach (var priceRebateItem in priceRebateItems)
            {
                if (priceRebateItem.Matched)
                {
                    return BadRequest(new { Message = "Price rebate is already matched. Please kindly check again." });
                }
            }

            foreach (var documentFromCashewItem in documentFromCashewItems)
            {
                if (documentFromCashewItem.Matched)
                {
                    return BadRequest(new { Message = "Document from Cashew item is already matched. Please kindly check again." });
                }
            }

            var documentFromCashewItemsDocumentNosAreTheSame = documentFromCashewItems.All(p => p.DocumentFromCashew?.DocumentNo == priceRebateItems.First().DocumentNo);
            if (!documentFromCashewItemsDocumentNosAreTheSame)
            {
                return BadRequest(new { Message = "Not all document nos selected are the same." });
            }

            var allDocumentNosAreTheSame = priceRebateItems.First().DocumentNo == documentFromCashewItems.First().DocumentFromCashew?.DocumentNo;
            if (!allDocumentNosAreTheSame)
            {
                return BadRequest(new { Message = "Not all document nos selected are the same." });
            }

            var priceRebateItemsStockCodesAreTheSame = priceRebateItems.All(p => p.StockCode == priceRebateItems.First().StockCode);
            if (!priceRebateItemsStockCodesAreTheSame)
            {
                return BadRequest(new { Message = "Not all stock codes selected are the same." });
            }

            var documentFromCashewItemsStockCodesAreTheSame = documentFromCashewItems.All(p => p.StockCode == priceRebateItems.First().StockCode);
            if (!documentFromCashewItemsStockCodesAreTheSame)
            {
                return BadRequest(new { Message = "Not all stock codes selected are the same." });
            }

            var allStockCodesAreTheSame = priceRebateItems.First().StockCode == documentFromCashewItems.First().StockCode;
            if (!allStockCodesAreTheSame)
            {
                return BadRequest(new { Message = "Not all stock codes selected are the same." });
            }

            var quantitiesAreTheSame = priceRebateItems.Sum(p => p.Quantity) == documentFromCashewItems.Sum(p => p.Quantity);
            if (!quantitiesAreTheSame)
            {
                return BadRequest(new { Message = "Quantities for both side do not balance. Please kindly check again." });
            }

            var matching = new MatchingViewModel();
            matching.Name = "Matching";
            await _unitOfWork.Matchings.AddAsync(matching);

            foreach (var priceRebateItem in priceRebateItems)
            {
                priceRebateItem.Matched = true;
                _unitOfWork.PriceRebateItems.Update(priceRebateItem);
                matching.PriceRebateItems.Add(priceRebateItem);
            }

            foreach (var documentFromCashewItem in documentFromCashewItems)
            {
                documentFromCashewItem.Matched = true;
                _unitOfWork.DocumentFromCashewItems.Update(documentFromCashewItem);
                matching.DocumentFromCashewItems.Add(documentFromCashewItem);
            }

            var allMatched = true;
            if (priceRebateItems.Any(i => !i.Matched)) {
                allMatched = false;
            }
            var priceRebate = await _unitOfWork.PriceRebates.GetByIdAsync(priceRebateItems[0].PriceRebateID);
            //priceRebate.AllItemsAreMatched = allMatched;
            _unitOfWork.PriceRebates.Update(priceRebate);

            await _unitOfWork.CompleteAsync();

            return Ok(new
            {
                Message = "Matched!"
            });
        }

        [HttpPost("Unmatch")]
        public async Task<IActionResult> Unmatch([FromBody] UnmatchWithDocumentFromCashewForm form)
        {
            if (form.PriceRebateItems == null)
            {
                return BadRequest(new
                {
                    Message = "Please provide price rebate items."
                });
            }
            var matchings = await _unitOfWork.Matchings.
                GetByConditions(m => m.PriceRebateItems.
                    Any(i => form.PriceRebateItems.
                        Contains(i.ID)));

            foreach (var matching in matchings) {
                foreach (var priceRebateItem in matching.PriceRebateItems)
                {
                    var priceRebate = await _unitOfWork.PriceRebates.GetByIdAsync(priceRebateItem.PriceRebateID);
                    //priceRebate.AllItemsAreMatched = false;
                    _unitOfWork.PriceRebates.Update(priceRebate);
                    priceRebateItem.Matched = false;
                    _unitOfWork.PriceRebateItems.Update(priceRebateItem);
                }
                foreach (var documentFromCashewItem in matching.DocumentFromCashewItems)
                {
                    documentFromCashewItem.Matched = false;
                    _unitOfWork.DocumentFromCashewItems.Update(documentFromCashewItem);
                }
                _unitOfWork.Matchings.Delete(matching);
            }
            await _unitOfWork.CompleteAsync();
            return Ok(new
            {
                Message = "Unmatched!"
            });
        }

        // DELETE: api/matchings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatching(int id)
        {
            var matching = await _unitOfWork.Matchings.GetByIdAsync(id);
            if (matching == null)
            {
                return NotFound();
            }

            _unitOfWork.Matchings.Delete(matching);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        private async Task<bool> MatchingExists(long id)
        {
            return await _unitOfWork.Matchings.AnyAsync(e => e.ID == id);
        }
    }
}
