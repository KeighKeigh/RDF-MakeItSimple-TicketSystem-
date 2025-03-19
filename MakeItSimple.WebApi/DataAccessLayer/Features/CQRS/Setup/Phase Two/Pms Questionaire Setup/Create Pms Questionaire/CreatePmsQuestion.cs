using DocumentFormat.OpenXml.Office2013.Excel;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Create_Pms_Questionaire
{
    public partial class CreatePmsQuestion
    {

        public class Handler : IRequestHandler<CreatePmsQuestionCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(CreatePmsQuestionCommand command, CancellationToken cancellationToken)
            {
                var questionAlreadyExist = await unitOfWork.PmsQuestion
                    .PmsQuestionAlreadyExist(command.Question,null);

                if (questionAlreadyExist)
                    return Result.Failure(PmsQuestionaireError.PmsQuestionAlreadyExist());

                var questionTypeNotExist = await unitOfWork.PmsQuestion.QuestionTypeNotExist(command.Question_Type);
                if (!questionTypeNotExist)
                    return Result.Failure(PmsQuestionaireError.PmsQuestionTypeNotExist());

                var newPmsQuestion = new PmsQuestionaire
                {
                    Question = command.Question,
                    QuestionType = command.Question_Type,
                    AddedBy = command.Added_By,        
                };

                 unitOfWork.PmsQuestion.CreatePmsQuestion(newPmsQuestion);

                foreach (var pmsQuestionModule in command.PmsQuestionModules)
                {

                    if (command.PmsQuestionModules.Count(x => x.PmsQuestionModuleId == pmsQuestionModule.PmsQuestionModuleId) > 1)
                        return Result.Failure(PmsQuestionaireError.PmsQuestionModuleDuplicated());

                    var pmsQuestionModuleNotExist = await unitOfWork.PmsQuestionaireModules
                        .PmsQuestionaireModuleIdNotExist(pmsQuestionModule.PmsQuestionModuleId);
                    if(pmsQuestionModuleNotExist is null)
                        return Result.Failure(PmsQuestionaireModuleError.PmsQuestionaireModuleIdNotExist());

                    var newQTransaction = new QuestionTransactionId
                    {
                        PmsQuestionaireModuleId = pmsQuestionModule.PmsQuestionModuleId,
                        PmsQuestionaire = newPmsQuestion

                    };

                     unitOfWork.PmsQuestion.CreateQuestionTransaction(newQTransaction);
                }

                foreach (var questionType in command.PmsQuestionTypes)
                {
                    if (command.PmsQuestionTypes.Count(x => x.Description == questionType.Description) > 1)
                        return Result.Failure(PmsQuestionaireError.PmsQuestionTypeDuplicated());

                    var newPmsQType = new PmsQuestionType
                    {
                        Description = command.Question_Type.Contains(PmsConsString.TextType) ? "" : questionType.Description,
                        PmsQuestionaire = newPmsQuestion,
                        QuestionType = command.Question_Type,

                    };

                     unitOfWork.PmsQuestion
                        .CreateQuestionType(newPmsQType);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
