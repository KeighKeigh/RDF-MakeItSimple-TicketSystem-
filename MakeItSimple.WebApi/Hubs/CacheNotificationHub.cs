//using Microsoft.AspNetCore.SignalR;

//namespace MakeItSimple.WebApi.Hubs
//{
//    public interface ICacheNotificationHub
//    {
//        //Task OnConnectedAsync();
//        //Task OnDisconnectedAsync(Exception? exception);
//        Task BroadcastCacheAllAsync(string clientId, Guid userId, string message, string notificationType);
//    }

//    public class CacheNotificationHub : ICacheNotificationHub
//    {
//        private readonly IHubContext<CacheHub> _cacheNotificationHub;

//        public CacheNotificationHub(IHubContext<CacheHub> cacheNotificationHub)
//        {
//            _cacheNotificationHub = cacheNotificationHub;
//        }
//        //public override async Task OnConnectedAsync()
//        //{
//        //    Console.WriteLine($"Client connected: {Context.ConnectionId}");
//        //    await base.OnConnectedAsync();
//        //}

//        //public override async Task OnDisconnectedAsync(Exception? exception)
//        //{
//        //    Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
//        //    await base.OnDisconnectedAsync(exception);
//        //}

//        public async Task BroadcastCacheAllAsync(string clientId, Guid userId, string message, string notificationType)
//        {
//            await _cacheNotificationHub.Clients.All.SendAsync(notificationType, new
//            {
//                ClientId = clientId,
//                UserId = userId,
//                Message = message
//            });
//        }
//    }
//}
