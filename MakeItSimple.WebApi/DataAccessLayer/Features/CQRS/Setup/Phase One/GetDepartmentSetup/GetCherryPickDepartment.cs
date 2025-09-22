using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.GetLocationSetup.GetCherryPickLocation;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.GetDepartmentSetup
{
    public class GetCherryPickDepartment
    {

        public class GetCherryPickDepartmentResult
        {


                public int? Id { get; set; }
                public string DepartmentName { get; set; }
                public string DepartmentCode { get; set; }
            

        }

        public class GetCherryPickDepartmentQuery : UserParams, IRequest<PagedList<GetCherryPickDepartmentResult>>
        {
            public string UserType { get; set; }
            public string Role { get; set; }
            public Guid? UserId { get; set; }
            public string Search { get; set; }
        }


        public class Handler : IRequestHandler<GetCherryPickDepartmentQuery, PagedList<GetCherryPickDepartmentResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetCherryPickDepartmentResult>> Handle (GetCherryPickDepartmentQuery request, CancellationToken cancellationToken)
            {

                var dateToday = DateTime.Now;

                IQueryable<TicketConcern> requestConcernsQuery = _context.TicketConcerns
                     .AsNoTrackingWithIdentityResolution()
                     .Include(x => x.RequestConcern)
                     .ThenInclude(x => x.OneChargingMIS)
                     .OrderBy(x => x.Id)
                     .Where(x => x.IsActive == true && x.IsApprove == false)
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
                            .Where(x => x.RequestConcern.User.Fullname
                            .Contains(request.Search)
                            || x.Id.ToString().Contains(request.Search)
                            || x.RequestConcern.Concern.ToLower().Contains(request.Search.ToLower()));
                    }



                    if (!string.IsNullOrEmpty(request.UserType))
                    {
                        if (request.UserType == TicketingConString.Requestor)
                        {
                            if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                            {
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.RequestorBy == request.UserId);
                            }
                            else
                            {
                                return new PagedList<GetCherryPickDepartmentResult>(new List<GetCherryPickDepartmentResult>(), 0, request.PageNumber, request.PageSize);
                            }
                        }
                        if (request.UserType == TicketingConString.Receiver)
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



                                requestConcernsQuery = requestConcernsQuery
                                    .Where(rc =>
                                        (rc.RequestConcern.ChannelId.HasValue && userChannelId.Contains(rc.RequestConcern.ChannelId.Value)) && rc.AssignTo == null
                                        || (!rc.RequestConcern.ChannelId.HasValue && rc.RequestConcern.ServiceProviderId != null && serviceProviderIds.Contains(rc.RequestConcern.ServiceProviderId.Value) && rc.AssignTo == null)
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
                    .Select(g => new GetCherryPickDepartmentResult
                    {
                        Id = g.RequestConcern.DepartmentId,
                        DepartmentCode = g.RequestConcern.OneChargingMIS.department_code,
                        DepartmentName = g.RequestConcern.OneChargingMIS.department_name,

                    }).Distinct();

                return await PagedList<GetCherryPickDepartmentResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
