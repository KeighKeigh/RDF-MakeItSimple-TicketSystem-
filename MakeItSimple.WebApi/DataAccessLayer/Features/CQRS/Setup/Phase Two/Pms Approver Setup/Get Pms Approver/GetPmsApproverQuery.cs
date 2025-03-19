using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Get_Pms_Approver
{
    public partial class GetPmsApprover
    {
        public sealed class GetPmsApproverQuery : UserParams, IRequest<PagedList<object>>
        {
            public string Search { get; set; }
            public bool? Is_Archived { get; set; }
            public string Orders { get; set; }

        }
    }
}
