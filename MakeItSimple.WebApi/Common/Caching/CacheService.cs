using MakeItSimple.WebApi.Common.Caching.CacheDto;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;

using MakeItSimple.WebApi.Models.Setup.ChannelUserSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace MakeItSimple.WebApi.Common.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _cache;
        private readonly MisDbContext _context;
        //private readonly IHubContext<NotificationHub> _hubContext;
        //private readonly IHubCaller _hubCaller;

        public CacheService(IDistributedCache distributedCache,
            IMemoryCache cache,
            MisDbContext context
            //IHubContext<NotificationHub> hubContext,
            //IHubCaller hubCaller
            )
        {
            _distributedCache = distributedCache;
            _cache = cache;
            _context = context;
            //_hubContext = hubContext;
            //_hubCaller = hubCaller;
        }

        public async Task SetCacheAsync(string key, object value, TimeSpan expiration)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });
        }

        public async Task<object> GetCacheAsync(string key)
        {
            var json = await _distributedCache.GetStringAsync(key);
            return json == null ? null : System.Text.Json.JsonSerializer.Deserialize<object>(json);
        }

        public async Task RemoveCacheAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }


        public async Task<List<TicketOnHold>> GetTicketOnHolds()
        {
            string cacheKey = "TicketOnHoldCache";

            if (!_cache.TryGetValue(cacheKey, out List<TicketOnHold> data))
            {
                data = await _context.TicketOnHolds.Where(x => x.IsActive == true && x.IsHold == true).ToListAsync();

                _cache.Set(cacheKey, data);


            }
            return data;
        }

        public async Task<List<ClosingTicket>> GetClosingTickets()
        {
            string cacheKey = "ClosingTicketCache";

            if (!_cache.TryGetValue(cacheKey, out List<ClosingTicket> data))
            {
                data = await _context.ClosingTickets.Where(x => x.IsClosing == true && x.IsActive == true).ToListAsync();

                _cache.Set(cacheKey, data);


            }
            return data;
        }


        //OpenTickets
        public async Task<List<TicketConcern>> GetOpenTickets()
        {
            string cacheKey = "OpenTicketCache";

            if (!_cache.TryGetValue(cacheKey, out List<TicketConcern> data))
            {
                data = await _context.TicketConcerns.Where(x => x.ConcernStatus == "Pending").ToListAsync();



                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(cacheKey, data, cacheEntryOptions);

            }
            return data;
        }


        public void UpdateOpenTicketCacheAsync(List<TicketConcern> updateTickets)
        {

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            _cache.Set("UpdateTicketCache", updateTickets, cacheEntryOptions);
        }




        public async Task<List<TransferTicketConcern>> GetTransferTicketConcerns()
        {
            string cacheKey = "TransferTicketCache";

            if (!_cache.TryGetValue(cacheKey, out List<TransferTicketConcern> data))
            {
                data = await _context.TransferTicketConcerns.Where(x => x.IsActive == true && x.IsTransfer == true).ToListAsync();

                _cache.Set(cacheKey, data);

            }
            return data;
        }

        public async Task<List<RequestConcern>> GetRequestConcerns()
        {
            string cacheKey = "RequestConcernCache";

            if (!_cache.TryGetValue(cacheKey, out List<RequestConcern> data))
            {
                data = await _context.RequestConcerns.Where(x => x.IsActive).ToListAsync();

                _cache.Set(cacheKey, data);

            }
            return data;
        }
    }
}
