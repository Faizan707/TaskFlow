using TaskFlow.Enums;

namespace TaskFlow.DTOs.Tasks
{
    public class TaskUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int StageId { get; set; }
        public int AssigneeId { get; set; }
        public TaskPriority Priority { get; set; }
        public Enums.TaskStatus Status { get; set; }
    }
}

