using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Get_Pms
{
    public partial class GetPms
    {

        public class Handler : IRequestHandler<GetPmsQuery, PagedList<GetPmsResult>>
        {
            private readonly MisDbContext context;
            private readonly IUnitOfWork unitOfWork;

            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {
                this.context = context;
                this.unitOfWork = unitOfWork;
            }

            public async Task<PagedList<GetPmsResult>> Handle(GetPmsQuery request, CancellationToken cancellationToken)
            {

                IQueryable<Pms> query = context.Pms
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.PmsDetails)
                    .AsSplitQuery()
                    .OrderBy(x => x.Id);

                if(!string.IsNullOrEmpty(request.Search))
                    query = query.Where(x => unitOfWork.Pms.Search(request.Search).Contains(x));

                if(!string.IsNullOrEmpty(request.Pms_Status))
                {
                    switch(request.Pms_Status)
                    {
                        case PmsConsString.Rejected:
                            query = query.Where(x => unitOfWork.Pms.RejectedFilter().Contains(x));
                            break;
                        case PmsConsString.ForApproval:
                            query = query.Where(x => unitOfWork.Pms.ForApprovalFilter().Contains(x));
                            break;
                        case PmsConsString.Approved:
                            query = query.Where(x => unitOfWork.Pms.ApprovedFilter().Contains(x));
                            break;
                        default:
                            return new PagedList<GetPmsResult>(new List<GetPmsResult>(), 0, request.PageNumber, request.PageSize);
                    }
                }

                if (!string.IsNullOrEmpty(request.User_Type))
                {
                    switch (request.User_Type)
                    {
                        case PmsConsString.Requestor :
                            query = query
                                .Where(x => unitOfWork.Pms.UserTypeRequestor(request.UserId).Contains(x));
                            break;

                        case PmsConsString.Approver :

                            var pmsApprovalList = await unitOfWork.Pms.MinimumLevelOfApproverList();

                            var pmsApprovalByUserList = pmsApprovalList
                                .Where(x => x.UserId.Equals(request.UserId))
                                .Select(x => x.PmsId);

                            query = query.Where(x => pmsApprovalByUserList.Contains(x.Id));                         
                            break;

                        default:
                            return new PagedList<GetPmsResult>(new List<GetPmsResult>(), 0, request.PageNumber, request.PageSize);

                    }
                    
                }

                var results = query
                    .Select(x => new GetPmsResult
                    {
                        Id = x.Id,
                        PmsFormId = x.PmsFormId,
                        Form_Name = x.PmsForm.Form_Name,
                        RequestorId = x.Requestor,
                        Requestor_Name = x.RequestorByUser.Fullname,
                        PmsQuestionModules = x.PmsDetails
                        .GroupBy(x => x.PmsQuestionaireModuleId)
                        .Select(x => new GetPmsResult.PmsQuestionModule
                        {
                            PmsQuestionaireModuleId = x.Key,
                            Question_Module_Name = x.First().PmsQuestionaireModule.QuestionaireModuleName,
                            PmsQuestions = x.GroupBy(x => x.PmsQuestionaireId)
                            .Select(x => new GetPmsResult.PmsQuestionModule.PmsQuestion
                            {
                                PmsQuestionaireId = x.Key,
                                Question = x.First().PmsQuestionaire.Question,
                                Question_Type = x.First().PmsQuestionaire.QuestionType,
                                QuestionTypeChoices = x.First().PmsQuestionaire.QuestionType.Equals(PmsConsString.Dropdown) ?
                                x.First().PmsQuestionType.PmsQuestionaire.PmsQuestionTypes
                                .Select(x => new GetPmsResult.PmsQuestionModule.PmsQuestion.QuestionTypeChoice
                                {
                                    PmsQuestionTypeId = x.Id,
                                    Description = x.Description,

                                }).ToList() : null,
                                PmsQuestionDetails = x.Select(x => new GetPmsResult.PmsQuestionModule.PmsQuestion.PmsQuestionDetail
                                {
                                    PmsQuestionTypeId = x.PmsQuestionTypeId,
                                    Description = x.PmsQuestionType.Description,
                                    PmsDetailId = x.Id,
                                    Answer = x.Answer

                                }).ToList(),

                            }).ToList(),

                        }).ToList(),

                        PmsTechnicians = x.PmsTechnicians
                        .Select(x => new GetPmsResult.PmsTechnician
                        {
                            PmsTechnicianId = x.Id,
                            TechnicianId = x.UserId,
                            Technician = x.User.Fullname,
                        }).ToList(),

                        PmsAttachments = x.PmsAttachments
                        .Select(x => new GetPmsResult.PmsAttachment()
                        {
                            PmsAttachmentId = x.Id,
                            Attachment = x.Attachment,
                            FileName = x.FileName,
                            FileSize = x.FileSize,
                            Added_By = x.AddedByUser.Fullname,
                            Created_At = x.CreatedAt

                        }).ToList(),
                        Added_By = x.AddedByUser.Fullname,
                        Created_At = x.CreatedAt.Date,
                        Modified_By = x.ModifiedByUser.Fullname,
                        Updated_At = x.UpdatedAt,
                        Is_Approved = x.IsApproved,
                        Is_Rejected = x.IsRejected,
                        Pms_Status = x.IsRejected == true ? PmsConsString.Rejected
                        : x.IsApproved != true ? PmsConsString.ForApproval : PmsConsString.Approved

                    });


                results = unitOfWork.Pms.Orders(results, request.Orders);

                return await PagedList<GetPmsResult>.CreateAsync(results,request.PageNumber, request.PageSize);
            }
        }
    }
}
