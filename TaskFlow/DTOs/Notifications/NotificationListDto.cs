namespace TaskFlow.DTOs.Notifications
{
    public class NotificationListDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ActorName { get; set; } = string.Empty;
        public string? TaskTitle { get; set; }
        public int? TaskId { get; set; }
        public int? ProjectId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
