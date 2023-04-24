using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProiectPractica.Data;
using ProiectPractica.Data.Entities;
using ProiectPractica.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProiectPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly SocialMediaDB _db;
        private readonly IConfiguration _config;


        public
            AccountController(SocialMediaDB db, IConfiguration config)
        {
            _db= db;
            _config= config;
        }
        public ActionResult Login([FromBody] LoginDTO payload)
        {
            string base64HashedPasswordBytes;
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(payload.Password);
                var hashedPasswordBytes = sha256.ComputeHash(passwordBytes);
                base64HashedPasswordBytes = Convert.ToBase64String(hashedPasswordBytes);
            }
        
            var existingUser = _db.Users
                .Where(u => u.Email == payload.UserName
                        && u.HashedPassword == base64HashedPasswordBytes)
                .SingleOrDefault();
            if (existingUser == null)
            {
                return NotFound();
            }
            else
            {
                var jwt = GenerateJSONWebToken(existingUser);
                return Ok(jwt);
            }

        }
        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            var token = new JwtSecurityToken(_config["JWT:Issuer"],
                _config["JWT:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
