using TaskFlow.DTOs;
using TaskFlow.Models;

namespace TaskFlow.Interfaces
{
    public interface IUserService
    {
        Users CreateUser(Users user);
        Users? ValidateLogin(LoginDtos login);
    }
}
