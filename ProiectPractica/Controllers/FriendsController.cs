using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProiectPractica.Data;
using ProiectPractica.Data.Entities;

namespace ProiectPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly SocialMediaDB _db;

        public FriendsController(SocialMediaDB db)
        {
            _db = db;
        }

        [HttpPost("sendFriendRequest")]
        public ActionResult sendFriendRequest(int userId)
        {
            if (userId == getUserId())
                return BadRequest("Can not send a friend request to yourself.");

            if (_db.FriendRequests.Where(
                r => r.UserSenderId == getUserId() && r.UserReceiverId == userId)
                .Any())
                return BadRequest("Friend request already sent.");

            if (_db.FriendRequests.Where(
               r => r.UserSenderId == userId && r.UserReceiverId == getUserId())
                .Any())
                acceptRequest(userId);


            FriendRequest request = new FriendRequest();
            request.DateRequested = DateTime.Now;
            request.Status = Enums.FriendRequestStatus.Pending;
            request.UserSenderId = getUserId();
            request.UserReceiverId = userId;
            try
            {
                _db.FriendRequests.Add(request);
                _db.SaveChanges();
                return Ok("Friend request sent!");
            } catch (Exception) {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpGet("requests")]
        public ActionResult getFriendRequests() {
            int userId = getUserId();
            List<FriendRequest> requests = _db.FriendRequests.Where(r => r.UserReceiverId == userId).ToList();
            return Ok(requests);
        }

        [HttpPatch("acceptRequest")]
        public ActionResult acceptRequest(int userId)
        {
            try
            {
                FriendRequest request = _db.FriendRequests.Where(r => r.UserReceiverId == getUserId() &&
                r.UserSenderId == userId).SingleOrDefault();
                if (request == null)
                    request = _db.FriendRequests.Where(r => r.UserReceiverId == userId &&
                r.UserSenderId == getUserId()).SingleOrDefault();
                if (request != null)
                {
                    request.Status = Enums.FriendRequestStatus.Accepted;
                    request.DateResponded = DateTime.Now;
                    _db.SaveChanges();
                    return Ok("Friend added.");
                }
                return NotFound("Friend request between you and " + userId + " does not exist.");
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            }
        }
        

        [HttpDelete("removeFriend")]
        public ActionResult removeFriend(int userId) {
            try
            {
                FriendRequest request = _db.FriendRequests.Where(r => r.UserReceiverId == getUserId() &&
                r.UserSenderId == userId).SingleOrDefault();
                if (request == null)
                    request = _db.FriendRequests.Where(r => r.UserReceiverId == userId &&
                r.UserSenderId == getUserId()).SingleOrDefault();
                if (request != null)
                {
                    _db.FriendRequests.Remove(request);
                    _db.SaveChanges();
                    return Ok("Friend removed.");
                }
                return NotFound("Friend request between you and " + userId + " does not exist.");
            }
            catch (Exception)
            {
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
