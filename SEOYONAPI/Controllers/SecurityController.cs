using BusinessEntity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SEOYONAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {

        [HttpPost]
        public IActionResult Generatejsonwebtoken(AuthenticateRequest model)
        {
            model.EmailID = "SisTrac";
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("RU1QVUxTRVNFT1lPTlNJU1RSQUM="));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("Issuer","Empulse"),
                new Claim("Admin","true"),
                new Claim(JwtRegisteredClaimNames.UniqueName, model.EmailID)
            };
            var token = new JwtSecurityToken("Empulse","Empulse", claims, expires: DateTime.Now.AddDays(4000), signingCredentials: credentials);

            var response= new JwtSecurityTokenHandler().WriteToken(token);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        
    }
}
