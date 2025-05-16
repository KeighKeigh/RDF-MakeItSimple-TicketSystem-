//using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
//using MakeItSimple.WebApi.Models.Ticketing;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Memory;

//namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.CacheService
//{
//    public class CacheService : ICacheService
//    {

//        private readonly IMemoryCache _cache;
//        private readonly MisDbContext _context;

//        public CacheService(IMemoryCache cache, MisDbContext context)
//        {
//            _cache = cache;
//            _context = context;
//        }


//        public async Task CacheAllAsync()
//        {
//            var ticketOnHold = await _context.TicketOnHolds.Where(x => x.IsActive == true && x.IsHold == true).ToListAsync();
//            _cache.Set("Ticket On Hold", ticketOnHold);

//            var closingTicket = await _context.ClosingTickets.Where(x => x.IsClosing == true && x.IsActive == true).ToListAsync();
//            _cache.Set("Closed Tickets", closingTicket);

//            var openTicket = await _context.TicketConcerns.Where(x => x.OnHold == null && x.IsActive == true
//            && x.IsApprove == true && x.IsTransfer != true && x.IsClosedApprove != true).ToListAsync();
//            _cache.Set("Open Tickets", openTicket);

//            var transferTicket = await _context.TransferTicketConcerns.Where(x => x.IsActive == true && x.IsTransfer == true).ToListAsync();
//            _cache.Set("Transfer Tickets", transferTicket);

//            var requestConcern = await _context.RequestConcerns.Where(x => x.IsActive).ToListAsync();
//            _cache.Set("Request Concern Tickets", requestConcern);
//        }

//        public List<TicketOnHold> GetTicketOnHolds()
//        {
//            return _cache.TryGetValue("Ticket On Hold", out List<TicketOnHold> ticketOnHold)
//                ? ticketOnHold : new List<TicketOnHold>();
//        }

//        public List<ClosingTicket> GetClosingTickets()
//        {
//            return _cache.TryGetValue("Closed Tickets", out List<ClosingTicket> closingTicket)
//                ? closingTicket : new List<ClosingTicket>();
//        }

//        public List<TicketConcern> GetOpenTickets()
//        {
//            return _cache.TryGetValue("Open Tickets", out List<TicketConcern> openTicket)
//                ? openTicket : new List<TicketConcern>();
//        }

//        public List<TransferTicketConcern> GetTransferTicketConcerns()
//        {
//            return _cache.TryGetValue("Transfer Tickets", out List<TransferTicketConcern> transferTicket)
//                ? transferTicket : new List<TransferTicketConcern>();
//        }

//        public List<RequestConcern> GetRequestConcerns()
//        {
//            return _cache.TryGetValue("Ticket On Hold", out List<RequestConcern> requestTicket)
//                ? requestTicket : new List<RequestConcern>();
//        }
//    }
//}
