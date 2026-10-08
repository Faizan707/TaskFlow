using TaskFlow.DTOs.Projects;
using TaskFlow.Models;

namespace TaskFlow.Interfaces
{
    public interface IProjectService
    {
        ProjectListDto CreateProject(Project project, int userId);
        List<ProjectListDto> GetAllProjects();
        List<ProjectListDto> GetUserProjects(int userId);
        ProjectListDto? UpdateProject(
            int projectId,
            ProjectUpdateDto projectDto,
            int userId,
            string role
        );
        bool DeleteProject(int projectId, int userId, string role);
    }
}
