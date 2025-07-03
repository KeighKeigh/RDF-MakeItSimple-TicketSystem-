using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.ApprovalDateTicket
{
    public partial class ApprovalDateTicket
    {
        public class ApprovalDateTicketCommand : IRequest<Result>
        {
            public Guid? ApprovedDateBy { get; set; }
            public string Role { get; set; }
            public Guid? Users { get; set; }
            public Guid? Transacted_By { get; set; }
            public string Modules { get; set; }
            public List<ApproveDateRequest> ApproveDateRequests { get; set; }
            public class ApproveDateRequest
            {
                public int? ApprovalDateTicketId { get; set; }
            }

        }


    }
}
