using TaskFlow.Enums;

namespace TaskFlow.DTOs.Tasks
{
    public class TaskCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public int? StageId { get; set; }
        public int AssigneeId { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    }
}

