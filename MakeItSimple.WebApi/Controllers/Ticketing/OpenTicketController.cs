using MakeItSimple.WebApi.Common.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket.GetOpenTicket;
using System.Security.Claims;
using MediatR;
using MakeItSimple.WebApi.Common;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewTicketHistory.GetTicketHistory;
using Microsoft.AspNetCore.SignalR;
using MakeItSimple.WebApi.Common.SignalR;
using MakeItSimple.WebApi.Common.Caching;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/open-ticket")]
    [ApiController]
    public class OpenTicketController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly TimerControl _timerControl;

        public OpenTicketController(IMediator mediator, TimerControl timerControl)
        {
            _mediator = mediator;
            _timerControl = timerControl;
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetOpenTicket([FromQuery] GetOpenTicketQuery query)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        query.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        query.UserId = userId;
                    }

                }

                var openTicket = await _mediator.Send(query);

                Response.AddPaginationHeader(

                openTicket.CurrentPage,
                openTicket.PageSize,
                openTicket.TotalCount,
                openTicket.TotalPages,
                openTicket.HasPreviousPage,
                openTicket.HasNextPage

                );

                var result = new
                {
                    openTicket,
                    openTicket.CurrentPage,
                    openTicket.PageSize,
                    openTicket.TotalCount,
                    openTicket.TotalPages,
                    openTicket.HasPreviousPage,
                    openTicket.HasNextPage
                };

                var successResult = Result.Success(result);

                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetTicketHistory([FromRoute] int id)
        {
            try
            {
                var query = new GetTicketHistoryQuery
                {
                    TicketConcernId = id
                };

                var results = await _mediator.Send(query);
                if (results.IsFailure)
                {
                    return BadRequest(query);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }



    }
}
