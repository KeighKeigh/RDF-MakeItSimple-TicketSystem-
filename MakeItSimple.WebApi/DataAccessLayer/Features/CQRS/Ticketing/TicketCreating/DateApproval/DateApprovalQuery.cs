using MakeItSimple.WebApi.Common.Pagination;
using MediatR;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.NewFolder
{
    public partial class DateApproval
    {
        public class DateApprovalQuery : UserParams, IRequest<PagedList<DateApprovalResult>>
        {
            public Guid? UserId { get; set; }
            public string UserType { get; set; }
            public string Role { get; set; }
            public string Search { get; set; }
            public bool? IsDateApproved { get; set; }
            public bool? IsReject { get; set; }
        }
    }
}
