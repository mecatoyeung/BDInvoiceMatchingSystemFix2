using BDInvoiceMatchingSystem.WebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BDInvoiceMatchingSystem.WebAPI.Helpers
{
    public static class JwtHelper
    {
        public static string GenerateToken(ApplicationUser user, string key, string issuer, string audience)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(key);
            List<Claim> claims = new List<Claim> { };
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            if (user.AccountType == Enums.EnumAccountType.Admin)
            {
                claims.Add(new Claim("Admin", "true"));
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddHours(9),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
