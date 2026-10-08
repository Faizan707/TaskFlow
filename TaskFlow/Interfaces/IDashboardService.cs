using TaskFlow.DTOs.Dashboard;

namespace TaskFlow.Interfaces
{
    public interface IDashboardService
    {
        DashboardSummaryDto GetSummary(int userId, string role);
    }
}
