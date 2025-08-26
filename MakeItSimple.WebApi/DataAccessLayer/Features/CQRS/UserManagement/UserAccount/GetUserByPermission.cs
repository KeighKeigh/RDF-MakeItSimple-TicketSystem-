using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.OpenTicketConcern.GetOpenTicketSubUnit.GetOpenTicketSubUnit;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount
{
    public class GetUserByPermission
    {
        public class GetUserByPermissionResult
        {
            public Guid? UserId { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string EmpId { get; set; }
        }

        public class GetUserByPermissionQuery : UserParams, IRequest<PagedList<GetUserByPermissionResult>>
        {
            public Guid? ApproverId { get; set; }
            public bool? Status { get; set; }
            public string Search { get; set; }
            public string UserType { get; set; }
        }

        public class Handler : IRequestHandler<GetUserByPermissionQuery, PagedList<GetUserByPermissionResult>>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetUserByPermissionResult>> Handle(GetUserByPermissionQuery request, CancellationToken cancellationToken)
            {
                IQueryable<User> userQuery = _context.Users
                    .AsNoTrackingWithIdentityResolution().Where(x => x.IsStore == false)
                    .AsQueryable();


                var allUserList = await _context.UserRoles
                        .AsNoTracking()
                        .Select(x => new
                        {
                            x.Id,
                            x.UserRoleName,
                            x.Permissions
                        }).ToListAsync();


                var approverPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Approver))
                    .Select(x => x.Id)
                    .ToList();

                var requestorPermissionList = allUserList
                    .Where(x => x.Permissions.Contains(TicketingConString.Ticketing))
                    .Select(x => x.Id)
                    .ToList();
                    


                var userIds = await userQuery.Select(x => x.Id).ToListAsync();
                var userHasApprover = await _context.ApproverUsers.Where(x => userIds.Contains(x.UserId.Value)).Select(x => x.UserId).ToListAsync();
                var ApproverHandles = await _context.ApproverUsers.Where(x => userIds.Contains(x.ApproverId.Value)).Select(x => x.ApproverId).ToListAsync();



                if (!string.IsNullOrEmpty(request.UserType))
                {

                    if (ApproverHandles != null)
                    {
                        userQuery = userQuery.Where(x => !ApproverHandles.Contains(x.Id));
                    }


                    if (request.UserType == TicketingConString.Approver)
                    {
                        userQuery = userQuery.Where(x => approverPermissionList.Contains(x.UserRoleId));
                    }

                }
                else
                {
                    userQuery = userQuery.Where(x => requestorPermissionList.Contains(x.UserRoleId));


                    if (userHasApprover != null)
                    {
                        userQuery = userQuery.Where(x => !userHasApprover.Contains(x.Id));
                    }

                    
                }


                if (request.Status != null)
                {
                    userQuery = userQuery.Where(x => x.IsActive == request.Status);
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    userQuery = userQuery.Where(x => x.Fullname.ToLower().Contains(request.Search)
                    || x.Username.ToLower().Contains(request.Search));
                }

                var users = userQuery.Select(x => new GetUserByPermissionResult
                {
                    UserId = x.Id,
                    UserName = x.Username,
                    FullName = x.Fullname,
                    EmpId = x.EmpId,
                });

                return await PagedList<GetUserByPermissionResult>.CreateAsync(users, request.PageNumber, request.PageSize);



            }
        }
    }
}
