using fantasy_hoops.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly IBlogService _service;
        private readonly IBlogRepository _repository;

        public BlogController(IBlogService service, IBlogRepository repository)
        {
            _service = service;
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<Object> Get()
        {
            return _repository.GetPosts();
        }

        [HttpPost]
        public IActionResult SubmitPost([FromBody]SubmitPostViewModel model)
        {
            _service.SubmitPost(model);
            return Ok("Post was submitted successfully.");
        }

        [HttpDelete]
        public IActionResult DeletePost([FromQuery]int id)
        {
            if (!_repository.PostExists(id))
                return StatusCode(404, "Post not found.");

            _service.DeletePost(id);
            return Ok("Deleted successfully.");
        }
    }
}
