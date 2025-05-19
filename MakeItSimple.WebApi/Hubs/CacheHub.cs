//using Microsoft.AspNetCore.SignalR;

//namespace MakeItSimple.WebApi.Hubs
//{
//    public class CacheHub : Hub 
//    {
//        private readonly ICacheNotificationHub _cacheNotificationHub;

//        public CacheHub(ICacheNotificationHub cacheNotificationHub)
//        {
//            _cacheNotificationHub = cacheNotificationHub;
//        }

//        public async Task BroadcastClientInfo(string clientId, string userId, string message, string notificationType)
//        {
//            await _cacheNotificationHub.BroadcastCacheAllAsync(clientId, Guid.Parse(userId), message, notificationType);
//        }
//    }
//}
