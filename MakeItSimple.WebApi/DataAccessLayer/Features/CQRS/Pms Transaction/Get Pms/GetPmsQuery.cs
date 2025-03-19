using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Get_Pms
{
    public partial class GetPms
    {
        public sealed class GetPmsQuery : UserParams, IRequest<PagedList<GetPmsResult>>
        {
            public Guid? UserId { get; set; }
            public string Search { get; set; }
            public string Orders { get; set; }
            public string Pms_Status { get; set; }
            public string User_Type { get; set; }
            
        }
    }
}
