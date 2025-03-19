using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Update_Pms_Question
{
    public partial class UpdatePmsQuestion
    {

        public class Handler : IRequestHandler<UpdatePmsQuestionCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(UpdatePmsQuestionCommand command, CancellationToken cancellationToken)
            {

                var qTransactionList = new List<int>();
                var qTypeList = new List<int>();

                var pmsQuestionNotExist = await unitOfWork.PmsQuestion
                    .PmsQuestionNotExist(command.Id);
                if (pmsQuestionNotExist is null)
                    return Result.Failure(PmsQuestionaireError.PmsQuestionIdNotExist());

                var questionAlreadyExist = await unitOfWork.PmsQuestion
                    .PmsQuestionAlreadyExist(command.Question, pmsQuestionNotExist.Question);
                if (questionAlreadyExist)
                    return Result.Failure(PmsQuestionaireError.PmsQuestionAlreadyExist());

                var updatePmsQuestion = new PmsQuestionaire
                {
                    Id = command.Id,
                    Question = command.Question,
                    QuestionType = command.Question_Type,
                    ModifiedBy = command.Modified_By,
                };

                unitOfWork.PmsQuestion.UpdatePmsQ(updatePmsQuestion);

                foreach (var pmsQuestionModule in command.PmsQuestionModules)
                {
                    if (command.PmsQuestionModules.Count(x => x.PmsQuestionModuleId == pmsQuestionModule.PmsQuestionModuleId) > 1)
                        return Result.Failure(PmsQuestionaireError.PmsQuestionModuleDuplicated());

                    var pmsQuestionModuleNotExist = await unitOfWork.PmsQuestionaireModules
                        .PmsQuestionaireModuleIdNotExist(pmsQuestionModule.PmsQuestionModuleId);
                    if (pmsQuestionModuleNotExist is null)
                        return Result.Failure(PmsQuestionaireModuleError.PmsQuestionaireModuleIdNotExist());

                    var questionTransactionNotExist = await unitOfWork.PmsQuestion
                        .QuestionTransactionNotExist(pmsQuestionModule.Id.Value);

                    if (questionTransactionNotExist is not null)
                    {
                        qTransactionList.Add(pmsQuestionModule.Id.Value);

                    }
                    else
                    {

                        var newQTransaction = new QuestionTransactionId
                        {
                            PmsQuestionaireModuleId = pmsQuestionModule.PmsQuestionModuleId,
                            PmsQuestionaire = pmsQuestionNotExist
                        };

                        unitOfWork.PmsQuestion.CreateQuestionTransaction(newQTransaction);

                    }
                }

                foreach (var questionType in command.PmsQuestionTypes)
                {
                    if (command.PmsQuestionTypes.Count(x => x.Description == questionType.Description) > 1)
                        return Result.Failure(PmsQuestionaireError.PmsQuestionTypeDuplicated());

                    var qTypeIdNotExist = await unitOfWork.PmsQuestion
                        .PmsQTypeIdNotExist(questionType.Id.Value);

                    if (qTypeIdNotExist is not null)
                    {
                        var updatePmsQType = new PmsQuestionType
                        {
                            Id = questionType.Id.Value,
                            Description = questionType.Description,

                        };

                        unitOfWork.PmsQuestion.PmsQuestionTypeUpdate(updatePmsQType);
                    }
                    else
                    {
                        var newPmsQType = new PmsQuestionType
                        {
                            Description = command.Question_Type.Contains(PmsConsString.TextType) ? "" : questionType.Description,
                            PmsQuestionaire = pmsQuestionNotExist,
                            QuestionType = command.Question_Type,

                        };

                        unitOfWork.PmsQuestion
                           .CreateQuestionType(newPmsQType);
                    }

                }

                var qTransactionByQModuleList = await unitOfWork.PmsQuestion
                    .QTransactionByQModule(command.Id);

                var removeQTransactionList = qTransactionByQModuleList
                    .Where(x => !qTransactionList.Contains(x.Id));

                if (removeQTransactionList.Any())
                {
                    foreach (var remove in removeQTransactionList)
                    {
                        unitOfWork.PmsQuestion.RemoveQuestionModule(remove.Id);
                    }

                }

                var pmsQTypeByQModuleList = await unitOfWork.PmsQuestion
                    .PmsQTypeByQModule(command.Id);

                var removePmsQTypeList = pmsQTypeByQModuleList
                    .Where(x => !qTypeList.Contains(x.Id));

                if (removePmsQTypeList.Any())
                {
                    foreach (var remove in removePmsQTypeList)
                    {
                        unitOfWork.PmsQuestion.RemoveQType(remove.Id);
                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();

            }
        }
    }

}
