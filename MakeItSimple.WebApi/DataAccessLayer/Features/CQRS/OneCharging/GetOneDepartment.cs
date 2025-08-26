using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.OneCharging;
using MediatR;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneCompany;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging
{
    public class GetOneDepartment
    {
        public class GetOneDepartmentResult
        {
            public int? DepartmentId { get; set; }
            public string DepartmentCode { get; set; }
            public string DepartmentName { get; set; }

            public DateTime? deleted_at { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class GetOneDepartmentQuery : UserParams, IRequest<PagedList<GetOneDepartmentResult>>
        {
            public string Search { get; set; }

            public bool? Status { get; set; }
        }


        public class Handler : IRequestHandler<GetOneDepartmentQuery, PagedList<GetOneDepartmentResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOneDepartmentResult>> Handle(GetOneDepartmentQuery request, CancellationToken cancellationToken)
            {
                IQueryable<OneChargingMIS> oneChargingList = _context.OneChargings;

                if (!string.IsNullOrEmpty(request.Search))
                {
                    oneChargingList = oneChargingList.Where(x => x.department_code.ToLower().Contains(request.Search.ToLower())
                    || x.department_name.ToLower().Contains(request.Search.ToLower()));
                }

                if (request.Status != null)
                {
                    oneChargingList = oneChargingList.Where(x => x.IsActive == request.Status);
                }

                var result = oneChargingList.GroupBy(x => x.department_id).Select(x => new GetOneDepartmentResult
                {
                    DepartmentId = x.Key,
                    DepartmentCode = x.First().department_code,
                    DepartmentName = x.First().department_name,
                    deleted_at = x.First().deleted_at,
                    IsActive = x.First().IsActive,
                    UpdatedAt = x.First().UpdatedAt,
                });

                return await PagedList<GetOneDepartmentResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}
