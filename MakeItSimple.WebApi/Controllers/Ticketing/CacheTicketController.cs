//using LazyCache;
//using MakeItSimple.WebApi.Common.Caching;
//using MakeItSimple.WebApi.Common.SignalR;
//using MakeItSimple.WebApi.Hubs;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Caching.Memory;
//using Newtonsoft.Json;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;

//namespace MakeItSimple.WebApi.Controllers.Ticketing
//{
//    [ApiController]
//    [Route("api/[cache-controller]")]
//    public class CacheTicketController : ControllerBase
//    {
//        //private readonly ICacheService _cacheService;

//        //public CacheTicketController(ICacheService cacheService)
//        //{
//        //    _cacheService = cacheService;
//        //}

//        private readonly IMediator _mediator;
//        private readonly ICacheNotificationHub _hubCaller;
//        private readonly TimerControl _timerControl;
//        private readonly CacheService _cacheService;

//        private ICacheProvider _cacheProvider;

//        public CacheTicketController(IMediator mediator, ICacheNotificationHub hubCaller, TimerControl timerControl, ICacheProvider cacheProvider, CacheService cacheService)
//        {
//            _mediator = mediator;
//            _hubCaller = hubCaller;
//            _timerControl = timerControl;
//            _cacheProvider = cacheProvider;
//            _cacheService = cacheService;
//        }

//        private string ComputeHash(object obj)
//        {
//            var jsonString = JsonConvert.SerializeObject(obj);
//            using (var sha256 = SHA256.Create())
//            {
//                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(jsonString));
//                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
//            }
//        }

//        private async Task<IActionResult> HandleNotification<T>(T command, string notificationType)
//        {
//            try
//            {
//                    dynamic cmd = command;
//                    var cacheKey = $"{notificationType}_CacheKey";
//                    var timerKey = $"{notificationType}";

//                    var newData = await _mediator.Send(command);
//                    var newHash = ComputeHash(newData);

//                    if (_cacheProvider.TryGetValue(cacheKey, out object cachedResult))
//                    {
//                        var cachedHash = ComputeHash(cachedResult);

//                        if (cachedHash == newHash)
//                        {
//                            _timerControl.StopTimer(timerKey);
//                            return Ok(cachedResult);
//                        }
//                    }

//                    var cacheEntryOptions = new MemoryCacheEntryOptions
//                    {
//                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
//                        SlidingExpiration = TimeSpan.FromDays(1),
//                        Size = 1024
//                    };

//                    _cacheProvider.Set(cacheKey, newData, cacheEntryOptions);

//                    _timerControl.ScheduleTimer(timerKey, async (scopeFactory) =>
//                    {
//                        using var scope = scopeFactory.CreateScope();
//                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
//                        var requestData = await mediator.Send(command);

//                        var requestDataHash = ComputeHash(requestData);
//                        var lastDataHash = ComputeHash(_cacheProvider.Get(cacheKey));

//                        if (requestDataHash != lastDataHash)
//                        {

//                            _cacheProvider.Set(cacheKey, requestData, cacheEntryOptions);
//                            await _hubCaller.BroadcastCacheAllAsync( notificationType, requestData);
//                        }

//                    }, 5000, 5000);


//                    await _hubCaller.BroadcastCacheAllAsync( notificationType, newData);

//                    return Ok(newData);
                
//            }
//            catch (Exception ex)
//            {
//                return Conflict(ex.Message);
//            }
//        }

//        [HttpGet("open-tickets")]
//        public async Task<IActionResult> GetTicketOnHolds()
//        {
//            var tickets = await _cacheService.GetTicketOnHolds();
//            return Ok(tickets);
//        }

//        [HttpGet("open-tickets")]
//        public async Task<IActionResult> GetClosingTickets()
//        {
//            var tickets = await _cacheService.GetClosingTickets();
//            return Ok(tickets);
//        }

//        [HttpGet("open-tickets")]
//        public async Task<IActionResult> GetOpenTickets()
//        {
//            var tickets = await _cacheService.GetOpenTickets();
//            return Ok(tickets);
//        }

//        [HttpGet("transfer-tickets")]
//        public async Task<IActionResult> GetTransferTicketConcerns()
//        {
//            var tickets = await _cacheService.GetTransferTicketConcerns();
//            return Ok(tickets);
//        }

//        [HttpGet("request-concerns")]
//        public async Task<IActionResult> GetRequestConcerns()
//        {
//            var concerns = await _cacheService.GetRequestConcerns();
//            return Ok(concerns);
//        }
//    }
//}
