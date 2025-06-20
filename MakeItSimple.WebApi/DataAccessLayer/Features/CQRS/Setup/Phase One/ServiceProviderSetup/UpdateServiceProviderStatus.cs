using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ServiceProviderSetup
{
    public class UpdateServiceProviderStatus
    {
        public class UpdateServiceProviderStatusResult
        {
            public int Id { get; set; }

            public bool Status { get; set; }

        }


        public class UpdateServiceProviderStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }


        }

        public class Handler : IRequestHandler<UpdateServiceProviderStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateServiceProviderStatusCommand command, CancellationToken cancellationToken)
            {
                var service = await _context.ServiceProviders.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (service == null)
                {
                    return Result.Failure(ServiceError.ServiceProviderNotExist());
                }

                //var channelInUse = await _context.ChannelUsers.AnyAsync(x => x.ChannelId == command.Id, cancellationToken);

                //if (channelInUse == true)
                //{
                //    return Result.Failure(ChannelError.ChannelInUse(channels.ChannelName));
                //}


                service.IsActive = !service.IsActive;

                //if(channels.IsActive == false)
                //{

                //    var channelUserList = await _context.ChannelUsers.Where(x => x.ChannelId == channels.Id).ToListAsync();

                //    var ApproverSetupList = await _context.Approvers.Where(x => x.ChannelId == channels.Id).ToListAsync();

                //    foreach (var channelUsers in channelUserList)
                //    {
                //        channelUsers.IsActive = false;
                //    }

                //    foreach (var approver in ApproverSetupList)
                //    {
                //        approver.IsActive = false;
                //    }

                //}

                await _context.SaveChangesAsync();

                var results = new UpdateServiceProviderStatusResult
                {
                    Id = service.Id,
                    Status = service.IsActive
                };

                return Result.Success(results);

            }
        }
    }
}
