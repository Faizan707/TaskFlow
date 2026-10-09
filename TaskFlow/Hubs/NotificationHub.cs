using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TaskFlow.Hubs
{
    // Clients join group "user-{userId}" so we can push to one user.
    [Authorize]
    public class NotificationHub : Hub
    {
        public static string UserGroup(int userId) => $"user-{userId}";

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("userId")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    UserGroup(int.Parse(userId))
                );
            }

            await base.OnConnectedAsync();
        }
    }
}
