using MakeItSimple.WebApi.Models.Ticketing;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Ticketing
{
    public interface IApproverDateRepository
    {
        Task<ApproverDate> ApproverDateExist(int? id);

        Task<List<ApproverTicketing>> ApproverByDateApprovalTicketList(int? id);
        Task<ApproverTicketing> ApproverByMinLevel(int? id);
        Task<ApproverTicketing> ApproverPlusOne(int? id, int level);
        Task ApprovedApproval(int? id);
        Task NextApproverUser(int? id, Guid? userId);
        Task ApprovedDateTicket(ApproverDate closingTicket, CancellationToken cancellationToken);
        Task ApprovedTicketConcernByApprovingDate(TicketConcern ticketConcern, CancellationToken cancellationToken);
        Task ApprovedRequestConcernByApprovingDate(RequestConcern requestConcern, CancellationToken cancellation);
        Task RejectTargetDateTicket(ApproverDate approveDateTicket);
        Task RemoveTargetDateApprover(int? id);
    }
}
