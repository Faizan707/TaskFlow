using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.DTOs.Notifications;
using TaskFlow.Hubs;
using TaskFlow.Interfaces;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(
            AppDbContext context,
            IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public void CreateTaskAssignedNotification(
            int assigneeId,
            int actorId,
            int taskId,
            string taskTitle)
        {
            if (assigneeId == actorId)
                return;

            var actorName = _context.Users
                .Where(u => u.Id == actorId)
                .Select(u => u.name)
                .FirstOrDefault() ?? "Someone";

            var projectId = _context.Tasks
                .Where(t => t.Id == taskId)
                .Select(t => (int?)t.ProjectId)
                .FirstOrDefault();

            var notification = new Notification
            {
                UserId = assigneeId,
                ActorId = actorId,
                TaskId = taskId,
                Message = $"{actorName} assigned you the task \"{taskTitle}\"",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();

            var payload = new NotificationListDto
            {
                Id = notification.Id,
                Message = notification.Message,
                ActorName = actorName,
                TaskTitle = taskTitle,
                TaskId = taskId,
                ProjectId = projectId,
                IsRead = false,
                CreatedAt = notification.CreatedAt
            };

            // Real-time push to assignee's SignalR group
            _hubContext.Clients
                .Group(NotificationHub.UserGroup(assigneeId))
                .SendAsync("ReceiveNotification", payload);
        }

        public List<NotificationListDto> GetUserNotifications(int userId)
        {
            return _context.Notifications
                .Include(n => n.Actor)
                .Include(n => n.Task)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(30)
                .Select(n => new NotificationListDto
                {
                    Id = n.Id,
                    Message = n.Message,
                    ActorName = n.Actor.name,
                    TaskTitle = n.Task != null ? n.Task.Title : null,
                    TaskId = n.TaskId,
                    ProjectId = n.Task != null ? n.Task.ProjectId : null,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToList();
        }

        public int GetUnreadCount(int userId)
        {
            return _context.Notifications.Count(n =>
                n.UserId == userId && !n.IsRead);
        }

        public bool MarkAsRead(int notificationId, int userId)
        {
            var notification = _context.Notifications.FirstOrDefault(n =>
                n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return false;

            notification.IsRead = true;
            _context.SaveChanges();
            return true;
        }

        public int MarkAllAsRead(int userId)
        {
            var unread = _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToList();

            foreach (var notification in unread)
                notification.IsRead = true;

            _context.SaveChanges();
            return unread.Count;
        }
    }
}
