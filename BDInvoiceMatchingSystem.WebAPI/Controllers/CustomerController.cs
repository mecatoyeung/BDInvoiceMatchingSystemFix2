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
    [Authorize]
    [ApiController]
    [Route("Customers")]
    public class CustomerController : ControllerBase
    {
        private IUnitOfWork _unitOfWork { get; set; }
        private readonly IMapper _mapper;
        public CustomerController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/filesources
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return Ok(await _unitOfWork.Customers.GetAllAsync());
        }

        // GET: api/filesources/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(long id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            var vm = _mapper.Map<CustomerViewModel>(customer);

            return Ok(vm);
        }

        // POST: api/filesources
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerCreateForm form)
        {
            var customer = new Customer();
            customer.CustomerCode = form.CustomerCode;
            customer.CustomerName = form.CustomerName;
            customer.CustomerAddress = form.CustomerAddress;
            await _unitOfWork.Customers.AddAsync(customer);

            await _unitOfWork.CompleteAsync();

            foreach (var approximateName in form.ApproximateNames)
            {
                var mapping = new CustomerApproximateMapping();
                mapping.CustomerID = customer.ID;
                mapping.ApproximateValue = approximateName;
                await _unitOfWork.CustomerApproximateMappings.AddAsync(mapping);
            }

            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.ID }, customer);
        }

        // PUT: api/filesources/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(long id, CustomerUpdateForm form)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound("Customer not found");
                }

                customer.CustomerCode = form.CustomerCode;
                customer.CustomerName = form.CustomerName;
                customer.CustomerAddress = form.CustomerAddress;

                _unitOfWork.CustomerApproximateMappings.DeleteByConditions(m => m.CustomerID == customer.ID);

                foreach (var approximateName in form.ApproximateNames)
                {
                    var mapping = new CustomerApproximateMapping();
                    mapping.CustomerID = customer.ID;
                    mapping.ApproximateValue = approximateName;
                    await _unitOfWork.CustomerApproximateMappings.AddAsync(mapping);
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var updatedCustomer = await _unitOfWork.Customers.GetByIdAsync(id);

            var vm = _mapper.Map<CustomerViewModel>(updatedCustomer);

            return Ok(vm);
        }

        // DELETE: api/filesources/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(long id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _unitOfWork.Customers.Delete(customer);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        private async Task<bool> CustomerExists(long id)
        {
            return await _unitOfWork.Customers.AnyAsync(e => e.ID == id);
        }
    }
}
