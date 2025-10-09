using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.GetClosing.GetClosingTicket;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.NewFolder
{
    public partial class DateApproval
    {

        public class Handler : IRequestHandler<DateApprovalQuery, PagedList<DateApprovalResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<DateApprovalResult>> Handle(DateApprovalQuery request, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;
                var businessUnitList = new List<BusinessUnit>();

                IQueryable<ApproverDate> approverDatesQuery = _context.ApproverDates
                    .AsNoTrackingWithIdentityResolution()
                    .AsSplitQuery();


                if (approverDatesQuery.Any())
                {
                    var allUserList = await _context.UserRoles
                        .AsNoTracking()
                        .Select(x => new
                        {
                            x.Id,
                            x.UserRoleName,
                            x.Permissions

                        }).ToListAsync();

                    var receiverPermissionList = allUserList
                        .Where(x => x.Permissions
                        .Contains(TicketingConString.Receiver))
                        .Select(x => x.UserRoleName)
                        .ToList();

                    var approverPermissionList = allUserList
                        .Where(x => x.Permissions
                        .Contains(TicketingConString.Approver))
                        .Select(x => x.UserRoleName)
                        .ToList();

                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        approverDatesQuery = approverDatesQuery
                            .Where(x => x.TicketConcern.User.Fullname.Contains(request.Search)
                                        || x.TicketConcernId.ToString().Contains(request.Search));
                    }

                    if (request.IsReject != null)
                    {
                        approverDatesQuery = approverDatesQuery.Where(x => x.IsRejectDate == request.IsReject);
                    }

                    if (request.IsDateApproved != null)
                    {
                        approverDatesQuery = approverDatesQuery.Where(x => x.IsApproved == request.IsDateApproved);
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {
                        var filterApproval = approverDatesQuery.Select(x => x.Id);

                        if (request.UserType == TicketingConString.Approver)
                        {

                            if (approverPermissionList.Any(x => x.Contains(request.Role)))
                            {

                                var userApprover = await _context.Users
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                                var approverTransactList = await _context.ApproverTicketings
                                    .AsNoTracking()
                                    .Where(x => x.UserId == userApprover.Id)
                                    .Where(x => x.IsApprove == null)
                                    .Select(x => new
                                    {
                                        x.ApproverLevel,
                                        x.IsApprove,
                                        x.ApproverDateId,
                                        x.UserId,

                                    }).ToListAsync();

                                var userRequestIdApprovalList = approverTransactList.Select(x => x.ApproverDateId);
                                var userIdsInApprovalList = approverTransactList.Select(approval => approval.UserId);

                                approverDatesQuery = approverDatesQuery
                                    .Where(x => userIdsInApprovalList.Contains(x.TicketApprover)
                                    && userRequestIdApprovalList.Contains(x.Id));

                            }

                        }
                        else if (request.UserType == TicketingConString.Users)
                        {
                            approverDatesQuery = approverDatesQuery.Where(x => x.AddedByUser.Id == request.UserId);
                        }
                        else
                        {
                            return new PagedList<DateApprovalResult>(new List<DateApprovalResult>(), 0, request.PageNumber, request.PageSize);
                        }
                    }
                }

                var results = approverDatesQuery
                   .OrderByDescending(x => x.CreatedAt)
                   .Select(x => new DateApprovalResult
                   {
                       ApproverDatesId = x.Id,
                       TicketConcernId = x.TicketConcernId,
                       Concern_Details = x.TicketConcern.RequestConcern.Concern,
                       ChannelId = x.TicketConcern.RequestConcern.ChannelId,
                       Channel_Name = x.TicketConcern.RequestConcern.Channel.ChannelName,
                       UserId = x.TicketConcern.UserId,
                       Fullname = x.TicketConcern.User.Fullname,
                       ConcernStatus = x.TicketConcern.ConcernStatus,
                       IsActive = x.TicketConcern.IsActive,
                       Reason = x.TicketConcern.Reason,
                       

                       GetApproveDateTicketCategories = x.TicketConcern.RequestConcern.TicketCategories
                       .Select(t => new DateApprovalResult.GetApproveDateTicketCategory
                       {
                           TicketCategoryId = t.Id,
                           CategoryId = t.CategoryId,
                           Category_Description = t.Category.CategoryDescription,

                       }).ToList(),

                       GetApproveDateTicketSubCategories = x.TicketConcern.RequestConcern.TicketSubCategories
                       .Select(t => new DateApprovalResult.GetApproveDateTicketSubCategory
                       {
                           TicketSubCategoryId = t.Id,
                           SubCategoryId = t.SubCategoryId,
                           SubCategory_Description = t.SubCategory.SubCategoryDescription,
                       }).ToList(),

                       Reject_Remarks = x.RejectRemarks,
                       Target_Date = x.TicketConcern.TargetDate,
                       Added_By = x.AddedByUser.Fullname,
                       Created_At = x.CreatedAt,
                       ApprovedAt = x.ApprovedDateAt,
                       DateNeeded = x.TicketConcern.RequestConcern.DateNeeded,

                       DateApprovalAttachments = x.TicketAttachments.Select(x => new DateApprovalResult.DateApprovalAttachment
                       {

                           TicketAttachmentId = x.Id,
                           Attachment = x.Attachment,
                           FileName = x.FileName,
                           FileSize = x.FileSize,
                           Added_By = x.AddedByUser.Fullname,
                           Created_At = x.CreatedAt,
                           Modified_By = x.ModifiedByUser.Fullname,
                           Updated_At = x.UpdatedAt,

                       }).ToList()



                   }).Where(x => x.Target_Date != null && x.ConcernStatus == TicketingConString.ForApprovalTicket);

                return await PagedList<DateApprovalResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
