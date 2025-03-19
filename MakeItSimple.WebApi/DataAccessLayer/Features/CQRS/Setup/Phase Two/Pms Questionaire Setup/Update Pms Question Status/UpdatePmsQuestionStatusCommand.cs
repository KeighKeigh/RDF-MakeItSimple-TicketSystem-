using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Update_Pms_Question_Status
{
    public partial class UpdatePmsQuestionStatus
    {
        public class UpdatePmsQuestionStatusCommand : IRequest<Result>
        {
          
            public int Id { get; set; } 

        }
    }
}
