using TaskFlow.DTOs.Notifications;

namespace TaskFlow.Interfaces
{
    public interface INotificationService
    {
        void CreateTaskAssignedNotification(
            int assigneeId,
            int actorId,
            int taskId,
            string taskTitle
        );

        List<NotificationListDto> GetUserNotifications(int userId);
        int GetUnreadCount(int userId);
        bool MarkAsRead(int notificationId, int userId);
        int MarkAllAsRead(int userId);
    }
}
