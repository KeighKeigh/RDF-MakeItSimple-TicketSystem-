using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Hubs;
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
        private readonly IHubContext<CacheNotificationHub> _hubContext;

        public CacheService(IDistributedCache distributedCache,
            IMemoryCache cache,
            MisDbContext context,
            IHubContext<CacheNotificationHub> hubContext
            )
        {
            _distributedCache = distributedCache;
            _cache = cache;
            _context = context;
            _hubContext = hubContext;

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

        //public async Task CacheAllAsync()
        //{
        //    var ticketOnHold = await _context.TicketOnHolds.Where(x => x.IsActive == true && x.IsHold == true).ToListAsync();
        //    _cache.Set("Ticket On Hold", ticketOnHold);

        //    var closingTicket = await _context.ClosingTickets.Where(x => x.IsClosing == true && x.IsActive == true).ToListAsync();
        //    _cache.Set("Closed Tickets", closingTicket);

        //var openTicket = await _context.TicketConcerns.Where(x => x.OnHold == null && x.IsActive == true
        //&& x.IsApprove == true && x.IsTransfer != true && x.IsClosedApprove != true).ToListAsync();
        //    _cache.Set("Open Tickets", openTicket);

        //    var transferTicket = await _context.TransferTicketConcerns.Where(x => x.IsActive == true && x.IsTransfer == true).ToListAsync();
        //    _cache.Set("Transfer Tickets", transferTicket);

        //    var requestConcern = await _context.RequestConcerns.Where(x => x.IsActive).ToListAsync();
        //    _cache.Set("Request Concern Tickets", requestConcern);
        //}

        //public List<TicketOnHold> GetTicketOnHolds()
        //{
        //    return _cache.TryGetValue("Ticket On Hold", out List<TicketOnHold> ticketOnHold)
        //        ? ticketOnHold : new List<TicketOnHold>();
        //}

        //public List<ClosingTicket> GetClosingTickets()
        //{
        //    return _cache.TryGetValue("Closed Tickets", out List<ClosingTicket> closingTicket)
        //        ? closingTicket : new List<ClosingTicket>();
        //}

        //public List<TicketConcern> GetOpenTickets()
        //{
        //    return _cache.TryGetValue("Open Tickets", out List<TicketConcern> openTicket)
        //        ? openTicket : new List<TicketConcern>();
        //}

        //public List<TransferTicketConcern> GetTransferTicketConcerns()
        //{
        //    return _cache.TryGetValue("Transfer Tickets", out List<TransferTicketConcern> transferTicket)
        //        ? transferTicket : new List<TransferTicketConcern>();
        //}

        //public List<RequestConcern> GetRequestConcerns()
        //{
        //    return _cache.TryGetValue("Ticket On Hold", out List<RequestConcern> requestTicket)
        //        ? requestTicket : new List<RequestConcern>();
        //}

        public async Task<List<TicketOnHold>> GetTicketOnHolds()
        {
            string cacheKey = "Watduheyyylll";

            if (!_cache.TryGetValue(cacheKey, out List<TicketOnHold> data))
            {
                data = await _context.TicketOnHolds.Where(x => x.IsActive == true && x.IsHold == true).ToListAsync();

                _cache.Set(cacheKey, data);

                await _hubContext.Clients.All.SendAsync("ReceiveCacheUpdate", cacheKey, data);
            }
            return data;
        }

        public async Task<List<ClosingTicket>> GetClosingTickets()
        {
            string cacheKey = "Neighgha";

            if (!_cache.TryGetValue(cacheKey, out List<ClosingTicket> data))
            {
                data = await _context.ClosingTickets.Where(x => x.IsClosing == true && x.IsActive == true).ToListAsync();

                _cache.Set(cacheKey, data);

                await _hubContext.Clients.All.SendAsync("ReceiveCacheUpdate", cacheKey, data);
            }
            return data;
        }

        public async Task<List<TicketConcern>> GetOpenTickets()
        {
            string cacheKey = "chinchangchong";

            if (!_cache.TryGetValue(cacheKey, out List<TicketConcern> data))
            {
                data = await _context.TicketConcerns.Where(x => x.OnHold == null && x.IsActive == true
                       && x.IsApprove == true && x.IsTransfer != true && x.IsClosedApprove != true).ToListAsync();

                _cache.Set(cacheKey, data);

                await _hubContext.Clients.All.SendAsync("ReceiveCacheUpdate", cacheKey, data);
            }
            return data;
        }

        public async Task<List<TransferTicketConcern>> GetTransferTicketConcerns()
        {
            string cacheKey = "bisakLOL";

            if (!_cache.TryGetValue(cacheKey, out List<TransferTicketConcern> data))
            {
                data = await _context.TransferTicketConcerns.Where(x => x.IsActive == true && x.IsTransfer == true).ToListAsync();

                _cache.Set(cacheKey, data);

                await _hubContext.Clients.All.SendAsync("ReceiveCacheUpdate", cacheKey, data);
            }
            return data;
        }

        public async Task<List<RequestConcern>> GetRequestConcerns()
        {
            string cacheKey = "Nyenyenyenye";

            if (!_cache.TryGetValue(cacheKey, out List<RequestConcern> data))
            {
                data = await _context.RequestConcerns.Where(x => x.IsActive).ToListAsync();

                _cache.Set(cacheKey, data);

                await _hubContext.Clients.All.SendAsync("ReceiveCacheUpdate", cacheKey, data);
            }
            return data;
        }
    }
}
