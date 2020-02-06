using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DualJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        [HttpGet("oauth")]
        public async Task<IActionResult> GetOauth()
        {
            var client = new HttpClient();

            //var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            //if (disco.IsError) throw new Exception(disco.Error);

            var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "http://localhost:5000/connect/token",

                ClientId = "client",
                ClientSecret = "secret"
            });

            if (response.IsError) throw new Exception(response.Error);
            return Ok(response.AccessToken);
        }

        [HttpGet("jwt")]
        public async Task<IActionResult> GetJwt()
        {
            var claims = new List<Claim>();

            claims.Add(new Claim("UserId", "123"));

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("tosekret123dasdasd213das"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                "http://localhost:59043",
                "http://localhost:59043",
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );

            return Ok(new JwtSecurityTokenHandler().WriteToken(jwtToken));
        }

    }
}
