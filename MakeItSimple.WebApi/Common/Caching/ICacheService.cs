using MakeItSimple.WebApi.Common.Caching.CacheDto;
using MakeItSimple.WebApi.Models.Ticketing;

namespace MakeItSimple.WebApi.Common.Caching
{
    public interface ICacheService
    {
        Task SetCacheAsync(string key, object value, TimeSpan expiration);
        Task<object> GetCacheAsync(string key);
        Task RemoveCacheAsync(string key);

        //Task CacheAllAsync();
        Task<List<TicketOnHold>> GetTicketOnHolds();
        Task<List<ClosingTicket>> GetClosingTickets();

        Task<List<TicketConcern>> GetOpenTickets();
        //Task<List<TicketConcern>> GetOpenTicketsChannel();
        //Task UpdateOpenTicketCacheAsync();

        Task<List<TransferTicketConcern>> GetTransferTicketConcerns();
        Task<List<RequestConcern>> GetRequestConcerns();
    }
}
