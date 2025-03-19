using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Update_Pms_Question_Status
{
    public partial class UpdatePmsQuestionStatus
    {

        public class Handler : IRequestHandler<UpdatePmsQuestionStatusCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(UpdatePmsQuestionStatusCommand command, CancellationToken cancellationToken)
            {

                var pmsQuestionNotExist = await unitOfWork.PmsQuestion
                    .PmsQuestionNotExist(command.Id);
                if (pmsQuestionNotExist is null)
                    return Result.Failure(PmsQuestionaireError.PmsQuestionIdNotExist());

                unitOfWork.PmsQuestion.UpdatePmsQStatus(command.Id);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success(pmsQuestionNotExist); 

            }
        }
    }
}
