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
        public async Task<ActionResult<IEnumerable<Matching>>> GetMatchings()
        {
            return Ok(await _unitOfWork.FileSources.GetAllAsync());
        }

        // GET: api/matchings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Matching>> GetMatching(long id)
        {
            var matching = await _unitOfWork.Matchings.GetByIdAsync(id);

            if (matching == null)
            {
                return NotFound();
            }

            return Ok(matching);
        }

        [HttpGet("Multiple")]
        public async Task<ActionResult<Matching>> GetMatchings([FromQuery] List<long> documentFromCashewIds)
        {
            var matchings = await _unitOfWork.Matchings.GetByConditions(m => documentFromCashewIds.Contains(m.ID));

            if (matchings == null)
            {
                return NotFound();
            }

            return Ok(matchings);
        }

        [HttpPost("Match")]
        public async Task<ActionResult> Match([FromBody] MatchWithDocumentFromCashewForm form)
        {
            if (form.PriceRebateItems == null || form.PriceRebateItems.Count() == 0)
            {
                return BadRequest(new { Message = "Please provide price rebate items" });
            }
            
            /*if (form.DocumentFromCashewItems == null || form.DocumentFromCashewItems.Count() == 0)
            {
                return BadRequest(new { Message = "Please provide documents from cashew" });
            }*/

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

            if (documentFromCashewItems.Count() == 0)
            {
                foreach (var priceRebateItem in priceRebateItems)
                {
                    priceRebateItem.Matched = true;
                    _unitOfWork.PriceRebateItems.Update(priceRebateItem);
                }
                await _unitOfWork.CompleteAsync();

                var mPriceRebate = await _unitOfWork.PriceRebates.GetByIdAsync(priceRebateItems[0].PriceRebateID);
                var mAllMatched = true;
                if (await _unitOfWork.PriceRebateItems.Any(i => i.PriceRebateID == mPriceRebate.ID && !i.AutoMatchCompleted))
                {
                    mAllMatched = false;
                }

                mPriceRebate.AllItemsAreMatched = mAllMatched;
                if (mAllMatched)
                {
                    mPriceRebate.Status = PriceRebateStatus.COMPLETED;
                }
                _unitOfWork.PriceRebates.Update(mPriceRebate);

                return Ok(new
                {
                    Message = "Matched!"
                });
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

            var matching = new Matching();
            matching.Name = "Matching";
            await _unitOfWork.Matchings.AddAsync(matching);

            await _unitOfWork.CompleteAsync();

            foreach (var priceRebateItem in priceRebateItems)
            {
                priceRebateItem.Matched = true;
                priceRebateItem.MatchingID = matching.ID;
                _unitOfWork.PriceRebateItems.Update(priceRebateItem);
            }

            foreach (var documentFromCashewItem in documentFromCashewItems)
            {
                documentFromCashewItem.Matched = true;
                documentFromCashewItem.MatchingID = matching.ID;
                _unitOfWork.DocumentFromCashewItems.Update(documentFromCashewItem);
            }

            var priceRebate = await _unitOfWork.PriceRebates.GetByIdAsync(priceRebateItems[0].PriceRebateID);
            var allMatched = true;
            if (await _unitOfWork.PriceRebateItems.Any(i => i.PriceRebateID == priceRebate.ID && !i.AutoMatchCompleted))
            {
                allMatched = false;
            }

            priceRebate.AllItemsAreMatched = allMatched;
            if (allMatched)
            {
                priceRebate.Status = PriceRebateStatus.COMPLETED;
            }
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

            if (matchings.Count() > 0)
            {
                foreach (var matching in matchings)
                {
                    foreach (var priceRebateItem in matching.PriceRebateItems)
                    {
                        var priceRebate = await _unitOfWork.PriceRebates.GetByIdAsync(priceRebateItem.PriceRebateID);
                        priceRebate.AllItemsAreMatched = false;
                        _unitOfWork.PriceRebates.Update(priceRebate);
                        priceRebateItem.Matched = false;
                        priceRebateItem.MatchingID = null;
                        _unitOfWork.PriceRebateItems.Update(priceRebateItem);
                    }
                    foreach (var documentFromCashewItem in matching.DocumentFromCashewItems)
                    {
                        documentFromCashewItem.Matched = false;
                        documentFromCashewItem.MatchingID = null;
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
            else
            {
                var priceRebateItems = await _unitOfWork.PriceRebateItems.GetByConditions(pbi => form.PriceRebateItems.Contains(pbi.ID));
                foreach (var priceRebateItem in priceRebateItems)
                {
                    var priceRebate = await _unitOfWork.PriceRebates.GetByIdAsync(priceRebateItem.PriceRebateID);
                    priceRebate.AllItemsAreMatched = false;
                    _unitOfWork.PriceRebates.Update(priceRebate);
                    priceRebateItem.Matched = false;
                    priceRebateItem.MatchingID = null;
                    _unitOfWork.PriceRebateItems.Update(priceRebateItem);
                }
                await _unitOfWork.CompleteAsync();
                return Ok(new
                {
                    Message = "Unmatched!"
                });
            }
        }

        [HttpPost("AutoMatch")]
        public async Task<IActionResult> AutoMatch([FromBody] AutoMatchForm form)
        {
            var priceRebate = await _unitOfWork.PriceRebates.GetByIdAsync(form.PriceRebateId);
            priceRebate.Status = PriceRebateStatus.QUEUED;
            _unitOfWork.PriceRebates.ExecuteRawSql(String.Format("UPDATE PriceRebateItems SET AutoMatchCompleted = FALSE WHERE PriceRebateID = {0}", priceRebate.ID));
            _unitOfWork.PriceRebates.Update(priceRebate);
            await _unitOfWork.CompleteAsync();

            return Ok(new
            {
                Message = $"Auto match for Price Rebate (id={priceRebate.ID}) has been queued."
            });
            /*var priceRebate = await _unitOfWork.PriceRebates.GetByIdAsync(form.PriceRebateId);

            try
            {
                priceRebate.Status = PriceRebateStatus.PROCCESSING;
                _unitOfWork.PriceRebates.Update(priceRebate);
                await _unitOfWork.CompleteAsync();

                var priceRebateItems = await _unitOfWork.PriceRebateItems.GetByConditions(pbi =>
                    pbi.PriceRebateID == form.PriceRebateId &&
                    !pbi.Matched);
                Console.WriteLine("Start of AutoMatch");
                var index = 0;
                foreach (var priceRebateItem in priceRebateItems)
                {
                    Console.WriteLine($"AutoMatch Index: {index}");
                    var correspondingDocumentFromCashewItems = (await _unitOfWork.DocumentFromCashewItems.GetByConditions(ci =>
                        ci.DocumentFromCashew.DocumentNo == priceRebateItem.DocumentNo &&
                        ci.StockCode == priceRebateItem.StockCode &&
                        ci.Quantity == priceRebateItem.Quantity &&
                        !ci.Matched)).ToList();
                    if (correspondingDocumentFromCashewItems.Count > 0)
                    {
                        priceRebateItem.Matched = true;
                        _unitOfWork.PriceRebateItems.Update(priceRebateItem);

                        foreach (var documentFromCashewItem in correspondingDocumentFromCashewItems)
                        {
                            documentFromCashewItem.Matched = true;
                            _unitOfWork.DocumentFromCashewItems.Update(documentFromCashewItem);
                        }
                    }
                    index++;
                    if (index % 100 == 0)
                    {
                        priceRebate.AutoMatchProgress = index / priceRebateItems.Count();
                        _unitOfWork.PriceRebates.Update(priceRebate);
                        await _unitOfWork.CompleteAsync();
                    }
                }

                var allMatched = true;
                if (await _unitOfWork.PriceRebateItems.Any(i => !i.Matched))
                {
                    allMatched = false;
                }

                priceRebate.AllItemsAreMatched = allMatched;
                priceRebate.Status = PriceRebateStatus.COMPLETED;
                _unitOfWork.PriceRebates.Update(priceRebate);

                await _unitOfWork.CompleteAsync();

                return Ok(new
                {
                    Message = "Auto matched!"
                });
            }
            catch (Exception ex)
            {
                priceRebate.Status = PriceRebateStatus.READY;
                _unitOfWork.PriceRebates.Update(priceRebate);

                return BadRequest(new
                {
                    Message = ex?.InnerException?.StackTrace
                });
            }*/
        }

        [HttpPost("AutoMatchProgress")]
        public async Task<IActionResult> AutoMatchProgress([FromBody] AutoMatchForm form)
        {
            var priceRebate = await _unitOfWork.PriceRebates.GetByIdAsync(form.PriceRebateId);
            var priceRebateItemsCount = priceRebate.TotalUploadRow;
            var matchedPriceRebateItemsCount = await _unitOfWork.PriceRebateItems.Count(pbi =>
                pbi.PriceRebateID == form.PriceRebateId &&
                pbi.AutoMatchCompleted);

            return Ok(new
            {
                PriceRebateId = priceRebate.ID,
                priceRebate.Status,
                PriceRebateItemsCount = priceRebateItemsCount,
                MatchedPriceRebateItemsCount = matchedPriceRebateItemsCount,
                AutoMatchProgress = new Decimal(matchedPriceRebateItemsCount) / new Decimal(priceRebateItemsCount) * 100
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
