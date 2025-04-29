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

using BDInvoiceMatchingSystem.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using BDInvoiceMatchingSystem.WebAPI.ViewModels;
using Microsoft.Extensions.Options;

namespace BDInvoiceMatchingSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IConfiguration configuration, 
            IOptions<AppSettings> appSettings,
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager)
        {
            this._configuration = configuration;
            this._appSettings = appSettings.Value;
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] AccountRegisterForm form)
        {
            if (form.Secret != "Aa123456")
            {
                return BadRequest(new
                {
                    Message = "Secret is wrong."
                });
            }

            if (form.Password != form.ConfirmPassword)
            {
                return BadRequest(new
                {
                    Message = "Confirm Password does not match."
                });
            }

            var user = new ApplicationUser { UserName = form.Email, Email = form.Email };
            var result = await _userManager.CreateAsync(user, form.Password);
            if (result.Succeeded)
            {
                var secretKey = _configuration.GetValue<string>("Jwt:Key") ?? "";
                var issuer = _configuration.GetValue<string>("Jwt:Issuer") ?? "";
                var audience = _configuration.GetValue<string>("Jwt:Audience") ?? "";
                var token = JwtHelper.GenerateToken(user, secretKey, issuer, audience);
                Response.Cookies.Append("X-Access-Token", token, new CookieOptions
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.None
                });
                return Ok(new {
                    Message = "Registration is successful",
                    Token = token 
                });
            }
            return BadRequest(new
            {
                Message = String.Join("\n", result.Errors.Select(e => e.Description.ToString()))
            });
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AccountAuthenticateForm form)
        {
            var secretKey = _configuration.GetValue<string>("Jwt:Key") ?? "";
            var issuer = _configuration.GetValue<string>("Jwt:Issuer") ?? "";
            var audience = _configuration.GetValue<string>("Jwt:Audience") ?? "";

            var user = await _userManager.FindByEmailAsync(form.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, form.Password))
            {
                var token = JwtHelper.GenerateToken(user, secretKey, issuer, audience);
                Response.Cookies.Append("X-Access-Token", token, new CookieOptions
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.None
                });
                return Ok(new { Username = user.UserName, Token = token });
            }
            return Unauthorized();
        }

        [Authorize]
        [HttpGet("Current")]
        public async Task<IActionResult> Current()
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? String.Empty);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            UserViewModel vm = new UserViewModel();
            vm.UserId = user.Id;
            vm.Email = user.Email ?? "";
            vm.Username = user.Email ?? "";

            return Ok(vm);
        }

        [Authorize("AdminOnly")]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] AccountChangePasswordForm form)
        {
            var user = await _userManager.FindByIdAsync(form.UserId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            if (form.NewPassword != form.ConfirmNewPassword)
            {
                return BadRequest("Password not match.");
            }

            var result = await _userManager.ChangePasswordAsync(user, form.OldPassword, form.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password changed successfully.");
            }

            return BadRequest("Unexpected error. Please contract system administrator.");
        }
    }
}
