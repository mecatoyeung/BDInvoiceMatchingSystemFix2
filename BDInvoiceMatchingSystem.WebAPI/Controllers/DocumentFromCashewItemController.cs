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
using static BDInvoiceMatchingSystem.WebAPI.Data.IUnitOfWork;
using BDInvoiceMatchingSystem.WebAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace BDInvoiceMatchingSystem.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("DocumentFromCashewItems")]
    public class DocumentFromCashewItemController : ControllerBase
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IMapper _mapper;
        public DocumentFromCashewItemController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("Pagination")]
        public async Task<ActionResult<IEnumerable<DocumentFromCashewItem>>> GetDocumentsFromCashewItemsPagination(
                [FromQuery] DocumentFromCashewItemPaginationForm form
            )
        {
            var documentFromCashewItemsPagination = (await _unitOfWork.DocumentFromCashewItems.GetPagination(form));
            var vms = _mapper.Map<DocumentFromCashewItemPagination, DocumentFromCashewItemPaginationViewModel>(documentFromCashewItemsPagination);
            return Ok(vms);
        }

        [HttpGet("ByDocumentNos")]
        public async Task<ActionResult<IEnumerable<DocumentFromCashewItem>>> GetDocumentsFromCashewByInvoiceNos([FromQuery] List<string> documentNos)
        {
            var documentFromCashewItems = (await _unitOfWork.DocumentFromCashewItems.GetByDocumentNosAsync(documentNos)).ToList();
            var vms = _mapper.Map<List<DocumentFromCashewItem>, List<DocumentFromCashewItemListViewModel>>(documentFromCashewItems);
            return Ok(vms);
        }

        // PUT: api/documentfromcashewitems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocumentFromCashewItem(long id, DocumentFromCashewItem form)
        {
            if (id != form.ID)
            {
                return BadRequest();
            }

            var objectFromDb = new DocumentFromCashewItem();
            try
            {
                objectFromDb = await _unitOfWork.DocumentFromCashewItems.GetByIdAsync(id);
            } catch
            {
                return NotFound();
            }

            objectFromDb.StockCode = form.StockCode;
            objectFromDb.Description = form.Description;
            objectFromDb.LotNo = form.LotNo;
            objectFromDb.Quantity = form.Quantity;
            objectFromDb.UnitOfMeasure = form.UnitOfMeasure;
            objectFromDb.UnitPrice = form.UnitPrice;
            objectFromDb.Subtotal = form.Subtotal;

            _unitOfWork.DocumentFromCashewItems.Update(objectFromDb);

            var documentFromCashewFromDb = await _unitOfWork.DocumentsFromCashew.GetByIdAsync(objectFromDb.DocumentFromCashewID);
            documentFromCashewFromDb.CustomerCode = form.DocumentFromCashew.CustomerCode;
            documentFromCashewFromDb.DocumentNo = form.DocumentFromCashew.DocumentNo;
            documentFromCashewFromDb.DocumentDate = form.DocumentFromCashew.DocumentDate;
            documentFromCashewFromDb.DeliveryDate = form.DocumentFromCashew.DeliveryDate;

            _unitOfWork.DocumentsFromCashew.Update(documentFromCashewFromDb);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DocumentFromCashewItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/documentfromcashewitems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocumentFromCashewItem(int id)
        {
            var documentFromCashewItem = await _unitOfWork.DocumentFromCashewItems.GetByIdAsync(id);
            if (documentFromCashewItem == null)
            {
                return NotFound();
            }

            var matchingID = documentFromCashewItem.MatchingID;

            _unitOfWork.DocumentFromCashewItems.Delete(documentFromCashewItem);

            if (matchingID != null)
            {
                var matching = await _unitOfWork.Matchings.GetByIdAsync(matchingID ?? 0);
                var priceRebateItems = await _unitOfWork.PriceRebateItems.GetByConditions(i => i.MatchingID == matchingID);
                foreach (var priceRebateItem in priceRebateItems)
                {
                    priceRebateItem.Matched = false;
                    priceRebateItem.MatchingID = null;
                    _unitOfWork.PriceRebateItems.Update(priceRebateItem);
                }
                if (matching != null) _unitOfWork.Matchings.Delete(matching);
            }

            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("BatchDelete")]
        public IActionResult BatchDeleteDocumentFromCashewItem([FromBody] BatchDeleteDocumentFromCashewItemForm form)
        {
            string sql = $"DELETE FROM DocumentFromCashewItems WHERE Id IN ({string.Join(",", form.Ids)})";
            _unitOfWork.DocumentFromCashewItems.ExecuteRawSql(sql);

            return NoContent();
        }

        private async Task<bool> DocumentFromCashewItemExists(long id)
        {
            return await _unitOfWork.DocumentFromCashewItems.AnyAsync(e => e.ID == id);
        }
    }
}
