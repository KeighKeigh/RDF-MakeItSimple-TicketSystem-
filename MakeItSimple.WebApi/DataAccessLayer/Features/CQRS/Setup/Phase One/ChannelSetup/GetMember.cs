using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.GetDepartmentSetup.GetCherryPickDepartment;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.ChannelSetup
{
    public class GetMember
    {
        public class GetMemberResult
        {
            public Guid? UserId { get; set; }
            public string EmpId { get; set; }
            public string FullName { get; set; }
            public string UserRole { get; set; }

        }

        public class GetMemberQuery : IRequest<Result>
        {
            public int? ChannelId { get; set; }

            public List<int> DepartmentId { get; set; }


        }

        public class Handler : IRequestHandler<GetMemberQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetMemberQuery request, CancellationToken cancellationToken)
            {
                var channelUsers = await _context.ChannelUsers
                    .Where(x => x.ChannelId == request.ChannelId)
                    .ToListAsync();

                var selectedChannelUsers = channelUsers.Select(x => x.UserId);

                var departmentList = await _context.OneChargings.ToListAsync();

                var result = departmentList
                    .Select(g => new GetCherryPickDepartmentResult
                    {
                        Id = g.department_id,
                        DepartmentCode = g.department_code,
                        DepartmentName = g.department_name,

                    }).Distinct();

                if (request.DepartmentId.Count() > 0 && result != null)
                {
                    result = result.Where(x => request.DepartmentId.Contains(x.Id.Value)).ToList();
                }
                var departmentSelect = result.Select(x => x.Id).ToList();

                var results = await _context.Users
                    .Where(x => !selectedChannelUsers.Contains(x.Id) && x.IsActive == true)
                    .Where(x => departmentSelect.Contains(x.DepartmentId.Value))
                    .Select(x => new GetMemberResult
                    {

                        UserId = x.Id,
                        EmpId = x.EmpId,
                        FullName = x.Fullname,
                        UserRole = x.UserRole.UserRoleName

                    }).ToListAsync();

                return Result.Success(results);
            }
        }
    }
}