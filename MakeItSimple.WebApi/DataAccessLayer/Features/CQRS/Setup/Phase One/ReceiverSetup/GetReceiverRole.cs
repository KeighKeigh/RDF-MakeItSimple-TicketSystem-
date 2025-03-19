﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.ReceiverSetup
{
    public class GetReceiverRole
    {
        public class GetReceiverRoleResult
        {
            public Guid? UserId { get; set; }

            public string EmpId { get; set; }

            public string FullName { get; set; }

        }

        public class GetReceiverRoleQuery : IRequest<Result> { }

        public class Handler : IRequestHandler<GetReceiverRoleQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetReceiverRoleQuery request, CancellationToken cancellationToken)
            {

                var receiver = await _context.Receivers.ToListAsync();

                var selectReceiver = receiver.Select(x => x.UserId);

                var receiverString = TicketingConString.Receiver;

                var allUserRole = await _context.UserRoles.ToListAsync();

                var roleList = allUserRole
                    .Where(x => x.Permissions.Contains(receiverString))
                     .ToList();

                var roleSelect = roleList.Select(x => x.Id).ToList();


                var users = await _context.Users.Include(x => x.UserRole)
                    .Where(x => roleSelect.Contains(x.UserRoleId))
                    .Where(x => !selectReceiver.Contains(x.Id) && x.IsActive == true)
                    .Select(x => new GetReceiverRoleResult
                    {
                        UserId = x.Id,
                        EmpId = x.EmpId,
                        FullName = x.Fullname

                    }).ToListAsync();


                return Result.Success(users);

            }
        }
    }
}
