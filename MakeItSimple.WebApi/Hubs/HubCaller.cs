using Microsoft.AspNetCore.SignalR;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace MakeItSimple.WebApi.Hubs
{
    public interface IHubCaller
    {
        Task SendNotificationAsync(Guid userId, string notificationType, object message);
        Task AddUserToGroupAsync(string connectionId, Guid userId);
        Task RemoveUserFromGroupAsync(string connectionId, Guid userId);
        Task BroadcastClientInfoAsync(string clientId, Guid userId, string message, string notificationType);
    }


    public class HubCaller : IHubCaller
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public HubCaller(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(Guid userId, string notificationType, object message)
        {
            await _hubContext.Clients.Group(userId.ToString()).SendAsync(notificationType, message);
        }

        public async Task AddUserToGroupAsync(string connectionId, Guid userId)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, userId.ToString());
        }

        public async Task RemoveUserFromGroupAsync(string connectionId, Guid userId)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, userId.ToString());
        }

        public async Task BroadcastClientInfoAsync(string clientId, Guid userId, string message, string notificationType)
        {
            await _hubContext.Clients.All.SendAsync(notificationType, new
            {
                ClientId = clientId,
                UserId = userId,
                Message = message
            });
        }

    }
}
