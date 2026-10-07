using TaskFlow.Data;
using TaskFlow.DTOs;
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
       
    }
}