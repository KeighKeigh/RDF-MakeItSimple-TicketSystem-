using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.UpsertPms
{
    public partial class UpsertPms
    {
        public sealed class UpsertPmsCommand : IRequest<Result>
        {
            public int? Id { get; set; }
            public int? PmsFormId { get; set; }
            public Guid? Requestor {  get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }

            public List<UpsertQuestionModule> UpsertQuestionModules { get; set; }
            public sealed class UpsertQuestionModule
            {
                public int? PmsQuestionaireModuleId { get; set; }
                public List<PmsDetail> PmsDetails { get; set; }
                public sealed class PmsDetail
                {
                    public int? PmsDetailId { get; set; }
                    public int? PmsQuestionaireId { get; set; }
                    public int PmsQuestionTypeId { get; set; }
                    public string Answer { get; set; }

                }

            }

            public List<PmsAttachment> PmsAttachments { get; set; }
            public sealed class PmsAttachment
            {
                public int? PmsAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }

            }


            public List<PmsTechnician> PmsTechnicians { get; set; }
            public sealed class PmsTechnician
            {
                public int? PmsTechnicianId { get; set; }
                public Guid? Technician { get; set;}

            }

        }
    }
}
