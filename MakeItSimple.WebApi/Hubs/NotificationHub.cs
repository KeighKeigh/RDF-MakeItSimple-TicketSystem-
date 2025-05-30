//using MakeItSimple.WebApi.Common.Caching;
//using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;

//namespace MakeItSimple.WebApi.Hubs
//{
//    public class NotificationHub : Hub
//    {
//        private readonly IHubCaller _hubCaller;
//        private readonly ICacheService _cacheService;
//        private readonly MisDbContext _context;

//        public NotificationHub(IHubCaller hubCaller, ICacheService cacheService, MisDbContext context)
//        {
//            _hubCaller = hubCaller;
//            _cacheService = cacheService;
//            _context = context;
//        }

//        public override async Task OnConnectedAsync()
//        {

//            if (Context.User.Identity is ClaimsIdentity identity &&
//                Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
//            {
//                var user = await _context.Users.FindAsync(userId);

//                var userChannelIds = await _context.ChannelUsers
//                                    .Where(x => x.UserId == userId)
//                                    .Select(x => x.ChannelId)
//                                    .ToListAsync();

//                foreach (var channelId in userChannelIds)
//                {
//                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Channel_{channelId}");
//                }
//                if (user == null || userChannelIds == null)
//                {
//                    await base.OnConnectedAsync();
//                    return;
//                }

//                //var groupId = user.GroupId;
//                //var groupName = $"Group_{groupId}";

//                // Add connection to group
//                //await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
//                await _hubCaller.AddUserToGroupAsync(Context.ConnectionId, userId);

//                var opentickets = await _cacheService.GetOpenTickets();
//                //var openticketchannel = await _cacheService.GetOpenTicketsChannel();
//                //var closedtickets = await _cacheService.GetClosingTickets();
//                //var transfertickets = await _cacheService.GetTransferTicketConcerns();
//                //var onholdticket = await _cacheService.GetTicketOnHolds();

//                var useropentickets = opentickets.Where(t => t.UserId == userId).ToList();
//                //var channelopenticket = openticketchannel.Where(t => userChannelIds.Contains(t.RequestConcern.ChannelId.Value));
//                //var userclosedtickets = closedtickets.Where(t => t.AddedBy == userId).ToList();
//                //var usertransfertickets = transfertickets.Where(t => t.TransferTo == userId).ToList();
//                //var useronholdtickets = onholdticket.Where(t => t.AddedBy == userId).ToList();

//                await _hubCaller.SendNotificationAsync(userId, "OpenTickets", useropentickets);
//                //await _hubCaller.SendNotificationAsync(userId, "ChannelOpenTickets", channelopenticket);
//                //await _hubCaller.SendNotificationAsync(userId, "ClosedTickets", userclosedtickets);
//                //await _hubCaller.SendNotificationAsync(userId, "TransferTickets", usertransfertickets);
//                //await _hubCaller.SendNotificationAsync(userId, "OnHoldTickets", useronholdtickets);

//            }
//            await base.OnConnectedAsync();
//        }

//        public override async Task OnDisconnectedAsync(Exception exception)
//        {
//            if (Context.User.Identity is ClaimsIdentity identity &&
//                Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
//            {
//                await _hubCaller.RemoveUserFromGroupAsync(Context.ConnectionId, userId);
//            }

//            await base.OnDisconnectedAsync(exception);
//        }

//        public async Task SendNotificationToUser(string userId, string notificationType, string message)
//        {
//            if (!string.IsNullOrEmpty(userId))
//            {
//                await _hubCaller.SendNotificationAsync(Guid.Parse(userId), notificationType, message);
//            }
//        }

//    }
//}
