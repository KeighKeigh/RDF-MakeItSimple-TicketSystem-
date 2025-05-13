﻿using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket.GetOpenTicket.GetOpenTicketResult.GetForClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetConcernTicket.GetRequestorTicketConcern;
namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket
{
    public partial class GetOpenTicket
    {

        public class Handler : IRequestHandler<GetOpenTicketQuery, PagedList<GetOpenTicketResult>>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOpenTicketResult>> Handle(GetOpenTicketQuery request, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                IQueryable<TicketConcern> ticketConcernQuery = _context.TicketConcerns
                   .AsNoTrackingWithIdentityResolution()
                   .Where(x => x.IsActive)
                    .AsSplitQuery();

                if (ticketConcernQuery.Any())
                {

                    var fillterApproval = ticketConcernQuery
                        .Select(x => x.Id);

                    var allUserList = await _context.UserRoles
                        .AsNoTracking()
                        .Select(x => new
                        {
                            x.Id,
                            x.UserRoleName,
                            x.Permissions

                        })
                        .ToListAsync();

                    var receiverPermissionList = allUserList
                         .Where(x => x.Permissions
                         .Contains(TicketingConString.Receiver))
                         .Select(x => x.UserRoleName)
                         .ToList();

                    var issueHandlerPermissionList = allUserList
                        .Where(x => x.Permissions.Contains(TicketingConString.IssueHandler))
                        .Select(x => x.UserRoleName)
                        .ToList();

                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        ticketConcernQuery = ticketConcernQuery
                            .Where(x => x.User.Fullname.ToLower().Contains(request.Search.ToLower())
                        || x.User.SubUnit.SubUnitName.ToLower().Contains(request.Search.ToLower())
                        || x.Id.ToString().Contains(request.Search));

                    }

                    if (request.Status is not null)
                    {
                        ticketConcernQuery = ticketConcernQuery
                            .Where(x => x.IsActive == request.Status);
                    }
                    if (request.Ascending is not null)
                    {

                        ticketConcernQuery = request.Ascending.Value is true
                            ? ticketConcernQuery
                            .OrderBy(x => x.Id)
                            : ticketConcernQuery
                            .OrderByDescending(x => x.Id);
                    }

                    if (!string.IsNullOrEmpty(request.Concern_Status))
                    {
                        switch (request.Concern_Status)
                        {
                            case TicketingConString.PendingRequest:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsApprove == false && x.OnHold == null);
                                break;

                            case TicketingConString.Open:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsApprove == true && x.IsTransfer != false
                                    && x.IsClosedApprove == null && x.OnHold == null );
                                break;

                            case TicketingConString.ForTransfer:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.OnHold == null)
                                    .Where(x => x.TransferTicketConcerns
                                    .FirstOrDefault(x => x.IsActive == true && x.IsTransfer == false)
                                    .TransferBy == request.UserId);
                                break;

                            case TicketingConString.ForOnHold:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.OnHold == false);
                                break;

                            case TicketingConString.OnHold:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.OnHold == true);
                                break;

                            case TicketingConString.ForClosing:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == false && x.OnHold == null);
                                break;

                            case TicketingConString.NotConfirm:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null);
                                break;

                            case TicketingConString.Closed:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold  == null);
                                break;

                            default:
                                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);

                        }
                    }

                    if (!string.IsNullOrEmpty(request.History_Status))
                    {
                        switch (request.History_Status)
                        {
                            case TicketingConString.PendingRequest:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsApprove == false && x.OnHold == null);
                                break;

                            case TicketingConString.Open:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsApprove == true && x.IsTransfer != false
                                    && x.IsClosedApprove == null && x.OnHold == null);
                                break;

                            case TicketingConString.ForTransfer:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.OnHold == null)
                                    .Where(x => x.TransferTicketConcerns
                                    .FirstOrDefault(x => x.IsActive == true && x.IsTransfer == false)
                                    .TransferBy == request.UserId);
                                break;
                            case TicketingConString.ForOnHold:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.OnHold == false);
                                break;

                            case TicketingConString.OnHold:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.OnHold == true);
                                break;

                            case TicketingConString.ForClosing:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == false && x.OnHold == null);
                                break;

                            case TicketingConString.NotConfirm:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null);
                                break;

                            case TicketingConString.Closed:
                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null);
                                break;

                            default:
                                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);

                        }
                    }

                    if (request.Date_From is not null && request.Date_To is not null)
                    {
                        ticketConcernQuery = ticketConcernQuery
                            .Where(x => x.TargetDate >= request.Date_From.Value && x.TargetDate <= request.Date_To.Value);
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {

                        if (request.UserType == TicketingConString.IssueHandler)
                        {
                            //var transferApprovalList = _context.TransferTicketConcerns
                            //    .AsNoTracking()
                            //    .Where(t => t.IsTransfer == false && t.IsActive == true && t.TransferTo == request.UserId)
                            //    .Select(t => t.TicketConcernId);

                            ticketConcernQuery = ticketConcernQuery.Where(x => x.UserId == request.UserId);
                        }
                        else if (request.UserType == TicketingConString.Receiver)
                        {
                            var listOfRequest = await ticketConcernQuery
                                .Select(x => x.User.BusinessUnitId)
                                .ToListAsync();

                            var businessUnitDefault = await _context.BusinessUnits
                            .AsNoTracking()
                            .Where(x => x.IsActive == true)
                            .Where(x => listOfRequest.Contains(x.Id))
                            .Select(x => x.Id)
                            .ToListAsync();

                            var receiverList = await _context.Receivers
                                .AsNoTrackingWithIdentityResolution()
                                .Include(x => x.BusinessUnit)
                                .AsSplitQuery()
                                .Where(x => businessUnitDefault.Contains(x.BusinessUnitId.Value) && x.IsActive == true &&
                                 x.UserId == request.UserId)
                                .Select(x => x.BusinessUnitId)
                                .ToListAsync();

                            if (receiverPermissionList.Any(x => x.Contains(request.Role)) && receiverList.Any())
                            {

                                ticketConcernQuery = ticketConcernQuery
                                    .Where(x => receiverList.Contains(x.RequestorByUser.BusinessUnitId));
                            }
                            else
                            {
                                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
                            }

                        }
                        else
                        {
                            return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
                        }
                    }

                }

                var results = ticketConcernQuery
                    .Select(x => new GetOpenTicketResult
                    {

                        Closed_Status = x.TargetDate.Value.Day >= x.Closed_At.Value.Day && x.IsClosedApprove == true
                        ? TicketingConString.OnTime : x.TargetDate.Value.Day < x.Closed_At.Value.Day && x.IsClosedApprove == true
                        ? TicketingConString.Delay : null,
                        TicketConcernId = x.Id,
                        RequestConcernId = x.RequestConcernId,
                        Concern_Description = x.RequestConcern.Concern,
                        Company_Code = x.RequestConcern.Company.CompanyCode,
                        Company_Name = x.RequestConcern.Company.CompanyName,
                        BusinessUnit_Code = x.RequestConcern.BusinessUnit.BusinessCode,
                        BusinessUnit_Name = x.RequestConcern.BusinessUnit.BusinessName,
                        Department_Code = x.RequestorByUser.Department.DepartmentCode,
                        Department_Name = x.RequestorByUser.Department.DepartmentName,
                        Unit_Code = x.RequestConcern.Unit.UnitCode,
                        Unit_Name = x.RequestConcern.Unit.UnitName,
                        SubUnit_Code = x.RequestorByUser.SubUnit.SubUnitCode,
                        SubUnit_Name = x.User.SubUnit.SubUnitName,
                        Location_Code = x.RequestConcern.Location.LocationCode,
                        Location_Name = x.RequestConcern.Location.LocationName,
                        Requestor_By = x.RequestorBy,
                        Requestor_Name = x.RequestorByUser.Fullname,
                        GetOpenTicketCategories = x.RequestConcern.TicketCategories
                                                .Select(t => new GetOpenTicketResult.GetOpenTicketCategory
                                                {
                                                    TicketCategoryId = t.Id,
                                                    CategoryId = t.CategoryId,
                                                    Category_Description = t.Category.CategoryDescription,

                                                }).ToList(),

                        GetOpenTicketSubCategories = x.RequestConcern.TicketSubCategories
                                                .Select(t => new GetOpenTicketResult.GetOpenTicketSubCategory
                                                {
                                                    TicketSubCategoryId = t.Id,
                                                    SubCategoryId = t.SubCategoryId,
                                                    SubCategory_Description = t.SubCategory.SubCategoryDescription,
                                                }).ToList(),

                        Date_Needed = x.RequestConcern.DateNeeded,
                        Notes = x.RequestConcern.Notes,
                        Contact_Number = x.RequestConcern.ContactNumber,
                        Request_Type = x.RequestConcern.RequestType,
                        BackJobId = x.RequestConcern.BackJobId,
                        Back_Job_Concern = x.RequestConcern.BackJob.Concern,
                        ChannelId = x.RequestConcern.ChannelId,
                        Channel_Name = x.RequestConcern.Channel.ChannelName,
                        UserId = x.UserId,
                        Issue_Handler = x.User.Fullname,
                        Target_Date = x.TargetDate,
                        Ticket_Status = x.IsApprove == false && x.OnHold == null ? TicketingConString.PendingRequest
                                        : x.IsApprove == true != false && x.IsTransfer != false && x.IsClosedApprove == null && x.OnHold == null ? TicketingConString.OpenTicket
                                        : x.TransferTicketConcerns.FirstOrDefault(x => x.IsTransfer == false && x.IsActive == true)
                                        .TransferBy == request.UserId ? TicketingConString.ForTransfer
                                        : x.OnHold == false ? TicketingConString.ForOnHold
                                        : x.OnHold == true ? TicketingConString.OnHold
                                        : x.IsClosedApprove == false && x.OnHold == null ? TicketingConString.ForClosing
                                        : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null ? TicketingConString.NotConfirm
                                        : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null ? TicketingConString.Closed
                                        : "Unknown",

                        Added_By = x.AddedByUser.Fullname,
                        Created_At = x.CreatedAt,
                        Remarks = x.Remarks,
                        Modified_By = x.ModifiedByUser.Fullname,
                        Updated_At = x.UpdatedAt,
                        IsActive = x.IsActive,
                        Is_Closed = x.IsClosedApprove,
                        Closed_At = x.Closed_At,
                        Is_Transfer = x.IsTransfer,
                        Transfer_At = x.TransferAt,
                        GetForClosingTickets = x.ClosingTickets
                       .Where(x => x.IsActive == true && x.IsRejectClosed == false)
                       .Where(x => x.IsClosing == true ? x.IsClosing == true : x.IsClosing == false)
                      .Select(x => new GetOpenTicketResult.GetForClosingTicket
                      {
                          ClosingTicketId = x.Id,
                          Remarks = x.RejectRemarks,
                          Resolution = x.Resolution,
                          ForClosingTicketTechnicians = x.ticketTechnicians.
                          Select(t => new GetOpenTicketResult.GetForClosingTicket.ForClosingTicketTechnician
                          {
                              TicketTechnicianId = t.Id,
                              Technician_By = t.TechnicianBy,
                              Fullname = t.TechnicianByUser.Fullname,

                          }).ToList(),

                          GetForClosingTicketCategories = x.TicketConcern.RequestConcern.TicketCategories
                                                  .Select(t => new GetOpenTicketResult.GetForClosingTicket.GetForClosingTicketCategory
                                                  {
                                                      TicketCategoryId = t.Id,
                                                      CategoryId = t.CategoryId,
                                                      Category_Description = t.Category.CategoryDescription,

                                                  }).ToList(),

                          GetForClosingTicketSubCategories = x.TicketConcern.RequestConcern.TicketSubCategories
                                                  .Select(t => new GetOpenTicketResult.GetForClosingTicket.GetForClosingTicketSubCategory
                                                  {
                                                      TicketSubCategoryId = t.Id,
                                                      SubCategoryId = t.SubCategoryId,
                                                      SubCategory_Description = t.SubCategory.SubCategoryDescription,
                                                  }).ToList(),
                          Notes = x.Notes,
                          IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
                          ApproverLists = x.ApproverTickets
                          .Where(x => x.IsApprove != null)
                          .Select(x => new ApproverList
                          {
                              ApproverName = x.User.Fullname,
                              Approver_Level = x.ApproverLevel.Value

                          }).ToList(),

                          GetAttachmentForClosingTickets = x.TicketAttachments.Select(x => new GetAttachmentForClosingTicket
                          {
                              TicketAttachmentId = x.Id,
                              Attachment = x.Attachment,
                              FileName = x.FileName,
                              FileSize = x.FileSize,

                          }).ToList(),

                      }).ToList(),

                        GetForOnHolds = x.TicketOnHolds
                        .Where(x => x.IsHold == false && x.IsActive)
                        .Select(h => new GetOpenTicketResult.GetForOnHold
                        {
                            Id = h.Id,
                            Reason = h.Reason,
                            AddedBy = h.AddedByUser.Fullname,
                            CreatedAt = h.CreatedAt,
                            IsHold = h.IsHold,
                            ResumeAt = h.ResumeAt,
                            IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
                            GetAttachmentForOnHoldOpenTickets = h.TicketAttachments
                            .Select(t => new GetOpenTicketResult.GetForOnHold.GetAttachmentForOnHoldOpenTicket
                            {
                                TicketAttachmentId = t.Id,
                                Attachment = t.Attachment,
                                FileName = t.FileName,
                                FileSize = t.FileSize,

                            }).ToList(),

                        }).ToList(),

                        GetOnHolds = x.TicketOnHolds
                        .Where(x => x.IsHold == true && x.IsActive)
                        .Select(h => new GetOpenTicketResult.GetOnHold
                        {
                            Id = h.Id,
                            Reason = h.Reason,
                            AddedBy = h.AddedByUser.Fullname,
                            CreatedAt = h.CreatedAt,
                            IsHold = h.IsHold,
                            ResumeAt = h.ResumeAt,
                            IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
                            GetAttachmentOnHoldOpenTickets = h.TicketAttachments
                            .Select(t => new GetOpenTicketResult.GetOnHold.GetAttachmentOnHoldOpenTicket
                            {
                                TicketAttachmentId = t.Id,
                                Attachment = t.Attachment,
                                FileName = t.FileName,
                                FileSize = t.FileSize,

                            }).ToList(),

                        }).ToList(),

                        GetForTransferTickets = x.TransferTicketConcerns
                        .Where(x => x.IsActive == true && x.IsTransfer == false && x.TransferBy == request.UserId)
                        .Select(x => new GetOpenTicketResult.GetForTransferTicket
                        {
                            TransferTicketConcernId = x.Id,
                            Transfer_Remarks = x.TransferRemarks,
                            Transfer_To = x.TransferTo,
                            Transfer_To_Name = x.TransferToUser.Fullname,
                            Current_Target_Date = x.Current_Target_Date,
                            IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
                            GetAttachmentForTransferTickets = x.TicketAttachments.Select(x => new GetOpenTicketResult.GetForTransferTicket.GetAttachmentForTransferTicket
                            {
                                TicketAttachmentId = x.Id,
                                Attachment = x.Attachment,
                                FileName = x.FileName,
                                FileSize = x.FileSize,

                            }).ToList(),

                        })
                        .ToList(),

                        Transaction_Date = x.ticketHistories.Max(x => x.TransactionDate).Value,
                        Aging_Days = x.Closed_At != null ? EF.Functions.DateDiffDay(x.TargetDate.Value.Date, x.Closed_At.Value.Date)
                        : EF.Functions.DateDiffDay(x.TargetDate.Value.Date, DateTime.Now.Date)

                    });


                return await PagedList<GetOpenTicketResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
