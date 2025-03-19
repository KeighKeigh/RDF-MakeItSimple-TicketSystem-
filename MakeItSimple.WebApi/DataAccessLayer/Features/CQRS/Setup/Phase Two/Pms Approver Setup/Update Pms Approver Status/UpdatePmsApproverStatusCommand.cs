using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Update_Pms_Approver_Status
{
    public partial class UpdatePmsApproverStatus
    {
        public class UpdatePmsApproverStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }
    }
}
