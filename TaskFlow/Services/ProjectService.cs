using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.DTOs.Projects;
using TaskFlow.Interfaces;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        private readonly IKanbanStageService _kanbanStageService;

        public ProjectService(AppDbContext context, IKanbanStageService kanbanStageService)
        {
            _context = context;
            _kanbanStageService = kanbanStageService;
        }

        public ProjectListDto CreateProject(Project project, int userId)
        {
            project.UserId = userId;
            project.CreatedAt = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;

            _context.Project.Add(project);
            _context.SaveChanges();

            // Default kanban: Todo, In Progress, Done
            _kanbanStageService.EnsureDefaultStages(project.Id);

            return ToListDto(project.Id)!;
        }

        public List<ProjectListDto> GetAllProjects()
        {
            return _context.Project
                .Include(p => p.user)
                .OrderByDescending(p => p.UpdatedAt)
                .Select(p => new ProjectListDto
                {
                    Id = p.Id,
                    Name = p.name,
                    Description = p.description,
                    UserId = p.UserId,
                    CreatedBy = p.user.name,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToList();
        }

        public List<ProjectListDto> GetUserProjects(int userId)
        {
            return _context.Project
                .Include(p => p.user)
                .Where(x => x.UserId == userId)
                .OrderByDescending(p => p.UpdatedAt)
                .Select(p => new ProjectListDto
                {
                    Id = p.Id,
                    Name = p.name,
                    Description = p.description,
                    UserId = p.UserId,
                    CreatedBy = p.user.name,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToList();
        }

        public ProjectListDto? UpdateProject(
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

            return ToListDto(project.Id);
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

        private ProjectListDto? ToListDto(int projectId)
        {
            return _context.Project
                .Include(p => p.user)
                .Where(p => p.Id == projectId)
                .Select(p => new ProjectListDto
                {
                    Id = p.Id,
                    Name = p.name,
                    Description = p.description,
                    UserId = p.UserId,
                    CreatedBy = p.user.name,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .FirstOrDefault();
        }
    }
}
