using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ProiectPractica.Data;
using ProiectPractica.Data.Entities;
using ProiectPractica.DTOs;
using static ProiectPractica.Enums;

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

                    var reactions = _db.Reactions.Where(r => r.PostId == postId);
                    foreach(Reaction reaction in reactions)
                        _db.Reactions.Remove(reaction);

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

        [HttpGet("getPost")]
        public ActionResult<Post> GetPost(int postId)
        {
            try
            {
                Post post = _db.Posts.Where(p => p.Id == postId).Include(p => p.Comments).Include(p => p.Reactions).SingleOrDefault();
                if (post is null) return NotFound("No post with id: " + postId);
                else return Ok(post);
            }catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            }
        }

        [HttpGet("getPostComments")]
        public ActionResult<List<Comment>> GetPostComments(int postId)
        {
            try
            {
                var comments = _db.Comments.Where(c => c.Id == postId).ToList();
                if (comments is null) return NotFound("No post with id: " + postId);
                else return Ok(comments);
            }
            catch (Exception)
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
                    return NotFound("Comment with id: " + commentId + " not found");
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

        [HttpPost("addReaction")]
        public ActionResult AddNewReaction(ReactionType reactionType, int postId)
        {
            try
            {
                User user = _db.Users.Where(u => u.Id == getUserId()).SingleOrDefault();
                Reaction react = new Reaction();
                react.PostId = postId;
                react.UserId = getUserId();
                react.Type = reactionType;
                var existingReaction = _db.Reactions.Where(r => r.UserId == getUserId() && r.PostId == postId).SingleOrDefault();
                if (existingReaction != null) {
                    _db.Reactions.Remove(existingReaction);
                }
                _db.Reactions.Add(react);
                _db.SaveChanges();
                return Ok("Reaction added!");
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpDelete("deleteReaction")]
        public ActionResult DeleteReaction(int reactionId)
        {
            try
            {
                int requestingUser = getUserId();
                Reaction reactToDelete = _db.Reactions
                                    .Where(r => r.Id == reactionId)
                                    .SingleOrDefault();
                if (reactToDelete is null)
                    return NotFound("Reaction with id: " + reactionId + " not found");
                else if (reactToDelete.UserId == requestingUser)
                {
                    _db.Reactions.Remove(reactToDelete);
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

        [HttpGet("getPostReactions")]
        public ActionResult<List<Reaction>> GetPostReactions(int postId)
        {
            try
            {
                var reactions = _db.Comments.Where(r => r.Id == postId).ToList();
                if (reactions is null) return NotFound("No post with id: " + postId);
                else return Ok(reactions);
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
