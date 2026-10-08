namespace TaskFlow.DTOs.Kanban
{
    public class KanbanStageCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int ProjectId { get; set; }
    }
}
