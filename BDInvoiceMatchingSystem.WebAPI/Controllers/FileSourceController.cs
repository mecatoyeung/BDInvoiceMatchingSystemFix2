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
using Microsoft.AspNetCore.Authorization;

namespace BDInvoiceMatchingSystem.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("FileSources")]
    public class FileSourceController : ControllerBase
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IMapper _mapper;
        public FileSourceController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/filesources
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileSource>>> GetFileSources()
        {
            return Ok(await _unitOfWork.FileSources.GetAllAsync());
        }

        // GET: api/filesources/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FileSource>> GetFileSource(long id)
        {
            var fileSource = await _unitOfWork.FileSources.GetByIdAsync(id);

            if (fileSource == null)
            {
                return NotFound();
            }

            return Ok(fileSource);
        }

        // POST: api/filesources
        [HttpPost]
        public async Task<ActionResult<FileSource>> PostFileSource(FileSourceCreateForm form)
        {
            var fileSource = _mapper.Map<FileSource>(form);

            await _unitOfWork.FileSources.AddAsync(fileSource);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetFileSource), new { id = fileSource.ID }, fileSource);
        }

        // PUT: api/filesources/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFileSource(long id, FileSource fileSource)
        {
            if (id != fileSource.ID)
            {
                return BadRequest();
            }

            _unitOfWork.Entry(fileSource).State = EntityState.Modified;

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FileSourceExists(id))
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

        // DELETE: api/filesources/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileSource(int id)
        {
            var fileSource = await _unitOfWork.FileSources.GetByIdAsync(id);
            if (fileSource == null)
            {
                return NotFound();
            }

            _unitOfWork.FileSources.Delete(fileSource);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        private async Task<bool> FileSourceExists(long id)
        {
            return await _unitOfWork.FileSources.AnyAsync(e => e.ID == id);
        }
    }
}
