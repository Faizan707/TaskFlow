using TaskFlow.Data;
using TaskFlow.DTOs.Users;
using TaskFlow.Interfaces;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public Users CreateUser(Users user)

        {
            var existingUser = _context.Users
       .FirstOrDefault(x => x.email == user.email);

            if (existingUser != null)
            {
                throw new Exception("Email already exists");
            }
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password);

            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public Users? ValidateLogin(LoginDtos login)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.email == login.Email);

            if (user == null)
                return null;

            bool passwordValid = BCrypt.Net.BCrypt.Verify(
                login.Password,
                user.password
            );

            if (!passwordValid)
                return null;

            return user;
        }

        public List<UserListDto> GetAllUsers()
        {
            return _context.Users
                .OrderBy(u => u.Id)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Name = u.name,
                    Email = u.email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .ToList();
        }

        public UserListDto? UpdateUserRole(int userId, string role)
        {
            var allowedRoles = new[] { "User", "Manager", "Admin" };

            if (!allowedRoles.Contains(role))
                throw new Exception("Invalid role. Allowed roles: User, Manager, Admin");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return null;

            user.Role = role;
            user.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();

            return new UserListDto
            {
                Id = user.Id,
                Name = user.name,
                Email = user.email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
