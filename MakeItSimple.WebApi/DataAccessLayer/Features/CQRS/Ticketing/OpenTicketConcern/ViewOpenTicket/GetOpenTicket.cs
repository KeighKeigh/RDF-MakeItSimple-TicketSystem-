using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket.GetOpenTicket.GetOpenTicketResult.GetForClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetConcernTicket.GetRequestorTicketConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.GetTransfer.GetTransferTicket;
namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket
{
    //public partial class GetOpenTicket
    //{

    //    public class Handler : IRequestHandler<GetOpenTicketQuery, PagedList<GetOpenTicketResult>>
    //    {

    //        private readonly MisDbContext _context;

    //        public Handler(MisDbContext context)
    //        {
    //            _context = context;
    //        }

    //        public async Task<PagedList<GetOpenTicketResult>> Handle(GetOpenTicketQuery request, CancellationToken cancellationToken)
    //        {
    //            var dateToday = DateTime.Today;

    //            IQueryable<TicketConcern> ticketConcernQuery = _context.TicketConcerns
    //               .AsNoTrackingWithIdentityResolution()
    //               .Where(x => x.IsActive)
    //                .AsSplitQuery();

    //            if (ticketConcernQuery.Any())
    //            {

    //                var fillterApproval = ticketConcernQuery
    //                    .Select(x => x.Id);

    //                var allUserList = await _context.UserRoles
    //                    .AsNoTracking()
    //                    .Select(x => new
    //                    {
    //                        x.Id,
    //                        x.UserRoleName,
    //                        x.Permissions

    //                    })
    //                    .ToListAsync();

    //                var receiverPermissionList = allUserList
    //                     .Where(x => x.Permissions
    //                     .Contains(TicketingConString.Receiver))
    //                     .Select(x => x.UserRoleName)
    //                     .ToList();

    //                var issueHandlerPermissionList = allUserList
    //                    .Where(x => x.Permissions.Contains(TicketingConString.IssueHandler))
    //                    .Select(x => x.UserRoleName)
    //                    .ToList();

    //                if (!string.IsNullOrEmpty(request.Search))
    //                {
    //                    ticketConcernQuery = ticketConcernQuery
    //                        .Where(x => x.User.Fullname.ToLower().Contains(request.Search.ToLower())
    //                    || x.User.SubUnit.SubUnitName.ToLower().Contains(request.Search.ToLower())
    //                    || x.Id.ToString().Contains(request.Search));

    //                }

    //                if (request.Status is not null)
    //                {
    //                    ticketConcernQuery = ticketConcernQuery
    //                        .Where(x => x.IsActive == request.Status);
    //                }
    //                if (request.Ascending is not null)
    //                {

    //                    ticketConcernQuery = request.Ascending.Value is true
    //                        ? ticketConcernQuery
    //                        .OrderBy(x => x.Id)
    //                        : ticketConcernQuery
    //                        .OrderByDescending(x => x.Id);
    //                }
    //                else
    //                {
    //                    ticketConcernQuery = ticketConcernQuery.OrderBy(x => x.Id);
    //                }

    //                if (!string.IsNullOrEmpty(request.Concern_Status))
    //                {
    //                    switch (request.Concern_Status)
    //                    {
    //                        case TicketingConString.PendingRequest:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.IsApprove == false && x.OnHold == null);
    //                            break;

    //                        case TicketingConString.Open:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.IsApprove == true || x.IsTransfer != false
    //                                && x.IsClosedApprove == null && x.OnHold == null);
    //                            break;

    //                        case TicketingConString.ForTransfer:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.OnHold == null)
    //                                .Where(x => x.TransferTicketConcerns
    //                                .FirstOrDefault(x => x.IsActive == true && x.IsTransfer == false)
    //                                .TransferBy == request.UserId);
    //                            break;

    //                        case TicketingConString.ForOnHold:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.OnHold == false);
    //                            break;

    //                        case TicketingConString.OnHold:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.OnHold == true);
    //                            break;

    //                        case TicketingConString.ForClosing:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.IsClosedApprove == false && x.OnHold == null);
    //                            break;

    //                        case TicketingConString.NotConfirm:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null);
    //                            break;

    //                        case TicketingConString.Closed:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null);
    //                            break;

    //                        default:
    //                            return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);

    //                    }
    //                }

    //                if (!string.IsNullOrEmpty(request.History_Status))
    //                {
    //                    switch (request.History_Status)
    //                    {
    //                        case TicketingConString.PendingRequest:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.IsApprove == false && x.OnHold == null);
    //                            break;

    //                        case TicketingConString.Open:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.IsApprove == true || x.IsTransfer != false
    //                                && x.IsClosedApprove == null && x.OnHold == null);
    //                            break;

    //                        case TicketingConString.ForTransfer:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.OnHold == null)
    //                                .Where(x => x.TransferTicketConcerns
    //                                .FirstOrDefault(x => x.IsActive == true && x.IsTransfer == false)
    //                                .TransferBy == request.UserId);
    //                            break;
    //                        case TicketingConString.ForOnHold:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.OnHold == false);
    //                            break;

    //                        case TicketingConString.OnHold:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.OnHold == true);
    //                            break;

    //                        case TicketingConString.ForClosing:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.IsClosedApprove == false && x.OnHold == null);
    //                            break;

    //                        case TicketingConString.NotConfirm:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null);
    //                            break;

    //                        case TicketingConString.Closed:
    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null);
    //                            break;

    //                        default:
    //                            return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);

    //                    }
    //                }

    //                if (request.Date_From is not null && request.Date_To is not null)
    //                {
    //                    ticketConcernQuery = ticketConcernQuery
    //                        .Where(x => x.TargetDate >= request.Date_From.Value && x.TargetDate <= request.Date_To.Value);
    //                }

    //                if (!string.IsNullOrEmpty(request.UserType))
    //                {
    //                    if (request.UserType == TicketingConString.Approver)
    //                    {
    //                        if (request.UserType == TicketingConString.IssueHandler)
    //                        {
    //                            ticketConcernQuery = ticketConcernQuery.Where(x => x.AddedByUser.Id == request.UserId);
    //                        }
    //                        else
    //                        {
    //                            return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                        }
    //                    }

    //                    else if (request.UserType == TicketingConString.IssueHandler)
    //                    {
    //                        ticketConcernQuery = ticketConcernQuery.Where(x => x.AssignTo == request.UserId);
    //                    }
    //                    else if (request.UserType == TicketingConString.Receiver)
    //                    {
    //                        var listOfRequest = await ticketConcernQuery
    //                            .Select(x => x.User.BusinessUnitId)
    //                            .ToListAsync();

    //                        var businessUnitDefault = await _context.BusinessUnits
    //                        .AsNoTracking()
    //                        .Where(x => x.IsActive == true)
    //                        .Where(x => listOfRequest.Contains(x.Id))
    //                        .Select(x => x.Id)
    //                        .ToListAsync();

    //                        var receiverList = await _context.Receivers
    //                            .AsNoTrackingWithIdentityResolution()
    //                            .Include(x => x.BusinessUnit)
    //                            .AsSplitQuery()
    //                            .Where(x => businessUnitDefault.Contains(x.BusinessUnitId.Value) && x.IsActive == true &&
    //                             x.UserId == request.UserId)
    //                            .Select(x => x.BusinessUnitId)
    //                            .ToListAsync();

    //                        if (receiverPermissionList.Any(x => x.Contains(request.Role) && receiverList.Any()))
    //                        {

    //                            ticketConcernQuery = ticketConcernQuery
    //                                .Where(x => receiverList.Contains(x.RequestorByUser.BusinessUnitId));
    //                        }
    //                        else
    //                        {
    //                            return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                        }

    //                    }
    //                    else
    //                    {
    //                        return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                    }
    //                }

    //            }

    //            var results = ticketConcernQuery
    //                .Select(x => new GetOpenTicketResult
    //                {

    //                    Closed_Status = x.TargetDate.Value.Day >= x.Closed_At.Value.Day && x.IsClosedApprove == true
    //                    ? TicketingConString.OnTime : x.TargetDate.Value.Day < x.Closed_At.Value.Day && x.IsClosedApprove == true
    //                    ? TicketingConString.Delay : null,
    //                    TicketConcernId = x.Id,
    //                    RequestConcernId = x.RequestConcernId,
    //                    Concern_Description = x.RequestConcern.Concern,
    //                    Company_Code = x.RequestConcern.Company.CompanyCode,
    //                    Company_Name = x.RequestConcern.Company.CompanyName,
    //                    BusinessUnit_Code = x.RequestConcern.BusinessUnit.BusinessCode,
    //                    BusinessUnit_Name = x.RequestConcern.BusinessUnit.BusinessName,
    //                    Department_Code = x.RequestorByUser.Department.DepartmentCode,
    //                    Department_Name = x.RequestorByUser.Department.DepartmentName,
    //                    Unit_Code = x.RequestConcern.Unit.UnitCode,
    //                    Unit_Name = x.RequestConcern.Unit.UnitName,
    //                    SubUnit_Code = x.RequestorByUser.SubUnit.SubUnitCode,
    //                    SubUnit_Name = x.User.SubUnit.SubUnitName,
    //                    Location_Code = x.RequestConcern.Location.LocationCode,
    //                    Location_Name = x.RequestConcern.Location.LocationName,
    //                    Requestor_By = x.RequestorBy,
    //                    Requestor_Name = x.RequestorByUser.Fullname,
    //                    GetOpenTicketCategories = x.RequestConcern.TicketCategories
    //                                            .Select(t => new GetOpenTicketResult.GetOpenTicketCategory
    //                                            {
    //                                                TicketCategoryId = t.Id,
    //                                                CategoryId = t.CategoryId,
    //                                                Category_Description = t.Category.CategoryDescription,

    //                                            }).ToList(),

    //                    GetOpenTicketSubCategories = x.RequestConcern.TicketSubCategories
    //                                            .Select(t => new GetOpenTicketResult.GetOpenTicketSubCategory
    //                                            {
    //                                                TicketSubCategoryId = t.Id,
    //                                                SubCategoryId = t.SubCategoryId,
    //                                                SubCategory_Description = t.SubCategory.SubCategoryDescription,
    //                                            }).ToList(),

    //                    Date_Needed = x.RequestConcern.DateNeeded,
    //                    Notes = x.RequestConcern.Notes,
    //                    Contact_Number = x.RequestConcern.ContactNumber,
    //                    Request_Type = x.RequestConcern.RequestType,
    //                    BackJobId = x.RequestConcern.BackJobId,
    //                    Back_Job_Concern = x.RequestConcern.BackJob.Concern,
    //                    ChannelId = x.RequestConcern.ChannelId,
    //                    Channel_Name = x.RequestConcern.Channel.ChannelName,
    //                    UserId = x.UserId,
    //                    Issue_Handler = x.User.Fullname,
    //                    Target_Date = x.TargetDate,
    //                    Ticket_Status = x.IsApprove == false && x.OnHold == null ? TicketingConString.PendingRequest
    //                                    : x.IsApprove == true != false && x.IsTransfer != false && x.IsClosedApprove == null && x.OnHold == null ? TicketingConString.OpenTicket
    //                                    : x.TransferTicketConcerns.FirstOrDefault(x => x.IsTransfer == false && x.IsActive == true)
    //                                    .TransferBy == request.UserId ? TicketingConString.ForTransfer
    //                                    : x.OnHold == false ? TicketingConString.ForOnHold
    //                                    : x.OnHold == true ? TicketingConString.OnHold
    //                                    : x.IsClosedApprove == false && x.OnHold == null ? TicketingConString.ForClosing
    //                                    : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null ? TicketingConString.NotConfirm
    //                                    : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null ? TicketingConString.Closed
    //                                    : "Unknown",

    //                    Added_By = x.AddedByUser.Fullname,
    //                    Created_At = x.CreatedAt,
    //                    Remarks = x.Remarks,
    //                    Modified_By = x.ModifiedByUser.Fullname,
    //                    Updated_At = x.UpdatedAt,
    //                    IsActive = x.IsActive,
    //                    Is_Closed = x.IsClosedApprove,
    //                    Closed_At = x.Closed_At,
    //                    Is_Transfer = x.IsTransfer,
    //                    Transfer_At = x.TransferAt,
    //                    GetForClosingTickets = x.ClosingTickets
    //                   .Where(x => x.IsActive == true && x.IsRejectClosed == false)
    //                   .Where(x => x.IsClosing == true ? x.IsClosing == true : x.IsClosing == false)
    //                  .Select(x => new GetOpenTicketResult.GetForClosingTicket
    //                  {
    //                      ClosingTicketId = x.Id,
    //                      Remarks = x.RejectRemarks,
    //                      Resolution = x.Resolution,
    //                      ForClosingTicketTechnicians = x.ticketTechnicians.
    //                      Select(t => new GetOpenTicketResult.GetForClosingTicket.ForClosingTicketTechnician
    //                      {
    //                          TicketTechnicianId = t.Id,
    //                          Technician_By = t.TechnicianBy,
    //                          Fullname = t.TechnicianByUser.Fullname,

    //                      }).ToList(),

    //                      GetForClosingTicketCategories = x.TicketConcern.RequestConcern.TicketCategories
    //                                              .Select(t => new GetOpenTicketResult.GetForClosingTicket.GetForClosingTicketCategory
    //                                              {
    //                                                  TicketCategoryId = t.Id,
    //                                                  CategoryId = t.CategoryId,
    //                                                  Category_Description = t.Category.CategoryDescription,

    //                                              }).ToList(),

    //                      GetForClosingTicketSubCategories = x.TicketConcern.RequestConcern.TicketSubCategories
    //                                              .Select(t => new GetOpenTicketResult.GetForClosingTicket.GetForClosingTicketSubCategory
    //                                              {
    //                                                  TicketSubCategoryId = t.Id,
    //                                                  SubCategoryId = t.SubCategoryId,
    //                                                  SubCategory_Description = t.SubCategory.SubCategoryDescription,
    //                                              }).ToList(),
    //                      Notes = x.Notes,
    //                      IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
    //                      ApproverLists = x.ApproverTickets
    //                      .Where(x => x.IsApprove != null)
    //                      .Select(x => new ApproverList
    //                      {
    //                          ApproverName = x.User.Fullname,
    //                          Approver_Level = x.ApproverLevel.Value

    //                      }).ToList(),

    //                      GetAttachmentForClosingTickets = x.TicketAttachments.Select(x => new GetAttachmentForClosingTicket
    //                      {
    //                          TicketAttachmentId = x.Id,
    //                          Attachment = x.Attachment,
    //                          FileName = x.FileName,
    //                          FileSize = x.FileSize,

    //                      }).ToList(),

    //                  }).ToList(),

    //                    GetForOnHolds = x.TicketOnHolds
    //                    .Where(x => x.IsHold == false && x.IsActive)
    //                    .Select(h => new GetOpenTicketResult.GetForOnHold
    //                    {
    //                        Id = h.Id,
    //                        Reason = h.Reason,
    //                        AddedBy = h.AddedByUser.Fullname,
    //                        CreatedAt = h.CreatedAt,
    //                        IsHold = h.IsHold,
    //                        ResumeAt = h.ResumeAt,
    //                        IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
    //                        GetAttachmentForOnHoldOpenTickets = h.TicketAttachments
    //                        .Select(t => new GetOpenTicketResult.GetForOnHold.GetAttachmentForOnHoldOpenTicket
    //                        {
    //                            TicketAttachmentId = t.Id,
    //                            Attachment = t.Attachment,
    //                            FileName = t.FileName,
    //                            FileSize = t.FileSize,

    //                        }).ToList(),

    //                    }).ToList(),

    //                    GetOnHolds = x.TicketOnHolds
    //                    .Where(x => x.IsHold == true && x.IsActive)
    //                    .Select(h => new GetOpenTicketResult.GetOnHold
    //                    {
    //                        Id = h.Id,
    //                        Reason = h.Reason,
    //                        AddedBy = h.AddedByUser.Fullname,
    //                        CreatedAt = h.CreatedAt,
    //                        IsHold = h.IsHold,
    //                        ResumeAt = h.ResumeAt,
    //                        IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
    //                        GetAttachmentOnHoldOpenTickets = h.TicketAttachments
    //                        .Select(t => new GetOpenTicketResult.GetOnHold.GetAttachmentOnHoldOpenTicket
    //                        {
    //                            TicketAttachmentId = t.Id,
    //                            Attachment = t.Attachment,
    //                            FileName = t.FileName,
    //                            FileSize = t.FileSize,

    //                        }).ToList(),

    //                    }).ToList(),

    //                    GetForTransferTickets = x.TransferTicketConcerns
    //                    .Where(x => x.IsActive == true && x.IsTransfer == false && x.TransferBy == request.UserId)
    //                    .Select(x => new GetOpenTicketResult.GetForTransferTicket
    //                    {
    //                        TransferTicketConcernId = x.Id,
    //                        Transfer_Remarks = x.TransferRemarks,
    //                        Transfer_To = x.TransferTo,
    //                        Transfer_To_Name = x.TransferToUser.Fullname,
    //                        Current_Target_Date = x.Current_Target_Date,
    //                        IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
    //                        GetAttachmentForTransferTickets = x.TicketAttachments.Select(x => new GetOpenTicketResult.GetForTransferTicket.GetAttachmentForTransferTicket
    //                        {
    //                            TicketAttachmentId = x.Id,
    //                            Attachment = x.Attachment,
    //                            FileName = x.FileName,
    //                            FileSize = x.FileSize,

    //                        }).ToList(),

    //                    })
    //                    .ToList(),

    //                    Transaction_Date = x.ticketHistories.Max(x => x.TransactionDate).Value,
    //                    Aging_Days = x.Closed_At != null ? EF.Functions.DateDiffDay(x.TargetDate.Value.Date, x.Closed_At.Value.Date)
    //                    : EF.Functions.DateDiffDay(x.TargetDate.Value.Date, DateTime.Now.Date)

    //                });


    //            return await PagedList<GetOpenTicketResult>.CreateAsync(results, request.PageNumber, request.PageSize);
    //        }
    //    }
    //}

    //public partial class GetOpenTicket
    //{
    //    public class Handler : IRequestHandler<GetOpenTicketQuery, PagedList<GetOpenTicketResult>>
    //    {
    //        private readonly MisDbContext _context;

    //        public Handler(MisDbContext context)
    //        {
    //            _context = context;
    //        }

    //        public async Task<PagedList<GetOpenTicketResult>> Handle(GetOpenTicketQuery request, CancellationToken cancellationToken)
    //        {
    //            var dateToday = DateTime.Today;

    //            // Pre-fetch user permissions once
    //            var allUserList = await _context.UserRoles
    //                .AsNoTracking()
    //                .Select(x => new
    //                {
    //                    x.Id,
    //                    x.UserRoleName,
    //                    x.Permissions
    //                })
    //                .ToListAsync(cancellationToken);

    //            var receiverPermissionList = allUserList
    //                 .Where(x => x.Permissions.Contains(TicketingConString.Receiver))
    //                 .Select(x => x.UserRoleName)
    //                 .ToList();

    //            var issueHandlerPermissionList = allUserList
    //                .Where(x => x.Permissions.Contains(TicketingConString.IssueHandler))
    //                .Select(x => x.UserRoleName)
    //                .ToList();

    //            // Build the base query with necessary includes
    //            IQueryable<TicketConcern> ticketConcernQuery = _context.TicketConcerns
    //                .AsNoTracking()
    //                .Where(x => x.IsActive)
    //                .Include(x => x.User)
    //                    .ThenInclude(u => u.SubUnit)
    //                .Include(x => x.RequestorByUser)
    //                    .ThenInclude(u => u.Department)
    //                .Include(x => x.RequestorByUser)
    //                    .ThenInclude(u => u.SubUnit)
    //                .Include(x => x.RequestConcern)
    //                    .ThenInclude(rc => rc.Company)
    //                .Include(x => x.RequestConcern)
    //                    .ThenInclude(rc => rc.BusinessUnit)
    //                .Include(x => x.RequestConcern)
    //                    .ThenInclude(rc => rc.Unit)
    //                .Include(x => x.RequestConcern)
    //                    .ThenInclude(rc => rc.Location)
    //                .Include(x => x.RequestConcern)
    //                    .ThenInclude(rc => rc.Channel)
    //                .Include(x => x.RequestConcern)
    //                    .ThenInclude(rc => rc.BackJob)
    //                .Include(x => x.AddedByUser)
    //                .Include(x => x.ModifiedByUser)
    //                .AsSplitQuery();

    //            // Apply filters early to reduce dataset
    //            if (!string.IsNullOrEmpty(request.Search))
    //            {
    //                ticketConcernQuery = ticketConcernQuery
    //                    .Where(x => x.User.Fullname.ToLower().Contains(request.Search.ToLower())
    //                        || x.User.SubUnit.SubUnitName.ToLower().Contains(request.Search.ToLower())
    //                        || x.Id.ToString().Contains(request.Search));
    //            }

    //            if (request.Status is not null)
    //            {
    //                ticketConcernQuery = ticketConcernQuery
    //                    .Where(x => x.IsActive == request.Status);
    //            }

    //            if (request.Date_From is not null && request.Date_To is not null)
    //            {
    //                ticketConcernQuery = ticketConcernQuery
    //                    .Where(x => x.TargetDate >= request.Date_From.Value && x.TargetDate <= request.Date_To.Value);
    //            }

    //            // Apply concern status filter
    //            if (!string.IsNullOrEmpty(request.Concern_Status))
    //            {
    //                switch (request.Concern_Status)
    //                {
    //                    case TicketingConString.PendingRequest:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == false && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.ForApprovalTargetDate:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == false && x.ConcernStatus == TicketingConString.ForApprovalTargetDate);
    //                        break;

    //                    case TicketingConString.Open:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == true && x.ConcernStatus == TicketingConString.OnGoing || x.IsTransfer != false
    //                            && x.IsClosedApprove == null && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.ForTransfer:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == null)
    //                            .Where(x => x.TransferTicketConcerns
    //                            .FirstOrDefault(x => x.IsActive == true && x.IsTransfer == false)
    //                            .TransferBy == request.UserId);
    //                        break;

    //                    case TicketingConString.ForOnHold:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == false);
    //                        break;

    //                    case TicketingConString.OnHold:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == true);
    //                        break;

    //                    case TicketingConString.ForClosing:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == false && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.NotConfirm:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.Closed:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null);
    //                        break;

    //                    default:
    //                        return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                }
    //            }

    //            // Apply history status filter
    //            if (!string.IsNullOrEmpty(request.History_Status))
    //            {
    //                switch (request.History_Status)
    //                {
    //                    case TicketingConString.PendingRequest:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == false && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.Open:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == true || x.IsTransfer != false
    //                            && x.IsClosedApprove == null && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.ForTransfer:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == null)
    //                            .Where(x => x.TransferTicketConcerns
    //                            .FirstOrDefault(x => x.IsActive == true && x.IsTransfer == false)
    //                            .TransferBy == request.UserId);
    //                        break;
    //                    case TicketingConString.ForOnHold:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == false);
    //                        break;

    //                    case TicketingConString.OnHold:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == true);
    //                        break;

    //                    case TicketingConString.ForClosing:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == false && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.NotConfirm:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.Closed:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null);
    //                        break;

    //                    default:
    //                        return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                }
    //            }

    //            // Apply user type filter
    //            if (!string.IsNullOrEmpty(request.UserType))
    //            {
    //                if (request.UserType == TicketingConString.Approver)
    //                {
    //                    ticketConcernQuery = ticketConcernQuery.Where(x => x.AddedByUser.Id == request.UserId);
    //                }
    //                else if (request.UserType == TicketingConString.IssueHandler)
    //                {
    //                    ticketConcernQuery = ticketConcernQuery.Where(x => x.AssignTo == request.UserId);
    //                }
    //                else if (request.UserType == TicketingConString.Receiver)
    //                {
    //                    var listOfRequest = await ticketConcernQuery
    //                        .Select(x => x.User.BusinessUnitId)
    //                        .ToListAsync(cancellationToken);

    //                    var businessUnitDefault = await _context.BusinessUnits
    //                    .AsNoTracking()
    //                    .Where(x => x.IsActive == true)
    //                    .Where(x => listOfRequest.Contains(x.Id))
    //                    .Select(x => x.Id)
    //                    .ToListAsync(cancellationToken);

    //                    var receiverList = await _context.Receivers
    //                        .AsNoTrackingWithIdentityResolution()
    //                        .Include(x => x.BusinessUnit)
    //                        .AsSplitQuery()
    //                        .Where(x => businessUnitDefault.Contains(x.BusinessUnitId.Value) && x.IsActive == true &&
    //                         x.UserId == request.UserId)
    //                        .Select(x => x.BusinessUnitId)
    //                        .ToListAsync(cancellationToken);

    //                    if (receiverPermissionList.Any(x => x.Contains(request.Role) && receiverList.Any()))
    //                    {
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => receiverList.Contains(x.RequestorByUser.BusinessUnitId));
    //                    }
    //                    else
    //                    {
    //                        return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                    }
    //                }
    //                else
    //                {
    //                    return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                }
    //            }

    //            // Apply ordering
    //            if (request.Ascending is not null)
    //            {
    //                ticketConcernQuery = request.Ascending.Value
    //                    ? ticketConcernQuery.OrderBy(x => x.Id)
    //                    : ticketConcernQuery.OrderByDescending(x => x.Id);
    //            }
    //            else
    //            {
    //                ticketConcernQuery = ticketConcernQuery.OrderBy(x => x.Id);
    //            }

    //            // Check if query has any results before proceeding
    //            if (!await ticketConcernQuery.AnyAsync(cancellationToken))
    //            {
    //                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //            }

    //            // Project to result with optimized selection
    //            var results = ticketConcernQuery
    //                .Select(x => new GetOpenTicketResult
    //                {
    //                    Closed_Status = x.TargetDate.HasValue && x.Closed_At.HasValue && x.IsClosedApprove == true
    //                        ? (x.TargetDate.Value.Day >= x.Closed_At.Value.Day ? TicketingConString.OnTime : TicketingConString.Delay)
    //                        : null,

    //                    TicketConcernId = x.Id,
    //                    RequestConcernId = x.RequestConcernId,
    //                    Concern_Description = x.RequestConcern.Concern,
    //                    Company_Code = x.RequestConcern.Company.CompanyCode,
    //                    Company_Name = x.RequestConcern.Company.CompanyName,
    //                    BusinessUnit_Code = x.RequestConcern.BusinessUnit.BusinessCode,
    //                    BusinessUnit_Name = x.RequestConcern.BusinessUnit.BusinessName,
    //                    Department_Code = x.RequestorByUser.Department.DepartmentCode,
    //                    Department_Name = x.RequestorByUser.Department.DepartmentName,
    //                    Unit_Code = x.RequestConcern.Unit.UnitCode,
    //                    Unit_Name = x.RequestConcern.Unit.UnitName,
    //                    SubUnit_Code = x.RequestorByUser.SubUnit.SubUnitCode,
    //                    SubUnit_Name = x.User.SubUnit.SubUnitName,
    //                    Location_Code = x.RequestConcern.Location.LocationCode,
    //                    Location_Name = x.RequestConcern.Location.LocationName,
    //                    Requestor_By = x.RequestorBy,
    //                    Requestor_Name = x.RequestorByUser.Fullname,
    //                    GetOpenTicketCategories = x.RequestConcern.TicketCategories
    //                                                                .Select(t => new GetOpenTicketResult.GetOpenTicketCategory
    //                                                                {
    //                                                                    TicketCategoryId = t.Id,
    //                                                                    CategoryId = t.CategoryId,
    //                                                                    Category_Description = t.Category.CategoryDescription,

    //                                                                }).ToList(),

    //                    GetOpenTicketSubCategories = x.RequestConcern.TicketSubCategories
    //                                                                .Select(t => new GetOpenTicketResult.GetOpenTicketSubCategory
    //                                                                {
    //                                                                    TicketSubCategoryId = t.Id,
    //                                                                    SubCategoryId = t.SubCategoryId,
    //                                                                    SubCategory_Description = t.SubCategory.SubCategoryDescription,
    //                                                                }).ToList(),

    //                    Date_Needed = x.RequestConcern.DateNeeded,
    //                    Notes = x.RequestConcern.Notes,
    //                    Contact_Number = x.RequestConcern.ContactNumber,
    //                    Request_Type = x.RequestConcern.RequestType,
    //                    BackJobId = x.RequestConcern.BackJobId,
    //                    Back_Job_Concern = x.RequestConcern.BackJob.Concern,
    //                    ChannelId = x.RequestConcern.ChannelId,
    //                    Channel_Name = x.RequestConcern.Channel.ChannelName,
    //                    UserId = x.UserId,
    //                    Issue_Handler = x.User.Fullname,
    //                    Target_Date = x.TargetDate,

    //                    // Ticket status logic
    //                    Ticket_Status = x.IsApprove == false && x.OnHold == null ? TicketingConString.PendingRequest
    //                                    : x.IsApprove == true != false && x.IsTransfer != false && x.IsClosedApprove == null && x.OnHold == null ? TicketingConString.OpenTicket
    //                                    : x.TransferTicketConcerns.FirstOrDefault(x => x.IsTransfer == false && x.IsActive == true)
    //                                    .TransferBy == request.UserId ? TicketingConString.ForTransfer
    //                                    : x.OnHold == false ? TicketingConString.ForOnHold
    //                                    : x.OnHold == true ? TicketingConString.OnHold
    //                                    : x.IsClosedApprove == false && x.OnHold == null ? TicketingConString.ForClosing
    //                                    : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null ? TicketingConString.NotConfirm
    //                                    : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null ? TicketingConString.Closed
    //                                    : "Unknown",

    //                    Added_By = x.AddedByUser.Fullname,
    //                    Created_At = x.CreatedAt,
    //                    Remarks = x.Remarks,
    //                    Modified_By = x.ModifiedByUser.Fullname,
    //                    Updated_At = x.UpdatedAt,
    //                    IsActive = x.IsActive,
    //                    Is_Closed = x.IsClosedApprove,
    //                    Closed_At = x.Closed_At,
    //                    Is_Transfer = x.IsTransfer,
    //                    Transfer_At = x.TransferAt,
    //GetForClosingTickets = x.ClosingTickets
    //                   .Where(x => x.IsActive == true && x.IsRejectClosed == false)
    //                   .Where(x => x.IsClosing == true ? x.IsClosing == true : x.IsClosing == false)
    //                  .Select(x => new GetOpenTicketResult.GetForClosingTicket
    //                  {
    //                      ClosingTicketId = x.Id,
    //                      Remarks = x.RejectRemarks,
    //                      Resolution = x.Resolution,
    //                      ForClosingTicketTechnicians = x.ticketTechnicians.
    //                      Select(t => new GetOpenTicketResult.GetForClosingTicket.ForClosingTicketTechnician
    //                      {
    //                          TicketTechnicianId = t.Id,
    //                          Technician_By = t.TechnicianBy,
    //                          Fullname = t.TechnicianByUser.Fullname,

    //                      }).ToList(),

    //                      GetForClosingTicketCategories = x.TicketConcern.RequestConcern.TicketCategories
    //                                              .Select(t => new GetOpenTicketResult.GetForClosingTicket.GetForClosingTicketCategory
    //                                              {
    //                                                  TicketCategoryId = t.Id,
    //                                                  CategoryId = t.CategoryId,
    //                                                  Category_Description = t.Category.CategoryDescription,

    //                                              }).ToList(),

    //                      GetForClosingTicketSubCategories = x.TicketConcern.RequestConcern.TicketSubCategories
    //                                              .Select(t => new GetOpenTicketResult.GetForClosingTicket.GetForClosingTicketSubCategory
    //                                              {
    //                                                  TicketSubCategoryId = t.Id,
    //                                                  SubCategoryId = t.SubCategoryId,
    //                                                  SubCategory_Description = t.SubCategory.SubCategoryDescription,
    //                                              }).ToList(),
    //                      Notes = x.Notes,
    //                      IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
    //                      ApproverLists = x.ApproverTickets
    //                      .Where(x => x.IsApprove != null)
    //                      .Select(x => new ApproverList
    //                      {
    //                          ApproverName = x.User.Fullname,
    //                          Approver_Level = x.ApproverLevel.Value

    //                      }).ToList(),

    //                      GetAttachmentForClosingTickets = x.TicketAttachments.Select(x => new GetAttachmentForClosingTicket
    //                      {
    //                          TicketAttachmentId = x.Id,
    //                          Attachment = x.Attachment,
    //                          FileName = x.FileName,
    //                          FileSize = x.FileSize,

    //                      }).ToList(),

    //                  }).ToList(),

    //                    GetForOnHolds = x.TicketOnHolds
    //                    .Where(x => x.IsHold == false && x.IsActive)
    //                    .Select(h => new GetOpenTicketResult.GetForOnHold
    //                    {
    //                        Id = h.Id,
    //                        Reason = h.Reason,
    //                        AddedBy = h.AddedByUser.Fullname,
    //                        CreatedAt = h.CreatedAt,
    //                        IsHold = h.IsHold,
    //                        ResumeAt = h.ResumeAt,
    //                        IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
    //                        GetAttachmentForOnHoldOpenTickets = h.TicketAttachments
    //                        .Select(t => new GetOpenTicketResult.GetForOnHold.GetAttachmentForOnHoldOpenTicket
    //                        {
    //                            TicketAttachmentId = t.Id,
    //                            Attachment = t.Attachment,
    //                            FileName = t.FileName,
    //                            FileSize = t.FileSize,

    //                        }).ToList(),

    //                    }).ToList(),

    //                    GetOnHolds = x.TicketOnHolds
    //                    .Where(x => x.IsHold == true && x.IsActive)
    //                    .Select(h => new GetOpenTicketResult.GetOnHold
    //                    {
    //                        Id = h.Id,
    //                        Reason = h.Reason,
    //                        AddedBy = h.AddedByUser.Fullname,
    //                        CreatedAt = h.CreatedAt,
    //                        IsHold = h.IsHold,
    //                        ResumeAt = h.ResumeAt,
    //                        IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
    //                        GetAttachmentOnHoldOpenTickets = h.TicketAttachments
    //                        .Select(t => new GetOpenTicketResult.GetOnHold.GetAttachmentOnHoldOpenTicket
    //                        {
    //                            TicketAttachmentId = t.Id,
    //                            Attachment = t.Attachment,
    //                            FileName = t.FileName,
    //                            FileSize = t.FileSize,

    //                        }).ToList(),

    //                    }).ToList(),

    //                    GetForTransferTickets = x.TransferTicketConcerns
    //                                        .Where(x => x.IsActive == true && x.IsTransfer == false && x.TransferBy == request.UserId)
    //                                        .Select(x => new GetOpenTicketResult.GetForTransferTicket
    //                                        {
    //                                            TransferTicketConcernId = x.Id,
    //                                            Transfer_Remarks = x.TransferRemarks,
    //                                            Transfer_To = x.TransferTo,
    //                                            Transfer_To_Name = x.TransferToUser.Fullname,
    //                                            Current_Target_Date = x.Current_Target_Date,
    //                                            IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
    //                                            GetAttachmentForTransferTickets = x.TicketAttachments.Select(x => new GetOpenTicketResult.GetForTransferTicket.GetAttachmentForTransferTicket
    //                                            {
    //                                                TicketAttachmentId = x.Id,
    //                                                Attachment = x.Attachment,
    //                                                FileName = x.FileName,
    //                                                FileSize = x.FileSize,

    //                                            }).ToList(),

    //                                        })
    //                                        .ToList(),

    //                    Transaction_Date = x.ticketHistories.Any() ? x.ticketHistories.Max(h => h.TransactionDate).Value : x.CreatedAt,
    //                    Aging_Days = x.Closed_At != null
    //                        ? EF.Functions.DateDiffDay(x.TargetDate.Value.Date, x.Closed_At.Value.Date)
    //                        : EF.Functions.DateDiffDay(x.TargetDate.Value.Date, DateTime.Now.Date)
    //                });

    //            return await PagedList<GetOpenTicketResult>.CreateAsync(results, request.PageNumber, request.PageSize);
    //        }


    //    }
    //}


    ///KKEDIT

    //public partial class GetOpenTicket
    //{
    //    public class Handler : IRequestHandler<GetOpenTicketQuery, PagedList<GetOpenTicketResult>>
    //    {
    //        private readonly MisDbContext _context;

    //        public Handler(MisDbContext context)
    //        {
    //            _context = context;
    //        }

    //        public async Task<PagedList<GetOpenTicketResult>> Handle(GetOpenTicketQuery request, CancellationToken cancellationToken)
    //        {
    //            var dateToday = DateTime.Today;

    //            var allUserList = await _context.UserRoles
    //                .AsNoTracking()
    //                .Select(x => new
    //                {
    //                    x.Id,
    //                    x.UserRoleName,
    //                    x.Permissions
    //                })
    //                .ToListAsync(cancellationToken);

    //            var receiverPermissionList = allUserList
    //                 .Where(x => x.Permissions.Contains(TicketingConString.Receiver))
    //                 .Select(x => x.UserRoleName)
    //                 .ToList();

    //            List<int> receiverBusinessUnits = new List<int>();
    //            if (!string.IsNullOrEmpty(request.UserType) && request.UserType == TicketingConString.Receiver)
    //            {
    //                if (receiverPermissionList.Any(x => x.Contains(request.Role)))
    //                {
    //                    receiverBusinessUnits = await _context.Receivers
    //                        .AsNoTracking()
    //                        .Where(x => x.IsActive == true && x.UserId == request.UserId && x.BusinessUnitId.HasValue)
    //                        .Select(x => x.BusinessUnitId.Value)
    //                        .ToListAsync(cancellationToken);

    //                    if (!receiverBusinessUnits.Any())
    //                    {
    //                        return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                    }
    //                }
    //                else
    //                {
    //                    return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                }
    //            }

    //            IQueryable<TicketConcern> ticketConcernQuery = _context.TicketConcerns
    //                .AsNoTracking()
    //                .Where(x => x.IsActive);

    //            if (!string.IsNullOrEmpty(request.Search))
    //            {
    //                ticketConcernQuery = ticketConcernQuery
    //                    .Where(x => x.User.Fullname.ToLower().Contains(request.Search.ToLower())
    //                        || x.User.SubUnit.SubUnitName.ToLower().Contains(request.Search.ToLower())
    //                        || x.Id.ToString().Contains(request.Search)
    //                        || x.RequestConcern.Concern.ToLower().Contains(request.Search.ToLower()));
    //            }

    //            if (request.Status is not null)
    //            {
    //                ticketConcernQuery = ticketConcernQuery
    //                    .Where(x => x.IsActive == request.Status);
    //            }

    //            if (request.Date_From is not null && request.Date_To is not null)
    //            {
    //                ticketConcernQuery = ticketConcernQuery
    //                    .Where(x => x.TargetDate >= request.Date_From.Value && x.TargetDate <= request.Date_To.Value);
    //            }

    //            // Apply concern status filter
    //            if (!string.IsNullOrEmpty(request.Concern_Status))
    //            {
    //                switch (request.Concern_Status)
    //                {
    //                    case TicketingConString.PendingRequest:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == false && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.ForApprovalTicket:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == false && x.ConcernStatus == TicketingConString.ForApprovalTicket && x.IsAssigned == true);
    //                        break;

    //                    case TicketingConString.Open:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == true && x.ConcernStatus == TicketingConString.OnGoing  && x.IsClosedApprove == null && x.IsTransfer == null && x.OnHold == null || x.IsTransfer != false
    //                            && x.IsClosedApprove == null && x.OnHold == null && x.IsApprove == true );
    //                        break;

    //                    case TicketingConString.ForTransfer:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == null)
    //                            .Where(x => x.TransferTicketConcerns
    //                            .FirstOrDefault(x => x.IsActive == true && x.IsTransfer == false)
    //                            .TransferBy == request.UserId);
    //                        break;

    //                    case TicketingConString.ForOnHold:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == false);
    //                        break;

    //                    case TicketingConString.OnHold:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == true);
    //                        break;

    //                    case TicketingConString.ForClosing:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == false && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.NotConfirm:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.Closed:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null);
    //                        break;


    //                    case TicketingConString.DateRejected:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.ConcernStatus == TicketingConString.DateRejected);
    //                        break;

    //                    default:
    //                        return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                }
    //            }

    //            // Apply history status filter
    //            if (!string.IsNullOrEmpty(request.History_Status))
    //            {
    //                switch (request.History_Status)
    //                {
    //                    case TicketingConString.PendingRequest:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == false && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.Open:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == true || x.IsTransfer != false
    //                            && x.IsClosedApprove == null && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.ForTransfer:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == null)
    //                            .Where(x => x.TransferTicketConcerns
    //                            .FirstOrDefault(x => x.IsActive == true && x.IsTransfer == false)
    //                            .TransferBy == request.UserId);
    //                        break;
    //                    case TicketingConString.ForOnHold:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == false);
    //                        break;

    //                    case TicketingConString.OnHold:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == true);
    //                        break;

    //                    case TicketingConString.ForClosing:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == false && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.NotConfirm:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.Closed:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null);
    //                        break;

    //                    default:
    //                        return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                }
    //            }

    //            // Apply user type filter
    //            if (!string.IsNullOrEmpty(request.UserType))
    //            {
    //                if (request.UserType == TicketingConString.Approver)
    //                {
    //                    ticketConcernQuery = ticketConcernQuery.Where(x => x.AddedByUser.Id == request.UserId);
    //                }
    //                else if (request.UserType == TicketingConString.IssueHandler)
    //                {
    //                    ticketConcernQuery = ticketConcernQuery.Where(x => x.AssignTo == request.UserId);
    //                }
    //                else if (request.UserType == TicketingConString.Receiver)
    //                {

    //                    ticketConcernQuery = ticketConcernQuery
    //                        .Where(x => receiverBusinessUnits.Contains(x.RequestorByUser.BusinessUnitId));
    //                }
    //                else
    //                {
    //                    return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                }
    //            }


    //            if (request.Ascending == false)
    //            {
    //                ticketConcernQuery = ticketConcernQuery.OrderByDescending(x => x.Id);
    //            }


    //            if (request.AscendingDate == false)
    //            {
    //                ticketConcernQuery = ticketConcernQuery.OrderByDescending(x => x.TargetDate);

    //            }



    //            var totalCount = await ticketConcernQuery.CountAsync(cancellationToken);

    //            if (totalCount == 0)
    //            {
    //                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //            }


    //            var results = await ticketConcernQuery
    //                .Select(x => new GetOpenTicketResult
    //                {
    //                    Closed_Status = x.RequestConcern.Is_Confirm == null ? null :
    //                       x.IsClosedApprove == true ?
    //                       x.TargetDate.Value.Date >= x.Closed_At.Value.Date ?
    //                       TicketingConString.OnTime : 
    //                      TicketingConString.Delay : null,
    //                    TicketConcernId = x.Id,
    //                    RequestConcernId = x.RequestConcernId,
    //                    Severity = x.RequestConcern.Severity,
    //                    CompanyId = x.RequestConcern.CompanyId,
    //                    BusinessUnitId = x.RequestConcern.BusinessUnitId,
    //                    DepartmentId = x.RequestConcern.DepartmentId,
    //                    UnitId = x.RequestConcern.UnitId,
    //                    SubUnitid = x.RequestConcern.SubUnitId,
    //                    LocationId = x.RequestConcern.LocationId,
    //                    Concern_Description = x.RequestConcern.Concern,
    //                    Company_Code = x.RequestConcern.Company.CompanyCode,
    //                    Company_Name = x.RequestConcern.Company.CompanyName,
    //                    BusinessUnit_Code = x.RequestConcern.BusinessUnit.BusinessCode,
    //                    BusinessUnit_Name = x.RequestConcern.BusinessUnit.BusinessName,
    //                    Department_Code = x.RequestorByUser.Department.DepartmentCode,
    //                    Department_Name = x.RequestorByUser.Department.DepartmentName,
    //                    Unit_Code = x.RequestConcern.Unit.UnitCode,
    //                    Unit_Name = x.RequestConcern.Unit.UnitName,
    //                    SubUnit_Code = x.RequestorByUser.SubUnit.SubUnitCode,
    //                    SubUnit_Name = x.User.SubUnit.SubUnitName,
    //                    Location_Code = x.RequestConcern.Location.LocationCode,
    //                    Location_Name = x.RequestConcern.Location.LocationName,
    //                    Requestor_By = x.RequestorBy,
    //                    Requestor_Name = x.RequestorByUser.Fullname,
    //                    GetOpenTicketCategories = x.RequestConcern.TicketCategories
    //                                            .Select(t => new GetOpenTicketResult.GetOpenTicketCategory
    //                                            {
    //                                                TicketCategoryId = t.Id,
    //                                                CategoryId = t.CategoryId,
    //                                                Category_Description = t.Category.CategoryDescription,

    //                                            }).ToList(),

    //                    GetOpenTicketSubCategories = x.RequestConcern.TicketSubCategories
    //                                            .Select(t => new GetOpenTicketResult.GetOpenTicketSubCategory
    //                                            {
    //                                                TicketSubCategoryId = t.Id,
    //                                                SubCategoryId = t.SubCategoryId,
    //                                                SubCategory_Description = t.SubCategory.SubCategoryDescription,
    //                                            }).ToList(),

    //                    Date_Needed = x.RequestConcern.DateNeeded,
    //                    Notes = x.RequestConcern.Notes,
    //                    Contact_Number = x.RequestConcern.ContactNumber,
    //                    Request_Type = x.RequestConcern.RequestType,
    //                    BackJobId = x.RequestConcern.BackJobId,
    //                    Back_Job_Concern = x.RequestConcern.BackJob.Concern,
    //                    ChannelId = x.RequestConcern.ChannelId,
    //                    Channel_Name = x.RequestConcern.Channel.ChannelName,
    //                    ServiceProviderId = x.RequestConcern.ServiceProviderId,
    //                    ServiceProviderName = x.RequestConcern.ServiceProvider.ServiceProviderName,
    //                    TransferChannelId = x.RequestConcern.TransferChannelId,
    //                    Transfer_Channel_Name = x.RequestConcern.TransferChannel.ChannelName,
    //                    UserId = x.UserId,
    //                    Issue_Handler = x.User.Fullname,
    //                    Target_Date = x.TargetDate,
    //                    Ticket_Status = x.IsApprove == false && x.OnHold == null ? TicketingConString.PendingRequest
    //                                    : x.IsApprove == true != false && x.IsTransfer != false && x.IsClosedApprove == null && x.OnHold == null ? TicketingConString.OpenTicket
    //                                    : x.TransferTicketConcerns.FirstOrDefault(x => x.IsTransfer == false && x.IsActive == true)
    //                                    .TransferBy == request.UserId ? TicketingConString.ForTransfer
    //                                    : x.OnHold == false ? TicketingConString.ForOnHold
    //                                    : x.OnHold == true ? TicketingConString.OnHold
    //                                    : x.IsClosedApprove == false && x.OnHold == null ? TicketingConString.ForClosing
    //                                    : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null ? TicketingConString.NotConfirm
    //                                    : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null ? TicketingConString.Closed
    //                                    : "Unknown",

    //                    Added_By = x.AddedByUser.Fullname,
    //                    Created_At = x.CreatedAt,
    //                    Remarks = x.Remarks,
    //                    Modified_By = x.ModifiedByUser.Fullname,
    //                    Updated_At = x.UpdatedAt,
    //                    IsActive = x.IsActive,
    //                    Is_Closed = x.IsClosedApprove,
    //                    Closed_At = x.RequestConcern.Is_Confirm == null ? null : x.Closed_At,
    //                    Is_Transfer = x.IsTransfer,
    //                    Transfer_At = x.TransferAt,
    //                    GetForClosingTickets = x.ClosingTickets
    //                   .Where(x => x.IsActive == true && x.IsRejectClosed == false)
    //                   .Where(x => x.IsClosing == true ? x.IsClosing == true : x.IsClosing == false)
    //                  .Select(x => new GetOpenTicketResult.GetForClosingTicket
    //                  {
    //                      ClosingTicketId = x.Id,
    //                      Remarks = x.RejectRemarks,
    //                      Resolution = x.Resolution,
    //                      ForClosingTicketTechnicians = x.ticketTechnicians.
    //                      Select(t => new GetOpenTicketResult.GetForClosingTicket.ForClosingTicketTechnician
    //                      {
    //                          TicketTechnicianId = t.Id,
    //                          Technician_By = t.TechnicianBy,
    //                          Fullname = t.TechnicianByUser.Fullname,

    //                      }).ToList(),

    //                      GetForClosingTicketCategories = x.TicketConcern.RequestConcern.TicketCategories
    //                                              .Select(t => new GetOpenTicketResult.GetForClosingTicket.GetForClosingTicketCategory
    //                                              {
    //                                                  TicketCategoryId = t.Id,
    //                                                  CategoryId = t.CategoryId,
    //                                                  Category_Description = t.Category.CategoryDescription,

    //                                              }).ToList(),

    //                      GetForClosingTicketSubCategories = x.TicketConcern.RequestConcern.TicketSubCategories
    //                                              .Select(t => new GetOpenTicketResult.GetForClosingTicket.GetForClosingTicketSubCategory
    //                                              {
    //                                                  TicketSubCategoryId = t.Id,
    //                                                  SubCategoryId = t.SubCategoryId,
    //                                                  SubCategory_Description = t.SubCategory.SubCategoryDescription,
    //                                              }).ToList(),
    //                      Notes = x.Notes,
    //                      IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
    //                      ApproverLists = x.ApproverTickets
    //                      .Where(x => x.IsApprove != null)
    //                      .Select(x => new ApproverList
    //                      {
    //                          ApproverName = x.User.Fullname,
    //                          Approver_Level = x.ApproverLevel.Value

    //                      }).ToList(),

    //                      GetAttachmentForClosingTickets = x.TicketAttachments.Select(x => new GetAttachmentForClosingTicket
    //                      {
    //                          TicketAttachmentId = x.Id,
    //                          Attachment = x.Attachment,
    //                          FileName = x.FileName,
    //                          FileSize = x.FileSize,

    //                      }).ToList(),

    //                  }).ToList(),

    //                    GetForOnHolds = x.TicketOnHolds
    //                    .Where(x => x.IsHold == false && x.IsActive)
    //                    .Select(h => new GetOpenTicketResult.GetForOnHold
    //                    {
    //                        Id = h.Id,
    //                        Reason = h.Reason,
    //                        AddedBy = h.AddedByUser.Fullname,
    //                        CreatedAt = h.CreatedAt,
    //                        IsHold = h.IsHold,
    //                        ResumeAt = h.ResumeAt,
    //                        IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
    //                        GetAttachmentForOnHoldOpenTickets = h.TicketAttachments
    //                        .Select(t => new GetOpenTicketResult.GetForOnHold.GetAttachmentForOnHoldOpenTicket
    //                        {
    //                            TicketAttachmentId = t.Id,
    //                            Attachment = t.Attachment,
    //                            FileName = t.FileName,
    //                            FileSize = t.FileSize,

    //                        }).ToList(),

    //                    }).ToList(),

    //                    GetOnHolds = x.TicketOnHolds
    //                    .Where(x => x.IsHold == true && x.IsActive)
    //                    .Select(h => new GetOpenTicketResult.GetOnHold
    //                    {
    //                        Id = h.Id,
    //                        Reason = h.Reason,
    //                        AddedBy = h.AddedByUser.Fullname,
    //                        CreatedAt = h.CreatedAt,
    //                        IsHold = h.IsHold,
    //                        ResumeAt = h.ResumeAt,
    //                        IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
    //                        GetAttachmentOnHoldOpenTickets = h.TicketAttachments
    //                        .Select(t => new GetOpenTicketResult.GetOnHold.GetAttachmentOnHoldOpenTicket
    //                        {
    //                            TicketAttachmentId = t.Id,
    //                            Attachment = t.Attachment,
    //                            FileName = t.FileName,
    //                            FileSize = t.FileSize,

    //                        }).ToList(),

    //                    }).ToList(),

    //                    GetForTransferTickets = x.TransferTicketConcerns
    //                    .Where(x => x.IsActive == true && x.IsTransfer == false && x.TransferBy == request.UserId)
    //                    .Select(x => new GetOpenTicketResult.GetForTransferTicket
    //                    {
    //                        TransferTicketConcernId = x.Id,
    //                        Transfer_Remarks = x.TransferRemarks,
    //                        Transfer_To = x.TransferTo,
    //                        Transfer_To_Name = x.TransferToUser.Fullname,
    //                        Current_Target_Date = x.Current_Target_Date,
    //                        IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true) ? true : false,
    //                        GetAttachmentForTransferTickets = x.TicketAttachments.Select(x => new GetOpenTicketResult.GetForTransferTicket.GetAttachmentForTransferTicket
    //                        {
    //                            TicketAttachmentId = x.Id,
    //                            Attachment = x.Attachment,
    //                            FileName = x.FileName,
    //                            FileSize = x.FileSize,

    //                        }).ToList(),

    //                    })
    //                    .ToList(),

    //                    Transaction_Date = x.ticketHistories.Max(x => x.TransactionDate).Value,

    //                    Aging_Days = x.RequestConcern.Is_Confirm == null ? null : EF.Functions.DateDiffDay(x.DateApprovedAt.Value.Date, x.Closed_At.Value.Date) <= 0 ? 0 : EF.Functions.DateDiffDay(x.DateApprovedAt.Value.Date, x.Closed_At.Value.Date )

    //                }).ToListAsync();

    //            return new PagedList<GetOpenTicketResult>(results, totalCount, request.PageNumber, request.PageSize);
    //        }
    //    }
    //}



    //2025-09-18
    //public partial class GetOpenTicket
    //{
    //    public class Handler : IRequestHandler<GetOpenTicketQuery, PagedList<GetOpenTicketResult>>
    //    {
    //        private readonly MisDbContext _context;

    //        public Handler(MisDbContext context)
    //        {
    //            _context = context;
    //        }

    //        public async Task<PagedList<GetOpenTicketResult>> Handle(GetOpenTicketQuery request, CancellationToken cancellationToken)
    //        {
    //            var dateToday = DateTime.Today;

    //            // Optimization 1: Cache user roles in memory or use a more efficient query
    //            //List<int> receiverBusinessUnits = new List<int>();
    //            //if (!string.IsNullOrEmpty(request.UserType) && request.UserType == TicketingConString.Receiver)
    //            //{
    //            //    // Optimize: Direct query instead of loading all user roles
    //            //    var hasReceiverPermission = await _context.UserRoles
    //            //        .AsNoTracking()
    //            //        .AnyAsync(x => x.UserRoleName.Contains(request.Role) &&
    //            //                      x.Permissions.Contains(TicketingConString.Receiver), cancellationToken);

    //            //    if (hasReceiverPermission)
    //            //    {
    //            //        receiverBusinessUnits = await _context.Receivers
    //            //            .AsNoTracking()
    //            //            .Where(x => x.IsActive && x.UserId == request.UserId && x.BusinessUnitId.HasValue)
    //            //            .Select(x => x.BusinessUnitId.Value)
    //            //            .ToListAsync(cancellationToken);

    //            //        if (!receiverBusinessUnits.Any())
    //            //        {
    //            //            return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //            //        }
    //            //    }
    //            //    else
    //            //    {
    //            //        return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //            //    }
    //            //}

    //            IQueryable<TicketConcern> ticketConcernQuery = _context.TicketConcerns
    //                .AsNoTracking()
    //                .Include(x => x.ClosingTickets)
    //                .AsQueryable()
    //                .Where(x => x.IsActive == true && x.AssignTo == request.UserId);



    //            if (!string.IsNullOrEmpty(request.Concern_Status))
    //            {
    //                switch (request.Concern_Status)
    //                {
    //                    case TicketingConString.PendingRequest:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == false && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.ForApprovalTicket:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == false && x.ConcernStatus == TicketingConString.ForApprovalTicket && x.IsAssigned == true);
    //                        break;

    //                    case TicketingConString.Open:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.ConcernStatus == TicketingConString.OnGoing && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.ForTransfer:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == null)
    //                            .Where(x => x.TransferTicketConcerns
    //                            .FirstOrDefault(x => x.IsActive == true && x.IsTransfer == false)
    //                            .TransferBy == request.UserId);
    //                        break;

    //                    case TicketingConString.ForOnHold:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == false);
    //                        break;

    //                    case TicketingConString.OnHold:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == true);
    //                        break;

    //                    case TicketingConString.ForClosing:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == false && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.NotConfirm:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.Closed:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.DateRejected:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.ConcernStatus == TicketingConString.DateRejected);
    //                        break;

    //                    default:
    //                        return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                }
    //            }

    //            if (!string.IsNullOrEmpty(request.History_Status))
    //            {
    //                switch (request.History_Status)
    //                {
    //                    //case TicketingConString.PendingRequest:
    //                    //    ticketConcernQuery = ticketConcernQuery
    //                    //        .Where(x => x.IsApprove == false && x.OnHold == null);
    //                    //    break;

    //                    case TicketingConString.Open:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsApprove == true || x.IsTransfer != false
    //                            && x.IsClosedApprove == null && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.ForTransfer:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == null)
    //                            .Where(x => x.TransferTicketConcerns
    //                            .FirstOrDefault(x => x.IsActive == true && x.IsTransfer == false)
    //                            .TransferBy == request.UserId);
    //                        break;

    //                    case TicketingConString.ForOnHold:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == false);
    //                        break;

    //                    case TicketingConString.OnHold:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.OnHold == true);
    //                        break;

    //                    case TicketingConString.ForClosing:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == false && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.NotConfirm:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null);
    //                        break;

    //                    case TicketingConString.Closed:
    //                        ticketConcernQuery = ticketConcernQuery
    //                            .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null);
    //                        break;

    //                    default:
    //                        return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //                }
    //            }

    //            //if (!string.IsNullOrEmpty(request.UserType))
    //            //{
    //            //    if (request.UserType == TicketingConString.Approver)
    //            //    {
    //            //        ticketConcernQuery = ticketConcernQuery.Where(x => x.AddedByUser.Id == request.UserId);
    //            //    }
    //            //    else if (request.UserType == TicketingConString.IssueHandler)
    //            //    {
    //            //        ticketConcernQuery = ticketConcernQuery.Where(x => x.AssignTo == request.UserId);
    //            //    }
    //            //    else if (request.UserType == TicketingConString.Receiver)
    //            //    {
    //            //        ticketConcernQuery = ticketConcernQuery
    //            //            .Where(x => receiverBusinessUnits.Contains(x.RequestorByUser.BusinessUnitId));
    //            //    }
    //            //    else
    //            //    {
    //            //        return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //            //    }
    //            //}


    //            if (!string.IsNullOrEmpty(request.Search))
    //            {
    //                ticketConcernQuery = ticketConcernQuery
    //                    .Where(x => x.User.Fullname.ToLower().Contains(request.Search.ToLower())
    //                        || x.Id.ToString().Contains(request.Search)
    //                        || x.RequestConcern.Concern.ToLower().Contains(request.Search.ToLower()));
    //            }

    //            //if (request.Status is not null)
    //            //{
    //            //    ticketConcernQuery = ticketConcernQuery
    //            //        .Where(x => x.IsActive == request.Status);
    //            //}

    //            if (request.Date_From is not null && request.Date_To is not null)
    //            {
    //                ticketConcernQuery = ticketConcernQuery
    //                    .Where(x => x.TargetDate >= request.Date_From.Value && x.TargetDate <= request.Date_To.Value);
    //            }

    //            if (request.Ascending == false)
    //            {
    //                ticketConcernQuery = ticketConcernQuery.OrderByDescending(x => x.Id);
    //            }


    //            if (request.AscendingDate == false)
    //            {
    //                ticketConcernQuery = ticketConcernQuery.OrderByDescending(x => x.TargetDate);
    //            }



    //            var totalCount = await ticketConcernQuery.CountAsync(cancellationToken);

    //            if (totalCount == 0)
    //            {
    //                return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
    //            }

    //            //var pagedTicketIds = await ticketConcernQuery
    //            //    .Skip((request.PageNumber - 1) * request.PageSize)
    //            //    .Take(request.PageSize)
    //            //    .Select(x => x.Id)
    //            //    .ToListAsync(cancellationToken);

    //            //var pagedTicketConcerns = await ticketConcernQuery
    //            //    .AsNoTracking()
    //            //    .Where(x => pagedTicketIds.Contains(x.Id))
    //            //    .Select(x => new
    //            //    {
    //            //        x.Id,
    //            //        x.RequestConcernId,
    //            //        x.UserId,
    //            //        x.RequestorBy,
    //            //        x.TargetDate,
    //            //        x.IsApprove,
    //            //        x.OnHold,
    //            //        x.ConcernStatus,
    //            //        x.IsTransfer,
    //            //        x.IsClosedApprove,
    //            //        x.CreatedAt,
    //            //        x.UpdatedAt,
    //            //        x.Closed_At,
    //            //        x.TransferAt,
    //            //        x.IsActive,
    //            //        x.Remarks,

    //            //        TransferChannelId = x.RequestConcern.TransferChannelId,
    //            //        TransferChannelName = x.RequestConcern.TransferChannel.ChannelName,
    //            //        IssueHandlerName = x.User.Fullname,
    //            //        IssueHandlerSubUnit = x.User.OneChargingMIS.sub_unit_name,
    //            //        RequestorName = x.RequestorByUser.Fullname,
    //            //        RequestorDepartmentCode = x.RequestorByUser.OneChargingMIS.department_code,
    //            //        RequestorDepartmentName = x.RequestorByUser.OneChargingMIS.department_name,
    //            //        RequestorSubUnitCode = x.RequestorByUser.OneChargingMIS.sub_unit_code,
    //            //        RequestorBusinessUnitId = x.RequestorByUser.OneChargingMIS.business_unit_id,
    //            //        ConcernDescription = x.RequestConcern.Concern,
    //            //        DateNeeded = x.RequestConcern.DateNeeded,
    //            //        Notes = x.RequestConcern.Notes,
    //            //        ContactNumber = x.RequestConcern.ContactNumber,
    //            //        RequestType = x.RequestConcern.RequestType,
    //            //        BackJobId = x.RequestConcern.BackJobId,
    //            //        BackJobConcern = x.RequestConcern.BackJob.Concern,
    //            //        ChannelId = x.RequestConcern.ChannelId,
    //            //        ChannelName = x.RequestConcern.Channel.ChannelName,
    //            //        IsConfirm = x.RequestConcern.Is_Confirm,
    //            //        CompanyCode = x.RequestConcern.OneChargingMIS.company_code,
    //            //        CompanyName = x.RequestConcern.OneChargingMIS.company_name,
    //            //        BusinessUnitCode = x.RequestConcern.OneChargingMIS.business_unit_code,
    //            //        BusinessUnitName = x.RequestConcern.OneChargingMIS.business_unit_name,
    //            //        UnitCode = x.RequestConcern.OneChargingMIS.department_unit_code,
    //            //        UnitName = x.RequestConcern.OneChargingMIS.department_unit_name,
    //            //        LocationCode = x.RequestConcern.OneChargingMIS.location_code,
    //            //        LocationName = x.RequestConcern.OneChargingMIS.location_name,
    //            //        AddedByName = x.AddedByUser.Fullname,
    //            //        ModifiedByName = x.ModifiedByUser.Fullname,
    //            //        MaxTransactionDate = x.ticketHistories.Any() ? x.ticketHistories.Max(h => h.TransactionDate) : null
    //            //    })
    //            //    .ToListAsync(cancellationToken);


    //            //var requestConcernIds = pagedTicketConcerns.Select(x => x.RequestConcernId).ToList();


    //            //var ticketCategories = await _context.TicketCategories
    //            //    .AsNoTracking()
    //            //    .Where(tc => requestConcernIds.Contains(tc.RequestConcernId))
    //            //    .Select(tc => new
    //            //    {
    //            //        tc.RequestConcernId,
    //            //        tc.Id,
    //            //        tc.CategoryId,
    //            //        CategoryDescription = tc.Category.CategoryDescription
    //            //    })
    //            //    .ToListAsync(cancellationToken);

    //            //var ticketSubCategories = await _context.TicketSubCategories
    //            //    .AsNoTracking()
    //            //    .Where(tsc => requestConcernIds.Contains(tsc.RequestConcernId))
    //            //    .Select(tsc => new
    //            //    {
    //            //        tsc.RequestConcernId,
    //            //        tsc.Id,
    //            //        tsc.SubCategoryId,
    //            //        SubCategoryDescription = tsc.SubCategory.SubCategoryDescription
    //            //    })
    //            //    .ToListAsync(cancellationToken);


    //            //var closingTickets = new List<dynamic>();
    //            //var ticketOnHolds = new List<dynamic>();
    //            //var transferTickets = new List<dynamic>();


    //            var results = await ticketConcernQuery
    //                .Select(x => new GetOpenTicketResult
    //                {
    //                    Closed_Status = x.RequestConcern.Is_Confirm == null ? null :
    //                       x.IsClosedApprove == true ?
    //                       x.TargetDate.Value.Date >= x.Closed_At.Value.Date ?
    //                       TicketingConString.OnTime :
    //                      TicketingConString.Delay : null,
    //                    TicketConcernId = x.Id,
    //                    RequestConcernId = x.RequestConcernId,
    //                    Severity = x.RequestConcern.Severity,
    //                    CompanyId = x.RequestConcern.CompanyId,
    //                    BusinessUnitId = x.RequestConcern.BusinessUnitId,
    //                    DepartmentId = x.RequestConcern.DepartmentId,
    //                    UnitId = x.RequestConcern.UnitId,
    //                    SubUnitid = x.RequestConcern.SubUnitId,
    //                    LocationId = x.RequestConcern.LocationId,
    //                    Concern_Description = x.RequestConcern.Concern,
    //                    Company_Code = x.RequestConcern.OneChargingMIS.company_code,
    //                    Company_Name = x.RequestConcern.OneChargingMIS.company_name,
    //                    BusinessUnit_Code = x.RequestConcern.OneChargingMIS.business_unit_code,
    //                    BusinessUnit_Name = x.RequestConcern.OneChargingMIS.business_unit_name,
    //                    Department_Code = x.RequestorByUser.OneChargingMIS.department_code,
    //                    Department_Name = x.RequestorByUser.OneChargingMIS.department_name,
    //                    Unit_Code = x.RequestConcern.OneChargingMIS.department_unit_code,
    //                    Unit_Name = x.RequestConcern.OneChargingMIS.department_unit_name,
    //                    SubUnit_Code = x.RequestorByUser.OneChargingMIS.sub_unit_code,
    //                    SubUnit_Name = x.User.OneChargingMIS.sub_unit_name,
    //                    Location_Code = x.RequestConcern.OneChargingMIS.location_code,
    //                    Location_Name = x.RequestConcern.OneChargingMIS.location_name,
    //                    Requestor_By = x.RequestorBy,
    //                    Requestor_Name = x.RequestorByUser.Fullname,
    //                    GetOpenTicketCategories = x.RequestConcern.TicketCategories
    //                                            .Select(t => new GetOpenTicketResult.GetOpenTicketCategory
    //                                            {
    //                                                TicketCategoryId = t.Id,
    //                                                CategoryId = t.CategoryId,
    //                                                Category_Description = t.Category.CategoryDescription,
    //                                            }).ToList(),

    //                    GetOpenTicketSubCategories = x.RequestConcern.TicketSubCategories
    //                                            .Select(t => new GetOpenTicketResult.GetOpenTicketSubCategory
    //                                            {
    //                                                TicketSubCategoryId = t.Id,
    //                                                SubCategoryId = t.SubCategoryId,
    //                                                SubCategory_Description = t.SubCategory.SubCategoryDescription,
    //                                            }).ToList(),

    //                    Date_Needed = x.RequestConcern.DateNeeded,
    //                    Notes = x.RequestConcern.Notes,
    //                    Contact_Number = x.RequestConcern.ContactNumber,
    //                    Request_Type = x.RequestConcern.RequestType,
    //                    BackJobId = x.RequestConcern.BackJobId,
    //                    Back_Job_Concern = x.RequestConcern.BackJob.Concern,
    //                    ChannelId = x.RequestConcern.ChannelId,
    //                    Channel_Name = x.RequestConcern.Channel.ChannelName,
    //                    ServiceProviderId = x.RequestConcern.ServiceProviderId,
    //                    ServiceProviderName = x.RequestConcern.ServiceProvider.ServiceProviderName,
    //                    TransferChannelId = x.RequestConcern.TransferChannelId,
    //                    Transfer_Channel_Name = x.RequestConcern.TransferChannel.ChannelName,
    //                    UserId = x.UserId,
    //                    Issue_Handler = x.User.Fullname,
    //                    Target_Date = x.TargetDate,
    //                    Ticket_Status = x.IsApprove == false && x.OnHold == null ? TicketingConString.PendingRequest
    //                                    : x.IsApprove == true != false && x.IsTransfer != false && x.IsClosedApprove == null && x.OnHold == null ? TicketingConString.OpenTicket
    //                                    : x.TransferTicketConcerns.FirstOrDefault(x => x.IsTransfer == false && x.IsActive == true)
    //                                    .TransferBy == request.UserId ? TicketingConString.ForTransfer
    //                                    : x.OnHold == false ? TicketingConString.ForOnHold
    //                                    : x.OnHold == true ? TicketingConString.OnHold
    //                                    : x.IsClosedApprove == false && x.OnHold == null ? TicketingConString.ForClosing
    //                                    : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null ? TicketingConString.NotConfirm
    //                                    : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null ? TicketingConString.Closed
    //                                    : "Unknown",

    //                    Added_By = x.AddedByUser.Fullname,
    //                    Created_At = x.CreatedAt,
    //                    Remarks = x.Remarks,
    //                    Modified_By = x.ModifiedByUser.Fullname,
    //                    Updated_At = x.UpdatedAt,
    //                    IsActive = x.IsActive,
    //                    Is_Closed = x.IsClosedApprove,
    //                    Closed_At = x.RequestConcern.Is_Confirm == null ? null : x.Closed_At,
    //                    Is_Transfer = x.IsTransfer,
    //                    Transfer_At = x.TransferAt,
    //                    GetForClosingTickets = x.ClosingTickets
    //                   .Where(x => x.IsActive == true && x.IsRejectClosed == false)
    //                   .Where(x => x.IsClosing == true ? x.IsClosing == true : x.IsClosing == false)
    //                  .Select(x => new GetOpenTicketResult.GetForClosingTicket
    //                  {
    //                      ClosingTicketId = x.Id,
    //                      Remarks = x.RejectRemarks,
    //                      Resolution = x.Resolution,
    //                      CategoryConcernId = x.CategoryConcernId,
    //                      CategoryConcernName = x.CategoryConcernName,
    //                      ForClosingTicketTechnicians = x.ticketTechnicians.
    //                      Select(t => new GetOpenTicketResult.GetForClosingTicket.ForClosingTicketTechnician
    //                      {
    //                          TicketTechnicianId = t.Id,
    //                          Technician_By = t.TechnicianBy,
    //                          Fullname = t.TechnicianByUser.Fullname,
    //                      }).ToList(),

    //                      GetForClosingTicketCategories = x.TicketConcern.RequestConcern.TicketCategories
    //                                              .Select(t => new GetOpenTicketResult.GetForClosingTicket.GetForClosingTicketCategory
    //                                              {
    //                                                  TicketCategoryId = t.Id,
    //                                                  CategoryId = t.CategoryId,
    //                                                  Category_Description = t.Category.CategoryDescription,
    //                                              }).ToList(),

    //                      GetForClosingTicketSubCategories = x.TicketConcern.RequestConcern.TicketSubCategories
    //                                              .Select(t => new GetOpenTicketResult.GetForClosingTicket.GetForClosingTicketSubCategory
    //                                              {
    //                                                  TicketSubCategoryId = t.Id,
    //                                                  SubCategoryId = t.SubCategoryId,
    //                                                  SubCategory_Description = t.SubCategory.SubCategoryDescription,
    //                                              }).ToList(),
    //                      Notes = x.Notes,
    //                      IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true),
    //                      ApproverLists = x.ApproverTickets
    //                      .Where(x => x.IsApprove != null)
    //                      .Select(x => new ApproverList
    //                      {
    //                          ApproverName = x.User.Fullname,
    //                          Approver_Level = x.ApproverLevel.Value
    //                      }).ToList(),

    //                      GetAttachmentForClosingTickets = x.TicketAttachments.Select(x => new GetAttachmentForClosingTicket
    //                      {
    //                          TicketAttachmentId = x.Id,
    //                          Attachment = x.Attachment,
    //                          FileName = x.FileName,
    //                          FileSize = x.FileSize,
    //                      }).ToList(),
    //                  }).ToList(),

    //                    GetForOnHolds = x.TicketOnHolds
    //                    .Where(x => x.IsHold == false && x.IsActive)
    //                    .Select(h => new GetOpenTicketResult.GetForOnHold
    //                    {
    //                        Id = h.Id,
    //                        Reason = h.Reason,
    //                        AddedBy = h.AddedByUser.Fullname,
    //                        CreatedAt = h.CreatedAt,
    //                        IsHold = h.IsHold,
    //                        ResumeAt = h.ResumeAt,
    //                        IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true),
    //                        GetAttachmentForOnHoldOpenTickets = h.TicketAttachments
    //                        .Select(t => new GetOpenTicketResult.GetForOnHold.GetAttachmentForOnHoldOpenTicket
    //                        {
    //                            TicketAttachmentId = t.Id,
    //                            Attachment = t.Attachment,
    //                            FileName = t.FileName,
    //                            FileSize = t.FileSize,
    //                        }).ToList(),
    //                    }).ToList(),

    //                    GetOnHolds = x.TicketOnHolds
    //                    .Where(x => x.IsHold == true && x.IsActive)
    //                    .Select(h => new GetOpenTicketResult.GetOnHold
    //                    {
    //                        Id = h.Id,
    //                        Reason = h.Reason,
    //                        AddedBy = h.AddedByUser.Fullname,
    //                        CreatedAt = h.CreatedAt,
    //                        IsHold = h.IsHold,
    //                        ResumeAt = h.ResumeAt,
    //                        IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true),
    //                        GetAttachmentOnHoldOpenTickets = h.TicketAttachments
    //                        .Select(t => new GetOpenTicketResult.GetOnHold.GetAttachmentOnHoldOpenTicket
    //                        {
    //                            TicketAttachmentId = t.Id,
    //                            Attachment = t.Attachment,
    //                            FileName = t.FileName,
    //                            FileSize = t.FileSize,
    //                        }).ToList(),
    //                    }).ToList(),

    //                    GetForTransferTickets = x.TransferTicketConcerns
    //                    .Where(x => x.IsActive == true && x.IsTransfer == false && x.TransferBy == request.UserId)
    //                    .Select(x => new GetOpenTicketResult.GetForTransferTicket
    //                    {
    //                        TransferTicketConcernId = x.Id,
    //                        Transfer_Remarks = x.TransferRemarks,
    //                        Transfer_To = x.TransferTo,
    //                        Transfer_To_Name = x.TransferToUser.Fullname,
    //                        Current_Target_Date = x.Current_Target_Date,
    //                        IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true),
    //                        GetAttachmentForTransferTickets = x.TicketAttachments.Select(x => new GetOpenTicketResult.GetForTransferTicket.GetAttachmentForTransferTicket
    //                        {
    //                            TicketAttachmentId = x.Id,
    //                            Attachment = x.Attachment,
    //                            FileName = x.FileName,
    //                            FileSize = x.FileSize,
    //                        }).ToList(),
    //                    })
    //                    .ToList(),

    //                    Transaction_Date = x.ticketHistories.Max(x => x.TransactionDate).Value,

    //                    Aging_Days = x.RequestConcern.Is_Confirm == null ? null : EF.Functions.DateDiffDay(x.DateApprovedAt.Value.Date, x.Closed_At.Value.Date) <= 0 ? 0 : EF.Functions.DateDiffDay(x.DateApprovedAt.Value.Date, x.Closed_At.Value.Date),


    //                }).ToListAsync(cancellationToken);

    //            return new PagedList<GetOpenTicketResult>(results, totalCount, request.PageNumber, request.PageSize);
    //        }
    //    }
    //}

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
                    .AsNoTracking()
                    .Include(x => x.ClosingTickets)
                    .Include(x => x.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.AssignTo == request.UserId);




                if (!string.IsNullOrEmpty(request.Concern_Status))
                {
                    switch (request.Concern_Status)
                    {
                        case TicketingConString.PendingRequest:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsApprove == false && x.OnHold == null);
                            break;

                        case TicketingConString.ForApprovalTicket:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsApprove == false && x.ConcernStatus == TicketingConString.ForApprovalTicket && x.IsAssigned == true);
                            break;

                        case TicketingConString.Open:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsApprove == true && x.ConcernStatus == TicketingConString.OnGoing && x.IsClosedApprove == null && x.IsTransfer == null && x.OnHold == null || x.IsTransfer != false
                                && x.IsClosedApprove == null && x.OnHold == null && x.IsApprove == true);
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
                                .Where(x => x.RequestConcern.Is_Confirm == true);
                            break;

                        case TicketingConString.DateRejected:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.ConcernStatus == TicketingConString.DateRejected);
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
                                .Where(x => x.IsApprove == true || x.IsTransfer != false
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


                if (!string.IsNullOrEmpty(request.Search))
                {
                    ticketConcernQuery = ticketConcernQuery
                        .Where(x => x.User.Fullname.ToLower().Contains(request.Search.ToLower())
                            || x.Id.ToString().Contains(request.Search)
                            || x.RequestConcern.Concern.ToLower().Contains(request.Search.ToLower()));
                }

                if (request.Ascending == false)
                {
                    ticketConcernQuery = ticketConcernQuery.OrderByDescending(x => x.Id);
                }


                if (request.AscendingDate == false)
                {
                    ticketConcernQuery = ticketConcernQuery.OrderByDescending(x => x.TargetDate);
                }





                var totalCount = await ticketConcernQuery.CountAsync(cancellationToken);

                if (totalCount == 0)
                {
                    return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
                }


                var results = await ticketConcernQuery
                    .AsNoTracking()
                    .Select(x => new GetOpenTicketResult
                    {
                        Closed_Status = x.RequestConcern.Is_Confirm == null ? null :
                           x.IsClosedApprove == true ?
                           x.TargetDate.Value.Date >= x.Closed_At.Value.Date ?
                           TicketingConString.OnTime :
                          TicketingConString.Delay : null,
                        TicketConcernId = x.Id,
                        RequestConcernId = x.RequestConcernId,
                        Severity = x.RequestConcern.Severity,
                        CompanyId = x.RequestConcern.CompanyId,
                        //BusinessUnitId = x.RequestConcern.BusinessUnitId,
                        DepartmentId = x.RequestConcern.DepartmentId,
                        //UnitId = x.RequestConcern.UnitId,
                        //SubUnitid = x.RequestConcern.SubUnitId,
                        LocationId = x.RequestConcern.LocationId,
                        Concern_Description = x.RequestConcern.Concern,
                        //Company_Code = x.RequestConcern.OneChargingMIS.company_code,
                        //Company_Name = x.RequestConcern.OneChargingMIS.company_name,
                        //BusinessUnit_Code = x.RequestConcern.OneChargingMIS.business_unit_code,
                        //BusinessUnit_Name = x.RequestConcern.OneChargingMIS.business_unit_name,
                        Department_Code = x.RequestorByUser.OneChargingMIS.department_code,
                        Department_Name = x.RequestorByUser.OneChargingMIS.department_name,
                        //Unit_Code = x.RequestConcern.OneChargingMIS.department_unit_code,
                        //Unit_Name = x.RequestConcern.OneChargingMIS.department_unit_name,
                        //SubUnit_Code = x.RequestorByUser.OneChargingMIS.sub_unit_code,
                        //SubUnit_Name = x.User.OneChargingMIS.sub_unit_name,
                        Location_Code = x.RequestConcern.OneChargingMIS.location_code,
                        Location_Name = x.RequestConcern.OneChargingMIS.location_name,
                        Requestor_By = x.RequestorBy,
                        Requestor_Name = x.RequestorByUser.Fullname,
                        IsStore = x.RequestConcern.User.IsStore,
                        DateStarted = x.DateApprovedAt,
                        GetOpenTicketCategories = x.RequestConcern.TicketCategories
                                                .Select(t => new GetOpenTicketResult.GetOpenTicketCategory
                                                {
                                                    TicketCategoryId = t.Id,
                                                    CategoryId = t.CategoryId,
                                                    Category_Description = t.Category.CategoryDescription,
                                                }),

                        GetOpenTicketSubCategories = x.RequestConcern.TicketSubCategories
                                                .Select(t => new GetOpenTicketResult.GetOpenTicketSubCategory
                                                {
                                                    TicketSubCategoryId = t.Id,
                                                    SubCategoryId = t.SubCategoryId,
                                                    SubCategory_Description = t.SubCategory.SubCategoryDescription,
                                                }),

                        Date_Needed = x.RequestConcern.DateNeeded,
                        Notes = x.RequestConcern.Notes,
                        Contact_Number = x.RequestConcern.ContactNumber,
                        Request_Type = x.RequestConcern.RequestType,
                        BackJobId = x.RequestConcern.BackJobId,
                        Back_Job_Concern = x.RequestConcern.BackJob.Concern,
                        ChannelId = x.RequestConcern.ChannelId,
                        Channel_Name = x.RequestConcern.Channel.ChannelName,
                        ServiceProviderId = x.RequestConcern.ServiceProviderId,
                        ServiceProviderName = x.RequestConcern.ServiceProvider.ServiceProviderName,
                        TransferChannelId = x.RequestConcern.TransferChannelId,
                        Transfer_Channel_Name = x.RequestConcern.TransferChannel.ChannelName,
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
                        Closed_At = x.RequestConcern.Is_Confirm == null ? null : x.Closed_At,
                        Is_Transfer = x.IsTransfer,
                        Transfer_At = x.TransferAt,
                        GetForClosingTickets =  x.ClosingTickets
                       .Where(x => x.IsActive == true && x.IsRejectClosed == false)
                       .Where(x => x.IsClosing == true ? x.IsClosing == true : x.IsClosing == false)
                      .Select(x => new GetOpenTicketResult.GetForClosingTicket
                      {
                          ClosingTicketId = x.Id,
                          Remarks = x.RejectRemarks,
                          Resolution = x.Resolution,
                          CategoryConcernId = x.CategoryConcernId,
                          CategoryConcernName = x.CategoryConcernName,
                          ForClosingTicketTechnicians = x.ticketTechnicians.
                          Select(t => new GetOpenTicketResult.GetForClosingTicket.ForClosingTicketTechnician
                          {
                              TicketTechnicianId = t.Id,
                              Technician_By = t.TechnicianBy,
                              Fullname = t.TechnicianByUser.Fullname,
                          }),

                          //GetForClosingTicketCategories = x.TicketConcern.RequestConcern.TicketCategories
                          //                        .Select(t => new GetOpenTicketResult.GetForClosingTicket.GetForClosingTicketCategory
                          //                        {
                          //                            TicketCategoryId = t.Id,
                          //                            CategoryId = t.CategoryId,
                          //                            Category_Description = t.Category.CategoryDescription,
                          //                        }),

                          //GetForClosingTicketSubCategories = x.TicketConcern.RequestConcern.TicketSubCategories
                          //                        .Select(t => new GetOpenTicketResult.GetForClosingTicket.GetForClosingTicketSubCategory
                          //                        {
                          //                            TicketSubCategoryId = t.Id,
                          //                            SubCategoryId = t.SubCategoryId,
                          //                            SubCategory_Description = t.SubCategory.SubCategoryDescription,
                          //                        }),
                          Notes = x.Notes,
                          IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true),
                          ApproverLists = x.ApproverTickets
                          .Where(x => x.IsApprove != null)
                          .Select(x => new ApproverList
                          {
                              ApproverName = x.User.Fullname,
                              Approver_Level = x.ApproverLevel.Value
                          }),

                          GetAttachmentForClosingTickets = x.TicketAttachments.Select(x => new GetAttachmentForClosingTicket
                          {
                              TicketAttachmentId = x.Id,
                              Attachment = x.Attachment,
                              FileName = x.FileName,
                              FileSize = x.FileSize,
                          }),
                      }),

                        GetForOnHolds =  x.TicketOnHolds
                        .Where(x => x.IsHold == false && x.IsActive)
                        .Select(h => new GetOpenTicketResult.GetForOnHold
                        {
                            Id = h.Id,
                            Reason = h.Reason,
                            AddedBy = h.AddedByUser.Fullname,
                            CreatedAt = h.CreatedAt,
                            IsHold = h.IsHold,
                            ResumeAt = h.ResumeAt,
                            IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true),
                            GetAttachmentForOnHoldOpenTickets = h.TicketAttachments
                            .Select(t => new GetOpenTicketResult.GetForOnHold.GetAttachmentForOnHoldOpenTicket
                            {
                                TicketAttachmentId = t.Id,
                                Attachment = t.Attachment,
                                FileName = t.FileName,
                                FileSize = t.FileSize,
                            }),
                        }),

                        GetOnHolds =  x.TicketOnHolds
                        .Where(x => x.IsHold == true && x.IsActive)
                        .Select(h => new GetOpenTicketResult.GetOnHold
                        {
                            Id = h.Id,
                            Reason = h.Reason,
                            AddedBy = h.AddedByUser.Fullname,
                            CreatedAt = h.CreatedAt,
                            IsHold = h.IsHold,
                            ResumeAt = h.ResumeAt,
                            IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true),
                            GetAttachmentOnHoldOpenTickets = h.TicketAttachments
                            .Select(t => new GetOpenTicketResult.GetOnHold.GetAttachmentOnHoldOpenTicket
                            {
                                TicketAttachmentId = t.Id,
                                Attachment = t.Attachment,
                                FileName = t.FileName,
                                FileSize = t.FileSize,
                            }),
                        }),

                        GetForTransferTickets =  x.TransferTicketConcerns
                        .Where(x => x.IsActive == true && x.IsTransfer == false && x.TransferBy == request.UserId)
                        .Select(x => new GetOpenTicketResult.GetForTransferTicket
                        {
                            TransferTicketConcernId = x.Id,
                            Transfer_Remarks = x.TransferRemarks,
                            Transfer_To = x.TransferTo,
                            Transfer_To_Name = x.TransferToUser.Fullname,
                            Current_Target_Date = x.Current_Target_Date,
                            IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true),
                            GetAttachmentForTransferTickets = x.TicketAttachments.Select(x => new GetOpenTicketResult.GetForTransferTicket.GetAttachmentForTransferTicket
                            {
                                TicketAttachmentId = x.Id,
                                Attachment = x.Attachment,
                                FileName = x.FileName,
                                FileSize = x.FileSize,
                            }),
                        }),

                        Transaction_Date = x.ticketHistories.Max(x => x.TransactionDate).Value,

                        Aging_Days = x.RequestConcern.Is_Confirm == null ? null : EF.Functions.DateDiffDay(x.DateApprovedAt.Value.Date, x.Closed_At.Value.Date) <= 0 ? 0 : EF.Functions.DateDiffDay(x.DateApprovedAt.Value.Date, x.Closed_At.Value.Date),


                    }).ToListAsync(cancellationToken);

                return new PagedList<GetOpenTicketResult>(results, totalCount, request.PageNumber, request.PageSize);
            }
        }
    }
}

