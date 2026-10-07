using TaskFlow.Data;
using TaskFlow.DTOs.Projects;
using TaskFlow.Interfaces;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService(AppDbContext context)
        {
            _context = context;
        }

        public Project CreateProject(Project project, int userId)
        {
            project.UserId = userId;
            project.CreatedAt = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;

            _context.Project.Add(project);
            _context.SaveChanges();

            return project;
        }

        public List<Project> GetAllProjects()
        {
            return _context.Project.ToList();
        }

        public List<Project> GetUserProjects(int userId)
        {
            return _context.Project
                .Where(x => x.UserId == userId)
                .ToList();
        }

        public Project? UpdateProject(
            int projectId,
            ProjectUpdateDto projectDto,
            int userId,
            string role)
        {
            var project = _context.Project
                .FirstOrDefault(x => x.Id == projectId);

            if (project == null)
                return null;

            // Normal User can only update their own project
            if (role == "User" && project.UserId != userId)
                return null;

            project.name = projectDto.name;
            project.description = projectDto.description;
            project.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return project;
        }
        public bool DeleteProject(int projectId, int userId, string role)
        {
            var project = _context.Project
                .FirstOrDefault(x => x.Id == projectId);

            if (project == null)
                return false;

            // User can delete only their own project
            if (role == "User" && project.UserId != userId)
                return false;

            _context.Project.Remove(project);
            _context.SaveChanges();

            return true;
        }
    }
}