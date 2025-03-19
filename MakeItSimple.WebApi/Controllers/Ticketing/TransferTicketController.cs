﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.ApprovalTransfer.ApprovedTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.CancelTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransfer.GetTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.RejectTransfer.RejectTransferTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.CreateTransfer.AddNewTransferTicket;
using MakeItSimple.WebApi.Common.SignalR;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.TransferUser.TransferTicketUser;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/transfer-ticket")]
    [ApiController]
    public class TransferTicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly TimerControl _timerControl;

        public TransferTicketController(IMediator mediator, TimerControl timerControl)
        {
            _mediator = mediator;
            _timerControl = timerControl;
        }

        [HttpPost("add-transfer")]
        public async Task<IActionResult> AddNewTransferTicket([FromForm] AddNewTransferTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                    command.Transfer_By = userId;
                    command.Transacted_By = userId;
                    command.Modified_By = userId;   
                }

                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }



        [HttpDelete("cancel")]
        public async Task<IActionResult> CancelTransferTicket([FromBody] CancelTransferTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Transacted_By = userId;
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("page")]
        public async Task<IActionResult> GetTransferTicket([FromQuery] GetTransferTicketQuery query)
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
                        //query.Users = userId;
                        query.UserId = userId;
                        
                    }
                }

                var transferTicket = await _mediator.Send(query);

                Response.AddPaginationHeader(

                transferTicket.CurrentPage,
                transferTicket.PageSize,
                transferTicket.TotalCount,
                transferTicket.TotalPages,
                transferTicket.HasPreviousPage,
                transferTicket.HasNextPage

                );

                var result = new
                {
                    transferTicket,
                    transferTicket.CurrentPage,
                    transferTicket.PageSize,
                    transferTicket.TotalCount,
                    transferTicket.TotalPages,
                    transferTicket.HasPreviousPage,
                    transferTicket.HasNextPage
                };

                var successResult = Result.Success(result);

                //var timerControl = _timerControl;
                //var clientsAll = _client.Clients.All;

                //if (timerControl != null && !timerControl.IsTimerStarted && clientsAll != null)
                //{
                //    timerControl.ScheduleTimer(async (scopeFactory) =>
                //    {
                //        using var scope = scopeFactory.CreateScope();
                //        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                //        var requestData = await mediator.Send(query);
                //        await clientsAll.SendAsync("TicketData", requestData);
                //    }, 2000);
                //}

                //await _client.Clients.All.SendAsync("ReceiveNotification", "New data has been received or sent.");

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("approval")]
        public async Task<IActionResult> ApprovedTransferTicket([FromBody] ApprovedTransferTicketCommand command)
        {
            try
            {
                
                if (User.Identity is ClaimsIdentity identity  )
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }
   
                    if(Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        //command.Transfer_By = userId;
                        command.Users = userId;
                        command.Transacted_By = userId;
                    }
                }

                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("reject")]
        public async Task<IActionResult> RejectTransferTicket([FromBody] RejectTransferTicketCommand command)
        {
            try
            {
                
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.RejectTransfer_By = userId;
                        command.Transacted_By = userId;
                    }
                }

                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("transfer-user")]
        public async Task<IActionResult> TransferTicketUser([FromQuery] TransferTicketUserCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.Transfer_By = userId;
                    }
                }

                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);


            }
            catch(Exception ex)
            {
                return Conflict(ex.Message);
            }
        }



    }
}
