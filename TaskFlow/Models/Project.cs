namespace TaskFlow.Models
{
    public class Project
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Users user { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
