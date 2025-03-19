namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Get_Pms
{
    public partial class GetPms
    {
        public record GetPmsResult
        {
            public int Id { get; set; }
            public int? PmsFormId { get; set; }
            public string Form_Name { get; set; }
            public Guid? RequestorId { get; set; }
            public string Requestor_Name { get; set; }
            public List<PmsQuestionModule> PmsQuestionModules { get; set; }

            public record PmsQuestionModule
            {
                public int ? PmsQuestionaireModuleId { get; set; }
                public string Question_Module_Name { get; set; }
                public List<PmsQuestion> PmsQuestions { get; set; }

                public record PmsQuestion
                {
                    public int? PmsQuestionaireId { get; set; }
                    public string Question {  get; set; }
                    public string Question_Type { get; set; }
                    public List<QuestionTypeChoice> QuestionTypeChoices {  get; set; }
                    public record QuestionTypeChoice
                    {
                        public int PmsQuestionTypeId { get; set; }
                        public string Description { get; set; }

                    }

                    public List<PmsQuestionDetail> PmsQuestionDetails { get; set; }

                    public record PmsQuestionDetail
                    {
                        public int PmsQuestionTypeId { get; set; }
                        public string Description { get; set; }
                        public int PmsDetailId { get; set; }
                        public string Answer { get; set; }
                    }
                }

            }
            public List<PmsTechnician> PmsTechnicians { get; set; }

            public record PmsTechnician
            {
                public int PmsTechnicianId { get; set; }
                public Guid? TechnicianId { get; set; }
                public string Technician { get; set; }
            }

            public List<PmsAttachment> PmsAttachments { get; set; }
            public record PmsAttachment 
            { 
                public int PmsAttachmentId { get; set; }
                public string Attachment { get; set; }
                public string FileName { get; set; }
                public decimal? FileSize { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }

            }

            public string Added_By { get; set; }
            public DateTime? Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public bool? Is_Approved { get; set; }
            public bool? Is_Rejected { get; set; }
            public string Pms_Status {  get; set; }

        }
    }
}
