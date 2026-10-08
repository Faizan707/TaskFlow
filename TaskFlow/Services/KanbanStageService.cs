using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.DTOs.Kanban;
using TaskFlow.DTOs.Tasks;
using TaskFlow.Interfaces;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class KanbanStageService : IKanbanStageService
    {
        private readonly AppDbContext _context;

        public KanbanStageService(AppDbContext context)
        {
            _context = context;
        }

        public void EnsureDefaultStages(int projectId)
        {
            var hasStages = _context.KanbanStages.Any(s => s.ProjectId == projectId);
            if (hasStages)
                return;

            var defaults = new[]
            {
                new KanbanStage { Name = "Todo", Order = 1, IsDefault = true, ProjectId = projectId },
                new KanbanStage { Name = "In Progress", Order = 2, IsDefault = true, ProjectId = projectId },
                new KanbanStage { Name = "Done", Order = 3, IsDefault = true, ProjectId = projectId }
            };

            _context.KanbanStages.AddRange(defaults);
            _context.SaveChanges();
        }

        public List<KanbanStageListDto> GetProjectStages(int projectId)
        {
            EnsureDefaultStages(projectId);

            var stages = _context.KanbanStages
                .Where(s => s.ProjectId == projectId)
                .OrderBy(s => s.Order)
                .ToList();

            var tasks = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .Include(t => t.Stage)
                .Where(t => t.ProjectId == projectId)
                .OrderByDescending(t => t.UpdatedAt)
                .ToList();

            return stages.Select(s => new KanbanStageListDto
            {
                Id = s.Id,
                Name = s.Name,
                Order = s.Order,
                IsDefault = s.IsDefault,
                ProjectId = s.ProjectId,
                Tasks = tasks
                    .Where(t => t.StageId == s.Id)
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
            }).ToList();
        }

        public KanbanStageListDto CreateStage(KanbanStageCreateDto dto)
        {
            EnsureDefaultStages(dto.ProjectId);

            var maxOrder = _context.KanbanStages
                .Where(s => s.ProjectId == dto.ProjectId)
                .Select(s => (int?)s.Order)
                .Max() ?? 0;

            var stage = new KanbanStage
            {
                Name = dto.Name,
                Order = maxOrder + 1,
                IsDefault = false,
                ProjectId = dto.ProjectId
            };

            _context.KanbanStages.Add(stage);
            _context.SaveChanges();

            return new KanbanStageListDto
            {
                Id = stage.Id,
                Name = stage.Name,
                Order = stage.Order,
                IsDefault = stage.IsDefault,
                ProjectId = stage.ProjectId,
                Tasks = new List<TaskListDto>()
            };
        }

        public KanbanStageListDto? UpdateStage(int stageId, KanbanStageUpdateDto dto)
        {
            var stage = _context.KanbanStages.FirstOrDefault(s => s.Id == stageId);
            if (stage == null)
                return null;

            stage.Name = dto.Name;
            stage.Order = dto.Order;
            _context.SaveChanges();

            return new KanbanStageListDto
            {
                Id = stage.Id,
                Name = stage.Name,
                Order = stage.Order,
                IsDefault = stage.IsDefault,
                ProjectId = stage.ProjectId,
                Tasks = new List<TaskListDto>()
            };
        }

        public bool DeleteStage(int stageId)
        {
            var stage = _context.KanbanStages.FirstOrDefault(s => s.Id == stageId);
            if (stage == null)
                return false;

            if (stage.IsDefault)
                return false;

            var hasTasks = _context.Tasks.Any(t => t.StageId == stageId);
            if (hasTasks)
                return false;

            _context.KanbanStages.Remove(stage);
            _context.SaveChanges();
            return true;
        }
    }
}
