namespace TaskFlow.DTOs.Kanban
{
    public class KanbanStageUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
