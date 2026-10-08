using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.DTOs.Dashboard;
using TaskFlow.DTOs.Tasks;
using TaskFlow.Enums;
using TaskFlow.Interfaces;

namespace TaskFlow.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public DashboardSummaryDto GetSummary(int userId, string role)
        {
            var isStaff = role == "Admin" || role == "Manager";

            var projectsQuery = _context.Project.AsQueryable();
            if (!isStaff)
                projectsQuery = projectsQuery.Where(p => p.UserId == userId);

            var tasksQuery = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Include(t => t.Stage)
                .AsQueryable();

            if (!isStaff)
                tasksQuery = tasksQuery.Where(t => t.AssigneeId == userId);

            var tasks = tasksQuery.ToList();

            return new DashboardSummaryDto
            {
                TotalProjects = projectsQuery.Count(),
                TotalTasks = tasks.Count,
                TodoCount = tasks.Count(t => t.Status == Enums.TaskStatus.Todo),
                InProgressCount = tasks.Count(t => t.Status == Enums.TaskStatus.InProgress),
                DoneCount = tasks.Count(t => t.Status == Enums.TaskStatus.Done),
                BlockedCount = tasks.Count(t => t.Status == Enums.TaskStatus.Blocked),
                LowPriorityCount = tasks.Count(t => t.Priority == TaskPriority.Low),
                MediumPriorityCount = tasks.Count(t => t.Priority == TaskPriority.Medium),
                HighPriorityCount = tasks.Count(t => t.Priority == TaskPriority.High),
                CriticalPriorityCount = tasks.Count(t => t.Priority == TaskPriority.Critical),
                TotalUsers = isStaff ? _context.Users.Count() : null,
                RecentTasks = tasks
                    .OrderByDescending(t => t.UpdatedAt)
                    .Take(8)
                    .Select(t => new TaskListDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        ProjectId = t.ProjectId,
                        ProjectName = t.Project.name,
                        AssigneeId = t.AssigneeId,
                        AssigneeName = t.Assignee.name,
                        StageId = t.StageId,
                        StageName = t.Stage.Name,
                        Priority = t.Priority,
                        Status = t.Status,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt
                    })
                    .ToList()
            };
        }
    }
}
