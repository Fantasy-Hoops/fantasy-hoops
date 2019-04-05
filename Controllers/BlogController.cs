using fantasy_hoops.Database;
using fantasy_hoops.Models.ViewModels;
using fantasy_hoops.Repositories;
using fantasy_hoops.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly BlogService _service;
        private readonly BlogRepository _repository;

        public BlogController()
        {
            GameContext context = new GameContext();
            _service = new BlogService(context);
            _repository = new BlogRepository(context);
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
