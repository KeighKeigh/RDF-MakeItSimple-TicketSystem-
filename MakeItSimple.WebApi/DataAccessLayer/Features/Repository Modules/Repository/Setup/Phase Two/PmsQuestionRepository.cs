using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Enumerator;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Setup.Create_Pms_Questionaire.CreatePmsQuestion;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Get_Pms_Form.GetPmsForm.GetPmsFormResult.PmsQuestionModule;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_Two
{
    public class PmsQuestionRepository : IPmsQuestionRepository
    {
        private readonly MisDbContext context;

        public PmsQuestionRepository(MisDbContext context)
        {
            this.context = context;
        }

        public  void CreatePmsQuestion(PmsQuestionaire pmsQuestion)
        {

             context.PmsQuestionaires.AddAsync(pmsQuestion);
        }

        public void CreateQuestionTransaction(QuestionTransactionId questionTransaction)
        {
            context.QuestionTransactionIds.AddAsync(questionTransaction);
        }

        public void CreateQuestionType(PmsQuestionType pQType)
        {
             context.PmsQuestionTypes.AddAsync(pQType);
        }

        public void PmsQuestionTypeUpdate(PmsQuestionType pmsQType)
        {
            var updatePmsQType = context.PmsQuestionTypes
                .FirstOrDefault(x => x.Id == pmsQType.Id);

            updatePmsQType = pmsQType;
        }

        public async Task<bool> PmsQuestionAlreadyExist(string pmsQuestion, string currentQuestion)
        {
            if(string.IsNullOrEmpty(currentQuestion))
                return await context.PmsQuestionaires
                    .AnyAsync(x => x.Question == pmsQuestion);

            return await context.PmsQuestionaires
                .Where(x => x.Question == pmsQuestion
                && !pmsQuestion.Equals(currentQuestion)) 
                .AnyAsync();
        }

        public async Task<bool> PmsQuestionTypeAlreadyExist(string pmsQuestionType, string currentQuestionType)
        {
            if (string.IsNullOrEmpty(currentQuestionType))
                return await context.PmsQuestionTypes
                    .AnyAsync(x => x.Description == pmsQuestionType);

            return await context.PmsQuestionTypes
                .Where(x => x.Description == pmsQuestionType
                && !pmsQuestionType.Equals(currentQuestionType))
                .AnyAsync();
        }

        public async Task<PmsQuestionaire> PmsQuestionNotExist(int id)
        {
            return await context.PmsQuestionaires.FindAsync(id);
        }

        public async Task<bool> QuestionTypeNotExist(string questionType)
        {

            bool doesNotExist = Enum.TryParse(questionType,out QuestionTypeEnumerator result) ? true : false;
            return await Task.FromResult(doesNotExist);

        }

        public async Task<QuestionTransactionId> QuestionTransactionNotExist(int Id)
        {
            return await context.QuestionTransactionIds.FindAsync(Id);
        }
        public async Task<PmsQuestionType> PmsQTypeIdNotExist(int id)
        {
            return await context.PmsQuestionTypes.FindAsync(id);
        }

        public IQueryable<PmsQuestionaire> SearchPmsForm(string search)
        {
            return context.PmsQuestionaires.Where(x => x.Question.ToLower().Contains(search));
        }
        public IQueryable<PmsQuestionaire> ArchivedPmsForm(bool? is_Archived)
        {
            return context.PmsQuestionaires.Where(q => q.IsActive == is_Archived);
        }
        public IQueryable<PmsQuestionaire> OrdersPmsForm(string order_By)
        {
            var query = context.PmsQuestionaires.AsQueryable();

            switch (order_By)
            {
                case PmsConsString.asc:
                    query = query.OrderBy(x => x.Id);
                    break;

                case PmsConsString.desc:
                    query = query.OrderByDescending(x => x.Id);
                    break;

                default:
                    query = query.OrderBy(x => x.Question);
                    break;
            }

            return query;
        }

        public void RemoveQuestionModule(int id)
        {

            context.QuestionTransactionIds
                .Where(x => x.Id == id)
                .ExecuteUpdate(update => update
                .SetProperty(u => u.IsDeleted, u => true));
           
        }


        public async Task<IReadOnlyList<QuestionTransactionId>> QTransactionList(List<int> id)
        {
            return await context.QuestionTransactionIds
                .Where(x => !x.IsDeleted && id.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<IReadOnlyList<QuestionTransactionId>> QTransactionByQModule(int id)
        {
            return await context.QuestionTransactionIds
                .Where(x => !x.IsDeleted && x.Id == id)
                .ToListAsync();
        }

        public void RemoveQType(int id)
        {
                 context.PmsQuestionTypes
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.Is_Deleted, u => true));
        }

        public async Task<IReadOnlyList<PmsQuestionType>> PmsQTypeByQModule(int Id)
        {
            return await context.PmsQuestionTypes
                .Where(x => !x.Is_Deleted && x.PmsQuestionaireId == Id)
                .ToListAsync();
        }

        public void UpdatePmsQ(PmsQuestionaire pmsQuestion)
        {
            var update = context.PmsQuestionaires
                .FirstOrDefault(x => x.Id == pmsQuestion.Id);

            update.Question = update.Question;
            update.QuestionType = pmsQuestion.QuestionType;
            update.ModifiedBy = pmsQuestion.ModifiedBy;
            update.UpdatedAt = DateTime.Now;
        }

        public void UpdatePmsQStatus(int id)
        {

            context.PmsQuestionaires
                .Where(x => x.Id == id)
                .ExecuteUpdate(update => update
                .SetProperty(u => u.IsActive, u => !u.IsActive));

        }
    }
}
