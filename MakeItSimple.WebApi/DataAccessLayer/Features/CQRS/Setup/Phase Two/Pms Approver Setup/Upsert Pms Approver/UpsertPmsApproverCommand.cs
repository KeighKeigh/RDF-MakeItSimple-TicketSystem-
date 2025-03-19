using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Upsert_Pms_Approver
{
    public partial class UpsertPmsApprover
    {
        public sealed class UpsertPmsApproverCommand : IRequest<Result>
        {
            public int PmsFormId { get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public List<CreatePmsApprover> PmsApprovers { get; set; }
            public sealed class CreatePmsApprover
            {
                public int? PmsApproverId { get; set; }
                public Guid? UserId { get; set; }
                public int? ApproverLevel { get; set; }
            }

        }

    }
}
