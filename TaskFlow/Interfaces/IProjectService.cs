using TaskFlow.DTOs;
using TaskFlow.Models;

namespace TaskFlow.Interfaces
{
    public interface IProjectService
    {
        Project CreateProject(Project project, int userId);
        List<Project> GetAllProjects();
        List<Project> GetUserProjects(int userId);
        Project? UpdateProject(
            int projectId,
            ProjectUpdateDto projectDto,
            int userId,
            string role
        );
        bool DeleteProject(int projectId, int userId, string role);
    }
}
