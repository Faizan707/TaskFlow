using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs;
using TaskFlow.Models;
using TaskFlow.Interfaces;

namespace TaskFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UserController(
            IUserService userService,
            IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public IActionResult CreateUser(Users users)
        {
            var user = _userService.CreateUser(users);

            return Ok(new
            {
                message = "User created successfully",
                name = user.name,
                email = user.email
            });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDtos login)
        {
            var user = _userService.ValidateLogin(login);

            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                message = "Login successful",
                token = token
            });
        }
    }
}