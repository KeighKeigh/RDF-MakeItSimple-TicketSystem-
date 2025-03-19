﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.TransferUser
{
    public partial class TransferTicketUser
    {

        public class Handler : IRequestHandler<TransferTicketUserCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(TransferTicketUserCommand command, CancellationToken cancellationToken)
            {
                var transferAlreadyTransferToList = await _context.TransferTicketConcerns
                    .AsNoTracking()
                    .Where(t => t.IsTransfer == true && t.TicketConcernId == command.TicketConcernId)
                    .Select(x => x.TransferTo)
                    .ToListAsync();

                var transferAlreadyTransferByList = await _context.TransferTicketConcerns
                    .AsNoTracking()
                    .Where(t => t.IsTransfer == true && t.TicketConcernId == command.TicketConcernId)
                    .Select(x => x.TransferBy)
                    .ToListAsync();

                var ticketConcern = await _context.TicketConcerns
                    .Include(t => t.RequestConcern)
                    .Where(x => x.Id == command.TicketConcernId)
                    .FirstOrDefaultAsync();

                if (ticketConcern is null)
                    return Result.Failure(TicketRequestError.TicketConcernIdNotExist());

                var result = await _context.ChannelUsers
                    .AsNoTracking()
                    .Include(r => r.User)
                    .Include(r => r.Channel)
                    .AsSplitQuery()
                    .Where(r => r.ChannelId == ticketConcern.RequestConcern.ChannelId
                    && !transferAlreadyTransferToList.Contains(r.UserId) && !transferAlreadyTransferByList.Contains(r.UserId) && r.UserId != command.Transfer_By && r.UserId != ticketConcern.UserId)
                    .Select(r => new TransferTicketUserResult
                    {
                        Id = r.UserId,
                        EmpId = r.User.EmpId,
                        Fullname = r.User.Fullname,
                        Channel_Name = r.Channel.ChannelName,

                    }).ToListAsync();

                return Result.Success(result);
                
            }
        }
    }
}
