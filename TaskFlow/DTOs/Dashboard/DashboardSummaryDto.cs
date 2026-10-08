using TaskFlow.DTOs.Tasks;

namespace TaskFlow.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int TodoCount { get; set; }
        public int InProgressCount { get; set; }
        public int DoneCount { get; set; }
        public int BlockedCount { get; set; }
        public int LowPriorityCount { get; set; }
        public int MediumPriorityCount { get; set; }
        public int HighPriorityCount { get; set; }
        public int CriticalPriorityCount { get; set; }
        public int? TotalUsers { get; set; }
        public List<TaskListDto> RecentTasks { get; set; } = new();
    }
}
