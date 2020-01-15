using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExJwtAuth.Configuratins;
using ExJwtAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ExJwtAuth.Controllers
{
    [Route("/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        [Authorize()]
        [HttpGet]
        public IActionResult GetAction()
        {
            var username = HttpContext.User.Identity.Name;
            var result = new
            {
                username = username
            };

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(
            [Required][FromBody]LoginParam loginParam
        )
        {
            var handler = new JwtSecurityTokenHandler();
            var claims = new[] {
                new Claim(ClaimTypes.Name, loginParam.Username)
            };
            var subject = new ClaimsIdentity(claims);
            var credentials = new SigningCredentials(
                JwtSecurityConfiguration.SecurityKey,
                SecurityAlgorithms.HmacSha256);
            var token = handler.CreateJwtSecurityToken(
                audience: loginParam.Username,
                issuer: JwtSecurityConfiguration.Issuer,
                subject: subject,
                signingCredentials: credentials);
            var tokenText = handler.WriteToken(token);
            var result = new
            {
                token = tokenText
            };

            return Ok(result);
        }
    }
}
