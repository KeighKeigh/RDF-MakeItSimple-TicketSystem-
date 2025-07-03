using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.RejectDateTicket
{
    public partial class RejectDateTicket
    {
        public class RejectDateTicketCommand : IRequest<Result>
        {
            public Guid? RejectDate_By { get; set; }
            public Guid? Transacted_By { get; set; }
            public string Reject_Remarks { get; set; }
            public string Modules { get; set; }
            public List<RejectDateRequest> RejectDateRequests { get; set; }
            public class RejectDateRequest
            {
                public int? ApproverDateId { get; set; }
            }
        }
    }
}
