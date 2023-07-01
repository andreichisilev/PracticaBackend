using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectPractica.Data;
using ProiectPractica.Data.Entities;
using ProiectPractica.DTOs;
using System.IdentityModel.Tokens.Jwt;
using static ProiectPractica.Enums;

namespace ProiectPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly SocialMediaDB _db;

        public UserController(SocialMediaDB db)
        {
            _db = db;

        }

        [HttpPut("changeProfilePicture")]
        public ActionResult ChangePictureProfile([FromBody] ChangeProfilePictureDTO payload)
        {
            try
            {
                int userId = getUserId();

                User user = _db.Users
                    .Where(u => u.Id == userId)
                    .SingleOrDefault();

                if (user is null)
                {
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
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("getUsers")]
        public ActionResult<ICollection<User>> GetAll(int pageSize, int pageNumber, UsersSortType sortType)
        {
            // as no tracking for performance improvement when you do not need to track changes
            var usersQuery = _db.Users.AsNoTracking();

            if (sortType == UsersSortType.UsernameAscendent)
                usersQuery = usersQuery.OrderBy(u => u.Username);
            else if (sortType == UsersSortType.UsernameDescendent)
                usersQuery = usersQuery.OrderByDescending(u => u.Username);
            else if (sortType == UsersSortType.DateCreatedAscendent)
                usersQuery = usersQuery.OrderBy(u => u.DateCreated);
            else if (sortType == UsersSortType.DateCreatedDescendent)
                usersQuery = usersQuery.OrderByDescending(u => u.DateCreated);
            else
                usersQuery = usersQuery.OrderBy(u => u.Username);

            usersQuery = usersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var users = usersQuery.ToList();

            return users;

        }

        [HttpGet("getUser")]
        public ActionResult<User> GetUser(int id)
        {
            User user = _db.Users
                .Where(u => u.Id == id)
                .Include(u => u.Posts)
                .SingleOrDefault();
            if (user is null)
            {
                return NotFound("No user with id: "+id);
            }
            else
            {
                return Ok(user);
            }
        }

        [HttpGet("getFeed")]
        public ActionResult<List<Post>> GetFeed()
        {
            var posts = _db.Posts.Where(p => p.UserId != getUserId()).OrderByDescending(p => p.DatePosted).ToList();
            return Ok(posts);
            
        }

        [HttpDelete("closeAccount")]
        public ActionResult CloseAccount() {
            try
            {
                int userId = getUserId();
                if (userId == null)
                {
                    return NotFound("User not found.");
                }
                else
                {
                    User currentUser = _db.Users
                        .Where(u => u.Id == userId)
                        .SingleOrDefault();
                    _db.Users.Remove(currentUser);
                    _db.SaveChanges();
                    return Ok("Account succesfully closed.");
                }
            } catch (Exception) {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private int getUserId()
        {
            var currentUser = HttpContext.User;
            string userIdString = null;
            if (currentUser.HasClaim(claim => claim.Type == "userId"))
            {
                userIdString = currentUser.Claims.FirstOrDefault(c => c.Type == "userId").Value;
            }
            int userId = int.Parse(userIdString);
            return userId;

        }
    }
}
