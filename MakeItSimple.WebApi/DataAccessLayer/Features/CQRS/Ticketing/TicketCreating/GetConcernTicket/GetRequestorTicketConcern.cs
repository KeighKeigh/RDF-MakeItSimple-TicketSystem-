﻿using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetConcernTicket.GetRequestorTicketConcern.GetRequestorTicketConcernResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetConcernTicket
{
    public partial class GetRequestorTicketConcern
    {

        public class Handler : IRequestHandler<GetRequestorTicketConcernQuery, PagedList<GetRequestorTicketConcernResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetRequestorTicketConcernResult>> Handle(GetRequestorTicketConcernQuery request, CancellationToken cancellationToken)
            {

                var dateToday = DateTime.Now;

                IQueryable<RequestConcern> requestConcernsQuery = _context.RequestConcerns
                     .AsNoTrackingWithIdentityResolution()
                     .OrderBy(x => x.Id)
                     .AsSplitQuery();
                
                if (requestConcernsQuery.Any())
                {

                    var allUserList = await _context.UserRoles
                        .AsNoTrackingWithIdentityResolution()
                        .Select(x => new
                        {
                            x.Id,
                            x.UserRoleName,
                            x.Permissions

                        }).ToListAsync();

                    var requestorPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Requestor))
                    .Select(x => x.UserRoleName)
                    .ToList();

                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        requestConcernsQuery = requestConcernsQuery
                            .Where(x => x.User.Fullname
                            .Contains(request.Search)
                            || x.Id.ToString().Contains(request.Search)
                            || x.Concern.ToLower().Contains(request.Search.ToLower()));
                    }

                    if (request.Status != null)
                    {
                        requestConcernsQuery = requestConcernsQuery
                            .Where(x => x.IsActive == request.Status);
                    }

                    if (request.Is_Approve != null)
                    {
                        var ticketStatusList = await _context.TicketConcerns
                            .AsNoTracking()
                            .Where(x => x.IsApprove == request.Is_Approve)
                            .Select(x => x.RequestConcernId)
                            .ToListAsync();

                        requestConcernsQuery = requestConcernsQuery
                           .Where(x => ticketStatusList.Contains(x.Id));
                    }

                    if (request.Ascending is not null)
                    {
                        
                        requestConcernsQuery = request.Ascending.Value is true
                            ? requestConcernsQuery
                            .OrderBy(x => x.Id)
                            : requestConcernsQuery
                            .OrderByDescending(x => x.Id);
                    }

                    if (request.Concern_Status is not null)
                    {

                        switch (request.Concern_Status)
                        {
                            case TicketingConString.Approval:
                                
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.ForApprovalTicket);
                                break;
                            case TicketingConString.OnGoing:
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.CurrentlyFixing);
                                break;

                            case TicketingConString.NotConfirm:
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.Is_Confirm == null && x.ConcernStatus == TicketingConString.NotConfirm);
                                break;

                            case TicketingConString.Done:
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.ConcernStatus == TicketingConString.Done && x.Is_Confirm == true);
                                break;
                            default:
                                return new PagedList<GetRequestorTicketConcernResult>(new List<GetRequestorTicketConcernResult>(), 0, request.PageNumber, request.PageSize);

                        }
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {
                        if (request.UserType == TicketingConString.Requestor)
                        {
                            if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                            {
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.UserId == request.UserId);
                            }
                            else
                            {
                                return new PagedList<GetRequestorTicketConcernResult>(new List<GetRequestorTicketConcernResult>(), 0, request.PageNumber, request.PageSize);
                            }
                        }
                        if(request.UserType == TicketingConString.Receiver)
                        {
                            var userChannelId = await _context.ChannelUsers
                                 .Where(cu => cu.UserId == request.UserId)
                                 .Select(cu => cu.ChannelId)
                                 .ToListAsync();


                            if (userChannelId.Any())
                            {
                                var serviceProviderIds = await _context.ServiceProviderChannels
                                    .Where(spc => userChannelId.Contains(spc.ChannelId.Value))
                                    .Select(spc => spc.ServiceProviderId)
                                    .Distinct()
                                    .ToListAsync();

                                var allRelatedChannelIds = await _context.ServiceProviderChannels
                                    .Where(spc => serviceProviderIds.Contains(spc.ServiceProviderId))
                                    .Select(spc => spc.ChannelId)
                                    .ToListAsync();

                                requestConcernsQuery = requestConcernsQuery
                                    .Where(rc =>
                                        (rc.ChannelId.HasValue && userChannelId.Contains(rc.ChannelId.Value)) && rc.AssignTo == null
                                        || (!rc.ChannelId.HasValue && rc.ServiceProviderId != null && serviceProviderIds.Contains(rc.ServiceProviderId.Value) && rc.AssignTo == null)
                                    );
                            }
                            else
                            {
                                requestConcernsQuery = requestConcernsQuery.Where(x => false);
                            }
                        }
                    }
                    

                }

                var results = requestConcernsQuery
                    .Select(g => new GetRequestorTicketConcernResult
                    {

                        RequestConcernId = g.Id,
                        Concern = g.Concern,
                        Resolution = g.Resolution,
                        CompanyId = g.CompanyId,
                        Company_Code = g.Company.CompanyCode,
                        Company_Name = g.Company.CompanyName,
                        BusinessUnitId = g.BusinessUnitId,
                        BusinessUnit_Code = g.BusinessUnit.BusinessCode,
                        BusinessUnit_Name = g.BusinessUnit.BusinessName,
                        DepartmentId = g.DepartmentId,
                        Department_Code = g.Department.DepartmentCode,
                        Department_Name = g.Department.DepartmentName,
                        UnitId = g.ReqUnitId,
                        Unit_Code = g.ReqUnit.UnitCode,
                        Unit_Name = g.ReqUnit.UnitName,
                        SubUnitId = g.ReqSubUnitId,
                        SubUnit_Code = g.ReqSubUnit.SubUnitCode,
                        SubUnit_Name = g.ReqSubUnit.SubUnitName,
                        LocationId = g.LocationId,
                        Location_Code = g.Location.LocationCode,
                        Location_Name = g.Location.LocationName,
                        RequestorId = g.UserId,
                        FullName = g.User.Fullname,
                        ChannelId = g.ChannelId,
                        Channel_Name = g.Channel.ChannelName,

                        TargetDate = g.TargetDate,
                        AssignTo = g.AssignTo,
                        AssignToName = _context.Users.Where(u => u.Id == g.AssignTo).Select(u => u.Fullname).FirstOrDefault(),
                        ServiceProviderId = g.ServiceProviderId,
                        ServiceProviderName = g.ServiceProvider.ServiceProviderName,
                        GetRequestTicketCategories = g.TicketCategories
                        .Select(t => new GetRequestorTicketConcernResult.GetRequestTicketCategory
                        {
                            TicketCategoryId = t.Id,
                            CategoryId = t.CategoryId,
                            Category_Description = t.Category.CategoryDescription,

                        }).ToList(),

                        GetRequestSubTicketCategories = g.TicketSubCategories
                        .Select(t => new GetRequestorTicketConcernResult.GetRequestSubTicketCategory
                        {
                            TicketSubCategoryId = t.Id,
                            Category_Id = t.SubCategory.CategoryId,
                            SubCategoryId = t.SubCategoryId,
                            SubCategory_Description = t.SubCategory.SubCategoryDescription,
                        }).ToList(),

                        Concern_Status = g.ConcernStatus,
                        Severity = g.Severity,
                        Is_Done = g.IsDone,
                        Remarks = g.Remarks,
                        Notes = g.Notes,
                        Contact_Number = g.ContactNumber,
                        Request_Type = g.RequestType,
                        BackJobId = g.BackJobId,
                        Back_Job_Concern = g.BackJob.Concern,
                        Added_By = g.AddedByUser.Fullname,
                        Date_Needed = g.DateNeeded,
                        Created_At = g.CreatedAt,
                        Modified_By = g.ModifiedByUser.Fullname,
                        updated_At = g.UpdatedAt,
                        Is_Confirmed = g.Is_Confirm,
                        Confirmed_At = g.Confirm_At,
                        TicketRequestConcerns = g.TicketConcerns
                            .Select(tc => new TicketRequestConcern
                            {
                                TicketConcernId = /*g.ConcernStatus == "For Approval" ? null :*/ tc.Id,
                                UserId = tc.UserId,
                                Issue_Handler = tc.User.Fullname,
                                Target_Date = tc.TargetDate,
                                Remarks = tc.Remarks,
                                Is_Assigned = tc.IsAssigned,
                                Added_By = tc.AddedByUser.Fullname,
                                Created_At = tc.CreatedAt,
                                Modified_By = tc.ModifiedByUser.Fullname,
                                Updated_At = tc.UpdatedAt,
                                Is_Active = tc.IsActive,
                                OnHold = tc.OnHold,
                                OnHold_At = tc.OnHoldAt,
                                OnHold_Reasons = tc.OnHoldAt != null ? tc.TicketOnHolds
                                .OrderByDescending(x => x.CreatedAt)
                                .First().Reason : null,
                                Resume_At = tc.Resume_At,
                                Closed_At = tc.Closed_At,
                                Closing_Notes = tc.IsClosedApprove == true ?
                                tc.ClosingTickets.Where(x => x.IsClosing == true)
                                .OrderByDescending(x => x.ClosingAt)
                                .First().Notes : null,
                                Is_Transfer = tc.IsTransfer,
                                Transfer_At = tc.TransferAt,
                                Transfer_By = tc.TransferByUser.Fullname,

                            }).ToList()

                    }) ;

                return await PagedList<GetRequestorTicketConcernResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}