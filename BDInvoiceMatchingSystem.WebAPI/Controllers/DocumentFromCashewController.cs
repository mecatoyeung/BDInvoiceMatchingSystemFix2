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
using Microsoft.Extensions.Options;

namespace BDInvoiceMatchingSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("DocumentsFromCashew")]
    public class DocumentFromCashewController : ControllerBase
    {

        private readonly MySettings _mySettings;
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IMapper _mapper;
        public DocumentFromCashewController(
            IOptions<MySettings> mySettings,
            IUnitOfWork unitOfWork, 
            IMapper mapper)
        {
            _mySettings = mySettings.Value;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentFromCashew>>> GetDocumentsFromCashew()
        {
            var documentsFromCashew = (await _unitOfWork.DocumentsFromCashew.GetAllAsync()).ToList();
            return Ok(documentsFromCashew);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentFromCashew>> GetDocumentFromCashew(long id)
        {
            var documentFromCashew = await _unitOfWork.DocumentsFromCashew.GetByIdAsync(id);
            return Ok(documentFromCashew);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocumentFromCashew(long id, DocumentFromCashew documentFromCashew)
        {
            if (id != documentFromCashew.ID)
            {
                return BadRequest();
            }

            _unitOfWork.Entry(documentFromCashew).State = EntityState.Modified;

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DocumentFromCashewExists(id))
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

        [HttpGet("{id}/DownloadCsv")]
        public async Task<ActionResult<DocumentFromCashew>> GetCsv(long id)
        {
            var documentFromCashew = await _unitOfWork.DocumentsFromCashew.GetByIdAsync(id);

            var filePath = Path.Combine(_mySettings.DocumentFromCashewFolderPath, documentFromCashew.CSVFilename);
            if (System.IO.File.Exists(filePath))
            {
                var fileName = Path.GetFileName(filePath);
                var contentType = "application/octet-stream"; // You can specify the correct content type based on the file type

                return PhysicalFile(filePath, contentType, fileName);
            }
            else
            {
                return NotFound("File not found.");
            }
        }

        [HttpGet("{id}/DownloadPdf")]
        public async Task<ActionResult<DocumentFromCashew>> GetPdf(long id)
        {
            var documentFromCashew = await _unitOfWork.DocumentsFromCashew.GetByIdAsync(id);

            var filePath = Path.Combine(_mySettings.DocumentFromCashewFolderPath, documentFromCashew.PDFFilename);
            if (System.IO.File.Exists(filePath))
            {
                var fileName = Path.GetFileName(filePath);
                var contentType = "application/octet-stream"; // You can specify the correct content type based on the file type

                return PhysicalFile(filePath, contentType, fileName);
            }
            else
            {
                return NotFound("File not found.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocumentFromCashew(long id)
        {
            var documentFromCashew = await _unitOfWork.DocumentsFromCashew.GetByIdAsync(id);
            if (documentFromCashew == null)
            {
                return NotFound();
            }

            var documentToCashewItems = await _unitOfWork.DocumentFromCashewItems.GetByConditions(i => i.DocumentFromCashewID == id);
            var matchingIds = documentToCashewItems.Select(i => i.MatchingID);
            _unitOfWork.Matchings.DeleteByConditions(i => matchingIds.Contains(i.ID));
            _unitOfWork.DocumentsFromCashew.Delete(documentFromCashew);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        private async Task<bool> DocumentFromCashewExists(long id)
        {
            return await _unitOfWork.DocumentsFromCashew.AnyAsync(e => e.ID == id);
        }
    }
}
