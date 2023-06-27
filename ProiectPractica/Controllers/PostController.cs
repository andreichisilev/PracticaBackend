using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectPractica.Data;
using ProiectPractica.Data.Entities;
using ProiectPractica.DTOs;

namespace ProiectPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly SocialMediaDB _db;

        public PostController(SocialMediaDB db)
        {
            _db = db;
        }

        [HttpPost("addPost")]
        public ActionResult AddNewPost([FromBody] PostDTO payload) {
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
                {   Post post = new Post();
                    post.Content = payload.Content;
                    post.PictureURL = payload.PictureURL;
                    post.DatePosted = DateTime.Now;
                    post.User = user;
                    post.UserId = userId;
                    _db.Posts.Add(post);
/*                    user.Posts.Add(post);*/
                    _db.Users.Update(user);
                    _db.SaveChanges();
                    return Ok("Post added");
                }
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpDelete("deletePost")]
        public ActionResult DeletePost(int postId)
        {
            try
            {
                int requestingUser = getUserId();
                Post postToDelete = _db.Posts
                                    .Where(p => p.Id == postId)
                                    .SingleOrDefault();
                if (postToDelete is null)
                    return NotFound("Post with id: " + postId + " not found");
                else if (postToDelete.UserId == requestingUser)
                {
                    var comments = _db.Comments.Where(c => c.PostId == postId);
                    foreach(Comment comment in comments)
                        _db.Comments.Remove(comment);

                    _db.Posts.Remove(postToDelete);
                    _db.SaveChanges();
                    return Ok("Post successfully deleted!");
                }
                else return Unauthorized("Not the post owner!");
            }catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("addComment")]
        public ActionResult AddNewComment([FromBody] CommentDTO payload,int postId) {
            try
            {
                User user = _db.Users.Where(u => u.Id == getUserId()).SingleOrDefault();
                Comment comment = new Comment();
                comment.PostId = postId;
                comment.UserId = getUserId();
                comment.DateCommented= DateTime.Now;
                comment.Content = payload.Content;
                _db.Comments.Add(comment);
                _db.SaveChanges();
                return Ok("Comment added!");
            }catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpDelete("deleteComment")]
        public ActionResult DeleteComment(int commentId)
        {
            try
            {
                int requestingUser = getUserId();
                Comment commentToDelete = _db.Comments
                                    .Where(c => c.Id == commentId)
                                    .SingleOrDefault();
                if (commentToDelete is null)
                    return NotFound("Post with id: " + commentId + " not found");
                else if (commentToDelete.UserId == requestingUser)
                {
                    _db.Comments.Remove(commentToDelete);
                    _db.SaveChanges();
                    return Ok("Comment successfully deleted!");
                }
                else return Unauthorized("Not the comment owner!");
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
