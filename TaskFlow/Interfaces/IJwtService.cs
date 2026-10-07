using TaskFlow.Models;

namespace TaskFlow.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Users user);
    }
}
