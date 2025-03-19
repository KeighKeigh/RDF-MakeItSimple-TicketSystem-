using MakeItSimple.WebApi.Common;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Update_Pms_Question
{
    public partial class UpdatePmsQuestion
    {
        public class UpdatePmsQuestionCommand : IRequest<Result>
        {

            public int Id { get; set; }
            public string Question { get; set; }
            public string Question_Type { get; set; }
            public Guid? Modified_By { get; set; }

            public List<UpdateQuestionModule> PmsQuestionModules { get; set; }

            public class UpdateQuestionModule
            {
                public int? Id { get; set; }
                public int PmsQuestionModuleId { get; set; }

            }

            public List<UpdatePmsQuestionType> PmsQuestionTypes { get; set; }
            public class UpdatePmsQuestionType
            {
                public int? Id { get; set; }
                public string Description { get; set; }
            }
        }
    }

}
