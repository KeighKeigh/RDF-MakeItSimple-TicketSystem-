using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OnHoldTicket.Reject_On_Hold
{
    public partial class RejectOnHold
    {
        public class RejectOnHoldCommand : IRequest<Result>
        {
            public Guid? RejectOnHold_By { get; set; }
            public Guid? Transacted_By { get; set; }
            public string Role { get; set; }
            public string Reject_Remarks { get; set; }

            public List<RejectOnHoldRequest> RejectOnHoldRequests { get; set; }
            public class RejectOnHoldRequest
            {
                public int? OnHoldTicketId { get; set; }
            }
        }
    }
}
