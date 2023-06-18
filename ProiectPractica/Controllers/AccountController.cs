using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProiectPractica.Data;
using ProiectPractica.Data.Entities;
using ProiectPractica.DTOs;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProiectPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        private string convertHashPassword(string password)
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

        private int getUserId()
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            string jwt = null;
            if (authorizationHeader != null
                && authorizationHeader.StartsWith("Bearer "))
            {
                jwt = authorizationHeader.Substring("Bearer ".Length).Trim();
            }

            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
            string userIdString = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            int userId = int.Parse(userIdString);
            return userId;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult Login([FromBody] LoginDTO payload)
        {
            string base64HashedPasswordBytes= convertHashPassword(payload.Password);
            var existingUser = _db.Users
                .Where(u => u.Email == payload.Email
                        && u.HashedPassword == base64HashedPasswordBytes)
                .SingleOrDefault();
            if (existingUser is null)
            {
                return NotFound("Email/password invalid.");
            }
            else
            {
                var jwt = GenerateJSONWebToken(existingUser);
                return new JsonResult( new { jwt });
            }

        }

        [HttpPost("register")]
        [AllowAnonymous]
        public ActionResult Register([FromBody] RegisterDTO payload)
        {
            if (_db.Users.Any(u => u.Username == payload.Username))
                return Conflict("Username is already taken.");

            if (_db.Users.Any(u => u.Email == payload.Email))
                return Conflict("Email is already taken.");
            
            try
            {

                User user = new User();
                user.Username = payload.Username;
                user.Email = payload.Email;
                user.BirthDate = payload.BirthDate;
                user.ProfilePictureURL = payload.ProfilePictureURL;
                user.HashedPassword = convertHashPassword(payload.Password);
                _db.Users.Add(user);
                _db.SaveChanges();
                return Ok("Registration succesful");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("changeProfilePicture")]
        [Authorize]
        public ActionResult ChangePictureProfile([FromBody] ChangeProfilePictureDTO payload)
        {
           int userId = getUserId();

            User user = _db.Users
                .Where(u => u.Id == userId)
                .SingleOrDefault();
            
            if(user is null) {
                return NotFound("User not existing.");
            }

            else
            {
                user.ProfilePictureURL = payload.profilePictureUrl;
                _db.Users.Update(user);
                _db.SaveChanges();
                return Ok("Action completed");
            }
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id.ToString())
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
