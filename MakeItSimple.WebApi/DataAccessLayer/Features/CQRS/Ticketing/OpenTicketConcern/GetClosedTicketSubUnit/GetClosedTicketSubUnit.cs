using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.OpenTicketConcern.GetOpenTicketSubUnit.GetOpenTicketSubUnit;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.OpenTicketConcern.GetClosedTicketSubUnit
{
    public partial class GetClosedTicketSubUnit
    {
        public class Handler : IRequestHandler<GetClosedTicketSubUnitQuery, PagedList<GetClosedTicketSubUnitResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetClosedTicketSubUnitResult>> Handle(GetClosedTicketSubUnitQuery request, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                IQueryable<TicketConcern> delayedTicketsQuery = _context.TicketConcerns
                .Where(x => x.RequestConcern.IsDone == true &&
                x.TargetDate.HasValue &&
                ((x.Closed_At.HasValue && x.Closed_At.Value > x.TargetDate.Value) ||
                 (!x.Closed_At.HasValue && DateTime.Now > x.TargetDate.Value)))
                 .AsNoTrackingWithIdentityResolution()
                .AsSplitQuery();

                if (delayedTicketsQuery.Any())
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
                        delayedTicketsQuery = delayedTicketsQuery
                            .Where(x => x.User.Fullname.Contains(request.Search)
                                        || x.ToString().Contains(request.Search));
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {
                        var filterApproval = delayedTicketsQuery.Select(x => x.Id);

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
                                    .Where(x => x.IsApprove == true)
                                    .Select(x => new
                                    {
                                        x.ApproverLevel,
                                        x.IsApprove,
                                        x.ApproverDateId,
                                        x.UserId,
                                        x.TicketConcernId

                                    }).ToListAsync();

                                var userRequestIdApprovalList = approverTransactList.Select(x => x.TicketConcernId);
                                var userIdsInApprovalList = approverTransactList.Select(approval => approval.UserId);

                                delayedTicketsQuery = delayedTicketsQuery
                                    .Where(x => userIdsInApprovalList.Contains(x.ApprovedDateBy)
                                    && userRequestIdApprovalList.Contains(x.Id));

                            }

                        }
                        else if (request.UserType == TicketingConString.Users)
                        {
                            delayedTicketsQuery = delayedTicketsQuery.Where(x => x.AddedByUser.Id == request.UserId);
                        }
                        else
                        {
                            return new PagedList<GetClosedTicketSubUnitResult>(new List<GetClosedTicketSubUnitResult>(), 0, request.PageNumber, request.PageSize);
                        }
                    }
                }

                var results = delayedTicketsQuery
                    .OrderByDescending(x => x.TargetDate)
                    .Select(x => new GetClosedTicketSubUnitResult
                    {

                        TicketConcernId = x.Id,
                        Concern_Details = x.RequestConcern.Concern,
                        Notes = x.RequestConcern.Notes,
                        DepartmentId = x.User.DepartmentId,
                        Department_Name = x.User.OneChargingMIS.department_name,
                        ChannelId = x.RequestConcern.ChannelId,
                        Channel_Name = x.RequestConcern.Channel.ChannelName,
                        UserId = x.UserId,
                        Fullname = x.User.Fullname,

                        GetCloseTicketSubUnitCategories = x.RequestConcern.TicketCategories
                        .Select(t => new GetClosedTicketSubUnitResult.GetCloseTicketSubUnitCategory
                        {
                            TicketCategoryId = t.Id,
                            CategoryId = t.CategoryId,
                            Category_Description = t.Category.CategoryDescription,

                        }).ToList(),

                        GetCloseTicketSubUnitSubCategories = x.RequestConcern.TicketSubCategories
                        .Select(t => new GetClosedTicketSubUnitResult.GetCloseTicketSubUnitSubCategory
                        {
                            TicketSubCategoryId = t.Id,
                            SubCategoryId = t.SubCategoryId,
                            SubCategory_Description = t.SubCategory.SubCategoryDescription,

                        }).ToList(),

                        SubCategoryDescription = x.RequestConcern.SubCategory.SubCategoryDescription,
                        Target_Date = x.TargetDate,
                        Added_By = x.AddedByUser.Fullname,
                        Created_At = x.CreatedAt,
                        Updated_At = x.UpdatedAt,
                        Modified_By = x.ModifiedByUser.Fullname,

                        CloseTicketSubCategoryAttachments = x.TicketAttachments.Select(x => new GetClosedTicketSubUnitResult.CloseTicketSubCategoryAttachment
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

                    });

                return await PagedList<GetClosedTicketSubUnitResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
