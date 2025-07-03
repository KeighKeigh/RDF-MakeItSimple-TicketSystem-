using DocumentFormat.OpenXml.InkML;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Ticketing;
using MakeItSimple.WebApi.Models.Ticketing;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Ticketing
{
    public class ApproverDateRepository : IApproverDateRepository
    {
        private readonly MisDbContext context;

        public ApproverDateRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<List<ApproverTicketing>> ApproverByDateApprovalTicketList(int? id)
        {
            return await context.ApproverTicketings
                 .Where(x => x.ClosingTicketId == id)
                 .ToListAsync();
        }

        public async Task<ApproverDate> ApproverDateExist(int? id)
        {
            return await context.ApproverDates
                .Include(x => x.TicketConcern)
                .ThenInclude(x => x.RequestorByUser)
                .Include(x => x.TicketConcern)
                .ThenInclude(x => x.RequestConcern)
                .ThenInclude(x => x.User)
                .Include(x => x.TicketConcern)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<ApproverTicketing> ApproverByMinLevel(int? id)
        {
            var approverList = await context.ApproverTicketings
                  .Where(x => x.ApproverDateId == id && x.IsApprove == null)
                  .ToListAsync();

            return approverList
                .FirstOrDefault(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));
        }

        public async Task<ApproverTicketing> ApproverPlusOne(int? id, int level)
        {
            return await context.ApproverTicketings
                .Include(x => x.User)
                .Where(x => x.ApproverDateId == id)
                .FirstOrDefaultAsync(x => x.ApproverLevel == level + 1);
        }
        public async Task ApprovedApproval(int? id)
        {
            await context.ApproverTicketings
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.IsApprove, u => true));
        }

        public async Task NextApproverUser(int? id, Guid? userId)
        {
            await context.ApproverDates
                 .Where(x => x.Id == id)
                 .ExecuteUpdateAsync(update => update
                 .SetProperty(u => u.TicketApprover, u => userId));
        }

        public async Task ApprovedDateTicket(ApproverDate ApproveDateTicket, CancellationToken cancellationToken)
        {

            await context.ApproverDates
                .Where(x => x.Id == ApproveDateTicket.Id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.TicketApprover, u => null)
                .SetProperty(u => u.IsApproved, u => true)
                .SetProperty(u => u.ApprovedDateAt, u => DateTime.Now)
                .SetProperty(u => u.ApprovedDateBy, u => ApproveDateTicket.ApprovedDateBy));
        }

        public async Task ApprovedTicketConcernByApprovingDate(TicketConcern ticketConcern, CancellationToken cancellationToken)
        {

            await context.TicketConcerns
                .Where(x => x.Id == ticketConcern.Id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.IsDateApproved, u => true)
                .SetProperty(u => u.DateApprovedAt, u => DateTime.Now)
                .SetProperty(u => u.ApprovedDateBy, u => ticketConcern.ApprovedDateBy)
                .SetProperty(u => u.ConcernStatus, u => ticketConcern.ConcernStatus)
                .SetProperty(u =>  u.IsAssigned, u => ticketConcern.IsAssigned)
                .SetProperty(u => u.IsApprove, u => ticketConcern.IsApprove));
        }

        public async Task ApprovedRequestConcernByApprovingDate(RequestConcern requestConcern, CancellationToken cancellation)
        {
            await context.RequestConcerns
                .Where(x => x.Id == requestConcern.Id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.Resolution, u => requestConcern.Resolution)
                .SetProperty(u => u.ConcernStatus, u => requestConcern.ConcernStatus));

        }

        public async Task RejectTargetDateTicket(ApproverDate approveDateTicket)
        {
            var rejectClosing = await context.ApproverDates
                .FirstOrDefaultAsync(x => x.Id == approveDateTicket.Id);


            await context.ApproverDates
                .Where(x => x.Id == approveDateTicket.Id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.RejectDateAt, u => DateTime.Now)
                .SetProperty(u => u.IsRejectDate, u => true)
                .SetProperty(u => u.RejectDateBy, u => approveDateTicket.RejectDateBy)
                .SetProperty(u => u.RejectRemarks, u => approveDateTicket.RejectRemarks)
                .SetProperty(u => u.Remarks, u => approveDateTicket.RejectRemarks));

            await context.TicketConcerns
                .Where(x => x.Id == rejectClosing.TicketConcernId)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.Remarks, u => approveDateTicket.RejectRemarks));

        }

        public async Task RemoveTargetDateApprover(int? id)
        {
            await context.ApproverTicketings
                 .Where(x => x.ApproverDateId == id)
                 .ExecuteDeleteAsync();
        }

    }

}
