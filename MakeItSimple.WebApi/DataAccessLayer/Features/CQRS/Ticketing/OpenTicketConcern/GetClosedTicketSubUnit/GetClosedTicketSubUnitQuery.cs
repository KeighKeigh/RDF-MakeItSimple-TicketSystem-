using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.OpenTicketConcern.GetClosedTicketSubUnit
{
    public partial class GetClosedTicketSubUnit
    {
        public class GetClosedTicketSubUnitQuery : UserParams, IRequest<PagedList<GetClosedTicketSubUnitResult>>
        {
            public Guid? UserId { get; set; }
            public string UserType { get; set; }
            public string Role { get; set; }
            public string Search { get; set; }
        }
    }
}
