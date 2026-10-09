namespace TaskFlow.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public Users User { get; set; } = null!;

        public int ActorId { get; set; }
        public Users Actor { get; set; } = null!;

        public int? TaskId { get; set; }
        public Tasks? Task { get; set; }

        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
