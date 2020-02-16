using System;
using System.Linq;
using System.Threading.Tasks;
using fantasy_hoops.Models;
using fantasy_hoops.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using fantasy_hoops.Services;
using fantasy_hoops.Repositories;
using fantasy_hoops.Repositories.Interfaces;
using fantasy_hoops.Services.Interfaces;

namespace fantasy_hoops.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public UserController(IUserRepository repository, IUserService service)
        {
            _userRepository = repository;
            _userService = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (!_userRepository.UserExists(model.UserName))
                return StatusCode(401, "You have entered an invalid username or password!");

            if (await _userService.Login(model))
                return Ok(await _userService.RequestToken(model.UserName));
            return StatusCode(401, "You have entered an invalid username or password!");
        }

        [Authorize]
        [HttpPost("googleLogin")]
        public async Task<IActionResult> GoogleLogin()
        {
            string email = User.Claims.ToList()[4].Value;
            bool emailExists = _userRepository.EmailExists(email);

            if(!emailExists)
            {
                await _userService.GoogleRegister(User);
            }

            bool success = await _userService.GoogleLogin(User);
            if (!success)
            {
                return Unauthorized("Google login failed.");
            }

            string token = await _userService.RequestTokenByEmail(email);
            if (token == null)
                return Unauthorized("Unable to provide access token.");

            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            // Checking for duplicates usernames
            if (_userRepository.UserExists(model.UserName))
                return StatusCode(422, "Username is already taken!");

            // Check for username length
            if (!Regex.IsMatch(model.UserName, @"^.{4,11}$"))
                return StatusCode(422, "Username must be between 4 and 11 symbols long!");

            // Password validation
            if (!Regex.IsMatch(model.Password, @"^.{8,20}$"))
                return StatusCode(422, "Password must contain 8-20 characters.");

            // Checking for duplicate email addresses
            if (_userRepository.EmailExists(model.Email))
                return StatusCode(422, "Email already has an user associated to it!");

            // Check if email is valid
            if (!Regex.IsMatch(model.Email, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                return StatusCode(422, "Entered email is invalid!");

            if (await _userService.Register(model))
                return Ok("You have registered successfully!");
            return StatusCode(500, "Registration has failed!");
        }

        [Authorize]
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            _userService.Logout();
            return Ok("You have signed out successfully!");
        }

        [HttpGet("{id}")]
        public IActionResult Get(String id, int start = 0, int count = 5)
        {
            var profile = _userRepository.GetProfile(id, start, count);
            if(profile == null)
                return NotFound($"User with id {id} has not been found!");
            return Ok(profile);
        }

        [HttpGet("name/{name}")]
        public IActionResult GetByName(String name, int start = 0, int count = 0)
        {
            User user = _userRepository.GetUserByName(name);
            if (user == null)
                return NotFound($"User with name {name} has not been found!");
            return Get(user.Id, start, count);
        }

        [HttpGet("friends/{id}")]
        public IActionResult GetFriends(String id)
        {
            return Ok(_userRepository.GetFriends(id).ToList());
        }

        [HttpGet("team/{id}")]
        public IActionResult GetTeam(String id)
        {
            return Ok(_userRepository.GetTeam(id));
        }

        [HttpPut("editprofile")]
        public async Task<IActionResult> EditProfile([FromBody]EditProfileViewModel model)
        {
            // No duplicate usernames
            if (_userRepository.IsDuplicateUserName(model.Id, model.Email))
                return StatusCode(409, "Username is already taken!");

            // Check for username length
            if (!Regex.IsMatch(model.UserName, @"^.{4,11}$"))
                return StatusCode(422, "Username must be between 4 and 11 symbols long!");

            // No duplicate emails
            if (_userRepository.IsDuplicateEmail(model.Id, model.Email))
                return StatusCode(409, "Email already has an user associated to it!");

            // Check if email is valid
            if (!Regex.IsMatch(model.Email, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
                return StatusCode(422, "Entered email is invalid!");

            // Password validation
            if (model.CurrentPassword.Length > 0 && model.NewPassword.Length > 0
                && !Regex.IsMatch(model.NewPassword, @"^.{8,20}$"))
                return StatusCode(422, "Password must contain 8-20 characters.");

            User user = _userRepository.GetUser(model.Id);
            if (user == null)
                return NotFound("User has not been found!");

            if(!(await _userService.UpdateProfile(model)))
               return StatusCode(401, "Wrong current password!");

            return Ok(_userService.RequestToken(model.UserName));
        }

        [HttpPost("uploadAvatar")]
        public IActionResult UploadAvatar([FromBody]AvatarViewModel model)
        {
            if (model.Avatar == null || model.Avatar.Length < 15)
                return StatusCode(400, "Please select a file!");

            if (model.Avatar == null || model.Avatar.Length < 15)
                return StatusCode(400, "Please select a file!");

            var fileType = model.Avatar.Substring(11, 3);
            if (!(fileType.Equals("png") || fileType.Equals("jpg")))
                return StatusCode(415, "Only .png and .jpg extensions are allowed!");

            if(!_userService.UploadAvatar(model))
                return StatusCode(500, "Avatar cannot be uploaded!");

            return Ok(_userService.RequestTokenById(model.Id));
        }

        [HttpPost("clearAvatar")]
        public IActionResult ClearAvatar([FromBody]AvatarViewModel model)
        {
            if(!_userService.ClearAvatar(model))
                return StatusCode(500, "Avatar cannot be cleared!");
            return Ok("Avatar cleared successfully!");
        }

        [HttpGet]
        public IActionResult GetUserPool()
        {
            return Ok(_userRepository.GetUserPool());
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteProfile()
        {
            bool success = await _userService.DeleteProfile(User);
            if(!success)
            {
                StatusCode(500, "Unable to delete profile.");
            }

            return Ok("Profile deleted successfully.");
        }
    }
}