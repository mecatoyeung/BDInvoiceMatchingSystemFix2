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
using AutoMapper;
using BDInvoiceMatchingSystem.WebAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace BDInvoiceMatchingSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("Tests")]
    public class TestController : ControllerBase
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IMapper _mapper;
        public TestController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/filesources
        [HttpGet]
        public async Task<ActionResult<string>> GetTestResult()
        {
            return Ok("Ok");
        }
    }
}
