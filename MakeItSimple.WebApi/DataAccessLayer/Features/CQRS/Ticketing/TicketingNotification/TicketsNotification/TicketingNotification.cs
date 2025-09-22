﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketingNotification.TicketsNotification
{
    public partial class TicketingNotification
    {

        public class Handler : IRequestHandler<TicketingNotificationCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly IMediator _mediator;

            public Handler(MisDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }

            public async Task<Result> Handle(TicketingNotificationCommand request, CancellationToken cancellationToken)
            {

                var allRequestTicketNotif = new int();
                var forTicketNotif = new int();
                var currentlyFixingNotif = new int();
                var notConfirmNotif = new int();
                var doneNotif = new int();
                var receiverForApprovalNotif = new int();
                var allTicketNotif = new int();
                var openTicketNotif = new int();
                var forTransferNotif = new int();
                //var transferApprovalNotif = new int();
                var ForApprovalTargetDate = new int();
                var ApprovedDateNotif = new int();
                //var forNotApprovedDateNotif = new int();
                var forCloseNotif = new int();
                var ForOnHoldNotif = new int();
                var onHoldNotif = new int();
                var notCloseConfirmCloseNotif = new int();
                var closedNotif = new int();
                var forApprovalClosingNotif = new int();
                var forApprovalTransferNotif = new int();
                var forApprovalOnHoldNotif = new int();
                var openTicketsForApproverNotif = new int();
                var delayTicketsForApproverNotif = new int();
                var dateRejectedNotif = new int();


                var dateToday = DateTime.Today;

                var allUserList = await _context.UserRoles
                    .AsNoTracking()
                    .Select(x => new
                    {
                        //x.Id,
                        x.UserRoleName,
                        x.Permissions

                    }).ToListAsync();

                var requestorPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Requestor))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var approverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Approver))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var receiverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Receiver))
                    .Select(x => x.UserRoleName)
                    .ToList();

                var requestConcernsQuery = await _context.RequestConcerns
                    .AsNoTracking()
                    .Where(x => x.IsActive == true )
                    .Select(x => new
                    {
                        x.Id,
                        x.User,
                        x.UserId,
                        x.ConcernStatus,
                        x.Is_Confirm,
                        //x.IsActive,
                        //x.IsDone,
                        //x.AssignTo

                    }).ToListAsync();

                var ticketConcernQuery = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Where(x => x.IsActive && x.AssignTo == request.UserId)
                    .AsSplitQuery()
                    .Select(x => new
                    {
                        x.Id,
                        RequestConcern = new
                        {
                            x.RequestConcern.Is_Confirm,
                            x.RequestConcern.ConcernStatus
                        },
                        //x.RequestConcernId,
                        //x.User,
                        //x.UserId,
                        //x.IsActive,
                        //x.IsDone,
                        x.IsApprove,
                        x.IsClosedApprove,
                        x.IsTransfer,
                        x.Closed_At,
                        x.OnHold,
                        //x.AssignTo,
                        x.IsDateApproved,
                        //x.DateApprovedAt,
                        x.ConcernStatus,
                        
                        

                    }).ToListAsync();

                var onHoldQuery = await _context.TicketOnHolds
                    .AsNoTrackingWithIdentityResolution()
                   .AsSplitQuery()
                   .Where(x => x.IsActive)
                   .Select(x => new
                   {
                       x.Id,
                       x.TicketApprover,
                       //x.TicketConcernId,

                   }).ToListAsync();

                var transferQuery = await _context.TransferTicketConcerns
                .AsNoTrackingWithIdentityResolution()
                .AsSplitQuery()
                .Where(x => x.IsActive == true && x.IsTransfer == false)
                .Select(x => new
                {
                    x.Id,
                    //TicketConcern = new
                    //{
                    //    x.TicketConcern.OnHold,
                    //},
                    //x.TicketConcernId,
                    x.TicketApprover,
                    x.TransferBy,
                    //x.TransferTo,

                }).ToListAsync();

                var closeQuery = await _context.ClosingTickets
                    .AsNoTracking()
                    .Where(x => x.IsActive)
                    .Where(x => x.IsRejectClosed == false)
                    .Where(x => x.IsClosing == false)
                    .Select(x => new
                    {
                        x.Id,
                        //x.IsClosing,
                        x.TicketApprover

                    }).ToListAsync();


                var dateApproval = await _context.ApproverDates
                    .AsNoTracking()
                    .Where(x => x.IsActive && x.IsApproved == false && x.IsRejectDate == false)
                    
                    .Select(x => new
                    {
                        x.Id,
                        //x.IsApproved,
                        //x.TicketApprover

                    }).ToListAsync();




                if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                {

                    //var transferApprovalList = await _context.TransferTicketConcerns
                    //    .AsNoTracking()
                    //    .Where(t => t.IsTransfer == false && t.TransferTo == request.UserId)
                    //    .Select(t => t.TicketConcernId)
                    //    .ToListAsync();


                    requestConcernsQuery = requestConcernsQuery
                        .Where(x => x.UserId == request.UserId)
                        .ToList();

                    allRequestTicketNotif = requestConcernsQuery.Count();

                    forTicketNotif = requestConcernsQuery
                            .Where(x => x.ConcernStatus == TicketingConString.ForApprovalTicket)
                            .Count();

                    currentlyFixingNotif = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.CurrentlyFixing)
                                    .Count();

                    notConfirmNotif = requestConcernsQuery
                        .Where(x => x.Is_Confirm == null && x.ConcernStatus == TicketingConString.NotConfirm)
                        .Count();

                    doneNotif = requestConcernsQuery
                        .Where(x => x.ConcernStatus == TicketingConString.Done && x.Is_Confirm == true)
                        .Count();

                    //ticketConcernQuery = ticketConcernQuery
                    //    .Where(x => x.AssignTo == request.UserId /*|| transferApprovalList.Contains(x.Id)*/ && ticketConcernQuery.Any())
                    //    .ToList();

                    allTicketNotif = ticketConcernQuery.Count();

                    openTicketNotif = ticketConcernQuery
                         .Where(x => x.IsApprove == true && x.IsTransfer != false
                         && x.IsClosedApprove == null && x.OnHold == null)
                         .Count();

                    ApprovedDateNotif = ticketConcernQuery
                         .Where(x => x.IsDateApproved == false 
                         && x.OnHold != true && x.ConcernStatus == TicketingConString.ForApprovalTicket)
                         .Count();

                    dateRejectedNotif = ticketConcernQuery
                        .Where(x => x.ConcernStatus == TicketingConString.DateRejected).Count();

                    forTransferNotif = transferQuery
                         .Where(x => x.TransferBy == request.UserId)
                         .Count();

                    //transferApprovalNotif = transferQuery
                    //    .Where(t => t.TransferTo == request.UserId)
                    //    .Count();

                    forCloseNotif = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == false && x.OnHold == null)
                        .Count();

                    ForOnHoldNotif = ticketConcernQuery
                        .Where(x => x.OnHold == false)
                        .Count();

                    onHoldNotif = ticketConcernQuery
                        .Where(x => x.OnHold == true)
                        .Count();

                    notCloseConfirmCloseNotif = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null)
                        .Count();

                    closedNotif = ticketConcernQuery
                        .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null)
                        .Count();

                }



                if (approverPermissionList.Any(x => x.Contains(request.Role)))
                {

                    var userApprover = await _context.Users
                        .AsNoTracking()
                        .Select(x => new
                        {
                            x.Id,
                            x.Username,

                        }).FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);


                    var approverSubUnitIds = await _context.ApproverUsers
                   .AsNoTracking()
                   .Where(x => x.ApproverId == userApprover.Id)
                  .Select(x => x.UserId)
                  .ToListAsync();


                    if (approverSubUnitIds.Any())
                    {
                        var openTicketsForApprover = await _context.RequestConcerns
                            .AsNoTracking()
                            .Where(x => x.IsActive == true
                                && x.ConcernStatus == TicketingConString.OnGoing
                                && approverSubUnitIds.Contains(x.AssignTo))
                            .Select(x => new
                            {
                                x.Id,
                                x.ConcernStatus,
                                x.UserId,
                                x.TargetDate
                            }).ToListAsync();

                        openTicketsForApproverNotif = openTicketsForApprover.Count();

                        var delayedTicketsForApprover = openTicketsForApprover.Where(x => x.TargetDate.Value.Date < dateToday)
                            .Select(x => new
                            {
                                x.Id,
                                x.ConcernStatus,
                                x.UserId
                            }).ToList();

                        delayTicketsForApproverNotif = delayedTicketsForApprover.Count();
                    }


                    var approverTransactList = await _context.ApproverTicketings
                        .AsNoTracking()
                        .Where(x => x.UserId == userApprover.Id)
                        .Where(x => x.IsApprove == null)
                        .Select(x => new
                        {
                            x.ApproverLevel,
                            x.IsApprove,
                            x.TransferTicketConcernId,
                            x.ClosingTicketId,
                            x.TicketOnHoldId,
                            x.UserId,
                            x.ApproverDateId

                        }).ToListAsync();

                   



                    //var approverOpenTicket = await _context.RequestConcerns
                    //    .AsNoTracking()
                    //    .Where(x => x.UserId == userApprover.Id)
                    //    .Where(x => x.IsDone == null)
                    //    .Select(x => new
                    //    {
                    //        x.UserId,
                    //        x.Id,

                    //    }).ToListAsync();

                    if (closeQuery.Any())
                    {

                        var userRequestIdApprovalList = approverTransactList
                            .Select(x => x.ClosingTicketId)
                            .ToList();

                        var userIdsInApprovalList = approverTransactList
                            .Select(approval => approval.UserId)
                            .ToList();

                        forApprovalClosingNotif = closeQuery
                              .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                              && userRequestIdApprovalList.Contains(x.Id))
                              .Count();
                    }

                    if (dateApproval.Any())
                    {

                        var userRequestIdApprovalList = approverTransactList
                            .Select(x => x.ApproverDateId)
                            .ToList();

                        var userIdsInApprovalList = approverTransactList
                            .Select(approval => approval.UserId)
                            .ToList();
                        
                        ForApprovalTargetDate = dateApproval
                              .Where(x => userRequestIdApprovalList.Contains(x.Id))
                              .Count();
                    }

                    if (transferQuery.Any())
                    {

                        var userRequestIdApprovalList = approverTransactList
                            .Select(x => x.TransferTicketConcernId)
                            .ToList();

                        var userIdsInApprovalList = approverTransactList
                            .Select(approval => approval.UserId)
                            .ToList();

                        forApprovalTransferNotif = transferQuery
                              .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                              && userRequestIdApprovalList.Contains(x.Id))
                              .Count();
                    }

                    if (onHoldQuery.Any())
                    {

                        var userRequestIdApprovalList = approverTransactList
                            .Select(x => x.TicketOnHoldId)
                            .ToList();

                        var userIdsInApprovalList = approverTransactList
                            .Select(approval => approval.UserId)
                            .ToList();

                        forApprovalOnHoldNotif = onHoldQuery
                              .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                              && userRequestIdApprovalList.Contains(x.Id))
                              .Count();
                    }




                }


                if (receiverPermissionList.Any(x => x.Contains(request.Role)))
                {

                    if (requestConcernsQuery.Any())
                    {
                        var listOfRequest = requestConcernsQuery
                            .Select(x => x.User.BusinessUnitId)
                            .ToList();

                        var receiverList = await _context.Receivers
                            .AsNoTrackingWithIdentityResolution()
                            .Include(x => x.BusinessUnit)
                            .AsSplitQuery()
                            .Where(x => x.IsActive == true)
                            .Where(x => listOfRequest
                            .Contains(x.BusinessUnitId.Value))
                            .Select(x => x.BusinessUnitId)
                            .ToListAsync();

                        var receiverConcernsQuery = requestConcernsQuery
                                .Where(x => receiverList.Contains(x.User.BusinessUnitId))
                                .Select(x => x.Id)
                                .ToList();

                        receiverForApprovalNotif = await _context.TicketConcerns
                           .AsNoTrackingWithIdentityResolution()
                           .Where(x => receiverConcernsQuery.Contains(x.RequestConcernId.Value) && x.IsApprove == false)
                           .CountAsync();

                    }

                }

                var notification = new TicketingNotifResult
                {
                    AllRequestTicketNotif = allRequestTicketNotif,
                    ForTicketNotif = forTicketNotif,
                    CurrentlyFixingNotif = currentlyFixingNotif,
                    NotConfirmNotif = notConfirmNotif,
                    DoneNotif = doneNotif,
                    ReceiverForApprovalNotif = receiverForApprovalNotif,
                    AllTicketNotif = allTicketNotif,
                    OpenTicketNotif = openTicketNotif,
                    ForTransferNotif = forTransferNotif,
                    ForOnHoldNotif = ForOnHoldNotif,
                    //TransferApprovalNotif = transferApprovalNotif,
                    OnHold = onHoldNotif,
                    ForCloseNotif = forCloseNotif,
                    NotConfirmCloseNotif = notCloseConfirmCloseNotif,
                    ClosedNotif = closedNotif,
                    ForApprovalOnHoldNotif = forApprovalOnHoldNotif,
                    ForApprovalTransferNotif = forApprovalTransferNotif,
                    ForApprovalClosingNotif = forApprovalClosingNotif,
                    ForApprovalTargetDate = ForApprovalTargetDate,
                    ApprovalDateNotif = ApprovedDateNotif,
                    ListOfOpenTicketNotif = openTicketsForApproverNotif,
                    ListOfDelayedTicketNotif = delayTicketsForApproverNotif,
                    DateRejectedNotif = dateRejectedNotif,
                };



                var ticketConcernList = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Where(x => x.RequestConcern.Is_Confirm == null
                    && x.RequestConcern.ConcernStatus == TicketingConString.NotConfirm && x.AssignTo == request.UserId)
                    .Select(x => new
                    {
                        x.Id,
                        x.RequestConcernId,
                        x.User,
                        x.UserId,
                        x.IsActive,
                        x.Closed_At,

                    }).ToListAsync();

                if (ticketConcernList == null)
                {
                    return Result.Success(notification);
                }

                foreach (var confirm in ticketConcernList)
                {

                    int hoursDifference = 24;

                    var daysClose = confirm.Closed_At.Value.Day - DateTime.Now.Day;

                    daysClose = Math.Abs(daysClose) * 1;

                    if (daysClose >= 1)
                    {
                        daysClose = daysClose * hoursDifference;
                    }

                    var hourConvert = daysClose + confirm.Closed_At.Value.Hour - DateTime.Now.Hour;

                    DayOfWeek todayWeek = DateTime.Now.DayOfWeek;
                    DayOfWeek exceptSat = DayOfWeek.Saturday;
                    DayOfWeek exceptSun = DayOfWeek.Sunday;

                    if (hourConvert >= hoursDifference && todayWeek != exceptSat && todayWeek != exceptSun)
                    {
                        var requestConcern = await _context.RequestConcerns
                            .FirstOrDefaultAsync(x => x.Id == confirm.RequestConcernId);

                        requestConcern.Is_Confirm = true;
                        requestConcern.Confirm_At = DateTime.Today;
                        requestConcern.ConcernStatus = TicketingConString.Done;

                        var ticketHistory = await _context.TicketHistories
                            .Where(x => x.TicketConcernId == confirm.Id)
                            .Where(x => x.IsApprove == null && x.Request.Contains(TicketingConString.NotConfirm))
                            .FirstOrDefaultAsync();

                        if (ticketHistory != null)
                        {
                            ticketHistory.TicketConcernId = confirm.Id;
                            ticketHistory.TransactedBy = request.UserId;
                            ticketHistory.TransactionDate = DateTime.Now;
                            ticketHistory.Request = TicketingConString.Confirm;
                            ticketHistory.Status = TicketingConString.CloseConfirm;
                        }

                        var addNewTicketTransactionNotification = new TicketTransactionNotification
                        {

                            Message = $"Ticket number {confirm.Id} has been closed",
                            AddedBy = request.UserId,
                            Created_At = DateTime.Now,
                            ReceiveBy = confirm.UserId.Value,
                            Modules = PathConString.IssueHandlerConcerns,
                            Modules_Parameter = PathConString.Closed,

                        };

                        await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                    }

                }





                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success(notification);

            }
        }
    }
}
