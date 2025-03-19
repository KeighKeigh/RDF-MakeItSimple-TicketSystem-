using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Get_Pms_Approver
{
    public partial class GetPmsApprover
    {

        public class Handler : IRequestHandler<GetPmsApproverQuery, PagedList<object>>
        {
            private readonly MisDbContext context;
            private readonly IUnitOfWork unitOfWork;

            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {
                this.context = context;
                this.unitOfWork = unitOfWork;
            }

            public async Task<PagedList<object>> Handle(GetPmsApproverQuery request, CancellationToken cancellationToken)
            {
                IQueryable<PmsApprover> query = context.PmsApprovers
                    .AsNoTrackingWithIdentityResolution();

                if (request.Is_Archived is not null)
                    query = query.Where(q => unitOfWork.PmsApprover.Archived(request.Is_Archived).Contains(q));

                if (!string.IsNullOrEmpty(request.Search))
                    query = query.Where(q => unitOfWork.PmsApprover.Search(request.Search).Contains(q));

                var results = query
                      .GroupBy(x => x.PmsFormId)
                    .Select(pa => new GetPmsApproverResult
                    {
                        PmsFormId = pa.Key.Value,
                        Form_Name = pa.First().PmsForms.Form_Name,
                        PmsUserApprovers = pa.Select(x => new GetPmsApproverResult.PmsUserApprover
                        {
                            Id = x.Id,
                            UserId = x.UserId,
                            Fullname = x.User.Fullname,
                            Added_By = x.AddedByUser.Fullname,
                            Created_At = x.CreatedAt.Date,
                            Modified_By = x.ModifiedByUser.Fullname,
                            Updated_At = x.UpdatedAt,
                            Is_Active = x.IsActive,

                        }).ToList(),

                    });

                results = unitOfWork.PmsApprover.Orders(results, request.Orders);

                return await PagedList<object>.CreateAsync(results,request.PageNumber,request.PageSize);
            }
        }
    }
}
