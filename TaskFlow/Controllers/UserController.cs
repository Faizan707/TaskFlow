using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs.Users;
using TaskFlow.Interfaces;
using TaskFlow.Models;

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

        // Admin: see all users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        // Admin: change a user's role
        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}/role")]
        public IActionResult UpdateUserRole(int userId, UpdateRoleDto dto)
        {
            try
            {
                var user = _userService.UpdateUserRole(userId, dto.Role);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(new
                {
                    message = "Role updated successfully",
                    user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
