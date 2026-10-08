using TaskFlow.Enums;

namespace TaskFlow.DTOs.Tasks
{
    public class TaskListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int AssigneeId { get; set; }
        public string AssigneeName { get; set; } = string.Empty;
        public int StageId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; }
        public Enums.TaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
