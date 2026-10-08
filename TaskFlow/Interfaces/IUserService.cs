using TaskFlow.DTOs.Users;
using TaskFlow.Models;

namespace TaskFlow.Interfaces
{
    public interface IUserService
    {
        Users CreateUser(Users user);
        Users? ValidateLogin(LoginDtos login);
        List<UserListDto> GetAllUsers();
        List<AssigneeDto> GetAssignees();
        UserListDto? UpdateUserRole(int userId, string role);
    }
}

