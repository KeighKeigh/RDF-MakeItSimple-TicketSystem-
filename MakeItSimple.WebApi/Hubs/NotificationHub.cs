using MakeItSimple.WebApi.Common.Caching;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MakeItSimple.WebApi.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly IHubCaller _hubCaller;
        private readonly ICacheService _cacheService;

        public NotificationHub(IHubCaller hubCaller, ICacheService cacheService)
        {
            _hubCaller = hubCaller;
            _cacheService = cacheService;
        }

        public override async Task OnConnectedAsync()
        {

            if (Context.User.Identity is ClaimsIdentity identity &&
                Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
            {
                await _hubCaller.AddUserToGroupAsync(Context.ConnectionId, userId);

                var opentickets = await _cacheService.GetOpenTickets();
                var closedtickets = await _cacheService.GetClosingTickets();
                var transfertickets = await _cacheService.GetTransferTicketConcerns();
                var onholdticket = await _cacheService.GetTicketOnHolds();

                var useropentickets = opentickets.Where(t => t.UserId == userId).ToList();
                var userclosedtickets = closedtickets.Where(t => t.AddedBy == userId).ToList();
                var usertransfertickets = transfertickets.Where(t => t.TransferTo == userId).ToList();
                var useronholdtickets = onholdticket.Where(t => t.AddedBy == userId).ToList();

                await _hubCaller.SendNotificationAsync(userId, "OpenTickets", useropentickets);
                await _hubCaller.SendNotificationAsync(userId, "ClosedTickets", userclosedtickets);
                await _hubCaller.SendNotificationAsync(userId, "TransferTickets", usertransfertickets);
                await _hubCaller.SendNotificationAsync(userId, "OnHoldTickets", useronholdtickets);

            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity is ClaimsIdentity identity &&
                Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
            {
                await _hubCaller.RemoveUserFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotificationToUser(string userId, string notificationType, string message)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                await _hubCaller.SendNotificationAsync(Guid.Parse(userId), notificationType, message);
            }
        }

        


    }
}
