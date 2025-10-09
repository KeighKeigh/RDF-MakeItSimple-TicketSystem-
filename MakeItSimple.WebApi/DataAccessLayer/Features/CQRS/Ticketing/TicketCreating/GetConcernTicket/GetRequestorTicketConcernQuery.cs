using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetConcernTicket
{
    public partial class GetRequestorTicketConcern
    {
        public class GetRequestorTicketConcernQuery : UserParams, IRequest<PagedList<GetRequestorTicketConcernResult>>
        {

            public string UserType { get; set; }
            public string Role { get; set; }
            public Guid? UserId { get; set; }
            public string Concern_Status { get; set; }
            public string Severity { get; set; }
            public string Search { get; set; }
            public bool? Status { get; set; }
            public bool? Is_Reject { get; set; }
            public bool? Is_Approve { get; set; }
            public bool? Ascending { get; set; }
            public string ServiceProviderName { get; set; }
            public string ChannelName { get; set; }

            public int? LocationId { get; set; }
            public int? DepartmentId { get; set; }
            public int? CategoryId { get; set; }

            public DateTime? DateFrom { get; set; }
            public DateTime? DateTo { get; set; }
        }
    }
}