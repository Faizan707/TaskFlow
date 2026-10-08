namespace TaskFlow.Models
{
    public class KanbanStage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsDefault { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
