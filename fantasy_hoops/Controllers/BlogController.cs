using fantasy_hoops.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models.Enums;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly IBlogRepository _blogRepository;

        public BlogController(IBlogService blogService, IBlogRepository blogRepository)
        {
            _blogService = blogService;
            _blogRepository = blogRepository;
        }

        [HttpGet]
        public List<BlogPostDto> GetApprovedPosts()
        {
            return _blogRepository.GetApprovedPosts();
        }

        [HttpGet("pending")]
        public List<BlogPostDto> GetUnapprovedPosts()
        {
            return _blogRepository.GetUnapprovedPosts();
        }
        
        [HttpGet("{id}")]
        public BlogPostDto GetPostById(int id)
        {
            return _blogRepository.GetPostById(id);
        }

        [Authorize(Roles = "Admin,Creator")]
        [HttpPost]
        public IActionResult SubmitPost([FromBody]SubmitPostViewModel model)
        {
            bool isAdmin = User.IsInRole("Admin");
            if (isAdmin)
            {
                model.Status = PostStatus.APPROVED;
            }
            
            if (!_blogRepository.AddPost(model))
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Failed creating post.");
            }

            return isAdmin
                ? Ok("Post created successfully.")
                : Ok("Post submitted successfully. The post will be available once approved by administrator.");
        }

        [HttpDelete]
        public IActionResult DeletePost([FromQuery]int id)
        {
            if (!_blogRepository.PostExists(id))
                return StatusCode(StatusCodes.Status404NotFound, "Post not found.");

            if (!_blogRepository.DeletePost(id))
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Failed deleting post.");
            }
            
            return Ok("Deleted successfully.");
        }

        [HttpPut]
        public IActionResult EditPost([FromBody]SubmitPostViewModel model)
        {
            if (!_blogRepository.PostExists(model.Id))
                return StatusCode(StatusCodes.Status404NotFound, "Post not found.");

            if (_blogRepository.UpdatePost(model))
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Failed updating post.");
            }
            
            return Ok("Deleted successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("approve/{postId}")]
        public IActionResult ApprovePost([FromRoute] int postId)
        {
            if (!_blogService.ApprovePost(postId))
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Failed approving blog post.");
            }

            return Ok("Blog post approved successfully.");
        }
    }
}
