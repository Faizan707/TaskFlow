using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.DTOs.Tasks;
using TaskFlow.Interfaces;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class TasksService : ITasks
    {
        private readonly AppDbContext _context;
        private readonly IKanbanStageService _kanbanStageService;
        private readonly INotificationService _notificationService;

        public TasksService(
            AppDbContext context,
            IKanbanStageService kanbanStageService,
            INotificationService notificationService)
        {
            _context = context;
            _kanbanStageService = kanbanStageService;
            _notificationService = notificationService;
        }

        public TaskListDto CreateTask(
            TaskCreateDto taskDto,
            int assigneeId,
            int assignedByUserId)
        {
            var assigneeExists = _context.Users.Any(u => u.Id == assigneeId);
            if (!assigneeExists)
                throw new Exception("Assignee user not found");

            _kanbanStageService.EnsureDefaultStages(taskDto.ProjectId);

            var stageId = taskDto.StageId
                ?? _context.KanbanStages
                    .Where(s => s.ProjectId == taskDto.ProjectId)
                    .OrderBy(s => s.Order)
                    .Select(s => s.Id)
                    .First();

            var task = new Tasks
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                ProjectId = taskDto.ProjectId,
                AssigneeId = assigneeId,
                StageId = stageId,
                Priority = taskDto.Priority,
                Status = Enums.TaskStatus.Todo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            _context.SaveChanges();

            _notificationService.CreateTaskAssignedNotification(
                assigneeId,
                assignedByUserId,
                task.Id,
                task.Title
            );

            return ToListDto(task.Id)!;
        }

        public List<TaskListDto> GetAllTasks()
        {
            return QueryTasks().ToList();
        }

        public List<TaskListDto> GetUserTasks(int userId)
        {
            return QueryTasks()
                .Where(t => t.AssigneeId == userId)
                .ToList();
        }

        public TaskListDto? UpdateTask(
            int taskId,
            TaskUpdateDto taskDto,
            int userId,
            string role)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId);

            if (task == null)
                return null;

            if (role == "User" && task.AssigneeId != userId)
                return null;

            var stage = _context.KanbanStages.FirstOrDefault(s =>
                s.Id == taskDto.StageId && s.ProjectId == task.ProjectId);

            if (stage == null)
                return null;

            if (taskDto.AssigneeId <= 0)
                return null;

            var assigneeExists = _context.Users.Any(u => u.Id == taskDto.AssigneeId);
            if (!assigneeExists)
                return null;

            var previousAssigneeId = task.AssigneeId;

            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.StageId = taskDto.StageId;
            task.AssigneeId = taskDto.AssigneeId;
            task.Priority = taskDto.Priority;
            task.Status = ResolveStatusForStage(stage.Name, taskDto.Status);
            task.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            if (previousAssigneeId != taskDto.AssigneeId)
            {
                _notificationService.CreateTaskAssignedNotification(
                    taskDto.AssigneeId,
                    userId,
                    task.Id,
                    task.Title
                );
            }

            return ToListDto(task.Id);
        }


        public TaskListDto? MoveTask(
            int taskId,
            int stageId,
            int userId,
            string role)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId);

            if (task == null)
                return null;

            if (role == "User" && task.AssigneeId != userId)
                return null;

            var stage = _context.KanbanStages.FirstOrDefault(s =>
                s.Id == stageId && s.ProjectId == task.ProjectId);

            if (stage == null)
                return null;

            task.StageId = stage.Id;
            task.Status = ResolveStatusForStage(stage.Name, task.Status);
            task.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return ToListDto(task.Id);
        }

        public bool DeleteTask(int taskId, int userId, string role)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId);

            if (task == null)
                return false;

            if (role == "User" && task.AssigneeId != userId)
                return false;

            _context.Tasks.Remove(task);
            _context.SaveChanges();

            return true;
        }

        private IQueryable<TaskListDto> QueryTasks()
        {
            return _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Include(t => t.Stage)
                .OrderByDescending(t => t.UpdatedAt)
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
                });
        }

        private TaskListDto? ToListDto(int taskId)
        {
            return QueryTasks().FirstOrDefault(t => t.Id == taskId);
        }

        private static Enums.TaskStatus ResolveStatusForStage(
            string stageName,
            Enums.TaskStatus fallback)
        {
            return stageName.Trim().ToLowerInvariant() switch
            {
                "todo" => Enums.TaskStatus.Todo,
                "in progress" => Enums.TaskStatus.InProgress,
                "done" => Enums.TaskStatus.Done,
                "blocked" => Enums.TaskStatus.Blocked,
                _ => fallback
            };
        }
    }
}
