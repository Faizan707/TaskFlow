using TaskFlow.DTOs.Tasks;

namespace TaskFlow.DTOs.Kanban
{
    public class KanbanStageListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsDefault { get; set; }
        public int ProjectId { get; set; }
        public List<TaskListDto> Tasks { get; set; } = new();
    }
}
