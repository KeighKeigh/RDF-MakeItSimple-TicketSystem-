using MakeItSimple.WebApi.Models.Setup.FormSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;

namespace MakeItSimple.WebApi.Models
{
    public class PmsDetail : BaseIdEntity
    {
        public int? PmsId { get; set; }
        public virtual Pms Pms { get; set; }
        public int? PmsQuestionaireModuleId { get; set; }
        public virtual PmsQuestionaireModule PmsQuestionaireModule { get; set; }
        public int? PmsQuestionaireId { get; set; }
        public virtual PmsQuestionaire PmsQuestionaire { get; set; }
        public int PmsQuestionTypeId { get; set; }
        public virtual PmsQuestionType PmsQuestionType { get; set; }
        public string Answer { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
