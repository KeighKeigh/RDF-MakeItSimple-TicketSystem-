using MakeItSimple.WebApi.Models.Setup.Phase_Two;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two
{
    public interface IPmsQuestionRepository
    {
        Task<bool> PmsQuestionAlreadyExist(string pmsQuestion , string currentQuestion);
        Task<bool> PmsQuestionTypeAlreadyExist(string pmsQuestionType, string currentQuestionType);
        Task<PmsQuestionaire> PmsQuestionNotExist(int id);
        Task<bool> QuestionTypeNotExist(string questionType);
        Task<PmsQuestionType> PmsQTypeIdNotExist(int id);
        Task<IReadOnlyList<QuestionTransactionId>> QTransactionList(List<int> id);
        Task<IReadOnlyList<QuestionTransactionId>> QTransactionByQModule(int id);
        Task<QuestionTransactionId> QuestionTransactionNotExist(int Id);
        Task<IReadOnlyList<PmsQuestionType>> PmsQTypeByQModule(int Id);


        void CreatePmsQuestion(PmsQuestionaire pmsQuestion);
        void CreateQuestionTransaction(QuestionTransactionId questionTransaction);
        void CreateQuestionType(PmsQuestionType pmsQType);
        void PmsQuestionTypeUpdate(PmsQuestionType pmsQType);
        void RemoveQuestionModule(int id);
        void RemoveQType(int id);
        void UpdatePmsQ(PmsQuestionaire pmsQuestion);
        void UpdatePmsQStatus(int id);


        IQueryable<PmsQuestionaire> SearchPmsForm(string search);
        IQueryable<PmsQuestionaire> ArchivedPmsForm(bool? is_Archived);
        IQueryable<PmsQuestionaire> OrdersPmsForm(string order_By);

    }
}
