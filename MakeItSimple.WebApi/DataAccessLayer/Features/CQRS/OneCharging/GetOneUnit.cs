using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.OneCharging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneDepartment;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging
{
    public class GetOneUnit
    {
        public class GetOneUnitResult
        {
            public int? UnitId { get; set; }
            public string UnitCode { get; set; }
            public string UnitName { get; set; }
            public DateTime? deleted_at { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class GetOneUnitQuery : UserParams, IRequest<PagedList<GetOneUnitResult>>
        {
            public string Search { get; set; }

            public bool? Status { get; set; }
        }


        public class Handler : IRequestHandler<GetOneUnitQuery, PagedList<GetOneUnitResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOneUnitResult>> Handle(GetOneUnitQuery request, CancellationToken cancellationToken)
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

                var result = oneChargingList.GroupBy(x => x.department_unit_id).Select(x => new GetOneUnitResult
                {
                    UnitId = x.Key,
                    UnitCode = x.First().department_unit_code,
                    UnitName = x.First().department_unit_name,
                    deleted_at = x.First().deleted_at,
                    IsActive = x.First().IsActive,
                    UpdatedAt = x.First().UpdatedAt,
                });

                return await PagedList<GetOneUnitResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}
