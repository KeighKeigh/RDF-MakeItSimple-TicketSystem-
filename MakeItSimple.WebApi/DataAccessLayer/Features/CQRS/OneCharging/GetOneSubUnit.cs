using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.OneCharging;
using MediatR;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneDepartment;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging
{
    public class GetOneSubUnit
    {
        public class GetOneSubUnitResult
        {
            public int? SubUnitId { get; set; }
            public string SubUnitCode { get; set; }
            public string SubUnitName { get; set; }
            public DateTime? deleted_at { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class GetOneSubUnitQuery : UserParams, IRequest<PagedList<GetOneSubUnitResult>>
        {
            public string Search { get; set; }

            public bool? Status { get; set; }
        }


        public class Handler : IRequestHandler<GetOneSubUnitQuery, PagedList<GetOneSubUnitResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOneSubUnitResult>> Handle(GetOneSubUnitQuery request, CancellationToken cancellationToken)
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

                var result = oneChargingList.GroupBy(x => x.sub_unit_id).Select(x => new GetOneSubUnitResult
                {
                    SubUnitId = x.Key,
                    SubUnitCode = x.First().sub_unit_code,
                    SubUnitName = x.First().sub_unit_name,
                    deleted_at = x.First().deleted_at,
                    IsActive = x.First().IsActive,
                    UpdatedAt = x.First().UpdatedAt,
                });

                return await PagedList<GetOneSubUnitResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}
