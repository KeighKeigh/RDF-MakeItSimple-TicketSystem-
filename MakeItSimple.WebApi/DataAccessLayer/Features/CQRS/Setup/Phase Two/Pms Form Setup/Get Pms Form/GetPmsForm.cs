﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Get_Pms_Form
{
    public partial class GetPmsForm
    {

        public class Handler : IRequestHandler<GetPmsFormQuery, PagedList<object>>
        {
            private readonly MisDbContext context;
            private readonly IUnitOfWork unitOfWork;

            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {
                this.context = context;
                this.unitOfWork = unitOfWork;
            }

            public async Task<PagedList<object>> Handle(GetPmsFormQuery request, CancellationToken cancellationToken)
            {
                 IQueryable<PmsForm> query = context.PmsForms
                     .AsNoTracking()
                     .OrderBy(x => x.Id)
                     .AsSplitQuery();

                if (!string.IsNullOrEmpty(request.Orders))
                    query = unitOfWork.PmsForm.OrdersPmsForm(request.Orders);

                if (request.Is_Archived is not null)
                    query = query.Where(q => unitOfWork.PmsForm.ArchivedPmsForm(request.Is_Archived).Contains(q));

                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(q => unitOfWork.PmsForm.SearchPmsForm(request.Search).Contains(q));

                 var results = query
                    .Select(q => new GetPmsFormResult
                    {
                        Id = q.Id,
                        Form_Name = q.Form_Name,
                        Added_By = q.AddedByUser.Fullname,
                        Created_At = q.CreatedAt.Date,
                        Modified_By = q.ModifiedByUser.Fullname,
                        Updated_At = q.UpdatedAt.Value.Date,
                        Is_Archived = q.IsActive,
                        PmsQuestionModules = q.PmsQuestionaireModules
                        .Where(pqm => pqm.IsActive == true)
                        .Select(pqm => new GetPmsFormResult.PmsQuestionModule
                        {
                            Id = pqm.Id,
                            Question_Module_Name = pqm.QuestionaireModuleName,
                            PmsQuestions = pqm.QuestionTransactionIds
                            .GroupBy(x => new
                            {
                                x.PmsQuestionaireModuleId,
                                x.PmsQuestionaireId,
                            })
                            .Select(x => new GetPmsFormResult.PmsQuestionModule.PmsQuestion
                            {
                                Id = x.Key.PmsQuestionaireId,
                                Question_Name = x.First().PmsQuestionaire.Question,
                                Question_Type = x.First().PmsQuestionaire.QuestionType,
                                QuestionChoices = x.First().PmsQuestionaire.QuestionType.Equals(PmsConsString.Dropdown) ? x.First().PmsQuestionaire.PmsQuestionTypes
                                .Select(x => new GetPmsFormResult.PmsQuestionModule.PmsQuestion.QuestionChoice
                                {
                                    Id = x.Id,
                                    Description = x.Description,
                                   
                                }).ToList() : null,

                            }).ToList(),

                        }).ToList()

                    });

                return await PagedList<object>.CreateAsync(results, request.PageNumber,request.PageSize);   
            }
        }
    }
}
