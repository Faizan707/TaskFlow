using TaskFlow.Enums;

namespace TaskFlow.Models
{
    public class Tasks
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int AssigneeId { get; set; }
        public Users Assignee { get; set; }

        public int StageId { get; set; }
        public KanbanStage Stage { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Todo;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
