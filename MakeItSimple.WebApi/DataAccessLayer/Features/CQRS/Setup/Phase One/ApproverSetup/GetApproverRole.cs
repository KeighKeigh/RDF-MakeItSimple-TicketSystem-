﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.ApproverSetup
{
    public class GetApproverRole
    {
        public class GetApproverRoleResult
        {
            public Guid? UserId { get; set; }
            public string EmpId { get; set; }

            public string FullName { get; set; }

            public string UserRole { get; set; }
        }


        public class GetApproverRoleQuery : IRequest<Result>
        {

            //public string Role {  get; set; }
            public int? SubUnitId { get; set; }
        }

        public class Handler : IRequestHandler<GetApproverRoleQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetApproverRoleQuery request, CancellationToken cancellationToken)
            {

                var approver = await _context.Approvers
                    .Where(x => x.SubUnitId == request.SubUnitId)
                    .Select(x => x.UserId)
                    .ToListAsync();

                var allUserRole = await _context.UserRoles
                    .ToListAsync();

                var roleList = allUserRole
                    .Where(x => x.Permissions.Contains(TicketingConString.Approver))
                    .Select(x => x.Id)
                    .ToList();

                var results = await _context.Users
                    .Include(x => x.UserRole)
                    .Where(x => !approver.Contains(x.Id) && roleList.Contains(x.UserRoleId)
                   && x.IsActive == true)
                    .Select(x => new GetApproverRoleResult
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