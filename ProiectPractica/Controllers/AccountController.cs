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

        private String convertHashPassword(String password)
        {
            string base64HashedPasswordBytes;
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashedPasswordBytes = sha256.ComputeHash(passwordBytes);
                base64HashedPasswordBytes = Convert.ToBase64String(hashedPasswordBytes);
            }
            return base64HashedPasswordBytes;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDTO payload)
        {
            /*
            string base64HashedPasswordBytes;
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(payload.Password);
                var hashedPasswordBytes = sha256.ComputeHash(passwordBytes);
                base64HashedPasswordBytes = Convert.ToBase64String(hashedPasswordBytes);
            }*/
            string base64HashedPasswordBytes= convertHashPassword(payload.Password);
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

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterDTO payload)
        {
            if (_db.Users.Any(u => u.UserName == payload.UserName))
                return BadRequest("Username is already taken.");

            if (_db.Users.Any(u => u.Email == payload.Email))
                return BadRequest("Email is already taken.");

            User user = new User();
            user.UserName = payload.UserName;
            user.Email = payload.Email;
            user.BirthDate = payload.BirthDate;
            user.HashedPassword = convertHashPassword(payload.Password);
            _db.Users.Add(user);
            _db.SaveChanges();
            return Ok();
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
