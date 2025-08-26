using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.Models.Setup.Phase_One.ApproverUsersSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Threading;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ApproverUserSetup
{
    public class AddApproverUser
    {


        public class AddApproverUserCommand : IRequest<Result>
        {
            public Guid? ApproverUserId { get; set; }
            public Guid? ApproverId { get; set; }
            public IEnumerable<Guid?> UserId { get; set; } = new List<Guid?>();
        }

        public class Handler : IRequestHandler<AddApproverUserCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(AddApproverUserCommand command, CancellationToken cancellation)
            {
                try
                {

                    if (command.ApproverId != command.ApproverUserId && command.ApproverUserId != null)
                    {
                        var recordsToUpdate = await _context.ApproverUsers.Include(x => x.Approver)
                            .Where(x => command.ApproverUserId == x.ApproverId)
                            .ToListAsync(cancellation);

                        foreach (var record in recordsToUpdate)
                        {
                            record.ApproverId = command.ApproverId;
                            record.UpdatedAt = DateTime.UtcNow;
                        }

                        await _context.SaveChangesAsync(cancellation);
                        return Result.Success();
                    }

                    var existingApproverUsers = await _context.ApproverUsers
                        .Where(x => x.ApproverId == command.ApproverId)
                        .ToListAsync(cancellation);

                    var existingUserIds = existingApproverUsers.Select(x => x.UserId).ToHashSet();
                    var newUserIds = command.UserId.ToHashSet();

                    var usersToAdd = newUserIds.Except(existingUserIds);
                    var usersToRemove = existingUserIds.Except(newUserIds);
                    var usersToUpdate = existingUserIds.Intersect(newUserIds);


                    foreach (var userId in usersToAdd)
                    {
                        var approverUser = new ApproverUser
                        {
                            ApproverId = command.ApproverId,
                            UserId = userId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                        };
                        await _context.ApproverUsers.AddAsync(approverUser);
                    }

                    var relationshipsToRemove = existingApproverUsers
                        .Where(x => usersToRemove.Contains(x.UserId));
                    _context.ApproverUsers.RemoveRange(relationshipsToRemove);

                    foreach (var userId in usersToUpdate)
                    {
                        var existingRelationship = existingApproverUsers
                            .First(x => x.UserId == userId);
                        existingRelationship.UpdatedAt = DateTime.UtcNow;
                    }

                    await _context.SaveChangesAsync(cancellation);
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    return Result.Failure(ApproverError.AddingApproverUserError());
                }
            }
        }
    }
}
