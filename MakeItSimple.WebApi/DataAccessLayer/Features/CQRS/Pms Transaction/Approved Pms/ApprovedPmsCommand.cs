using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Approved_Pms
{
    public partial class ApprovedPms
    {
        public class ApprovedPmsCommand : IRequest<Result>
        {
            public Guid? UserId { get; set; }
            public string Role {  get; set; }
            public int PmsId { get; set; }
        }
    }
}
