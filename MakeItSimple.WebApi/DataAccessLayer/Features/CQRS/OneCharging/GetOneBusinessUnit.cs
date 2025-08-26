using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.OneCharging;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging
{
    public class GetOneBusinessUnit
    {
        public class GetOneBusinessUnitResult
        {
            public int? BusinessUnitId { get; set; }
            public string BusinessUnitCode { get; set; }
            public string BusinessUnitName { get; set; }
            public DateTime? deleted_at { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class GetOneBusinessUnitQuery : UserParams, IRequest<PagedList<GetOneBusinessUnitResult>>
        {
            public string Search { get; set; }

            public bool? Status { get; set; }
        }


        public class Handler : IRequestHandler<GetOneBusinessUnitQuery, PagedList<GetOneBusinessUnitResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOneBusinessUnitResult>> Handle(GetOneBusinessUnitQuery request, CancellationToken cancellationToken)
            {
                IQueryable<OneChargingMIS> oneChargingList = _context.OneChargings;

                if (!string.IsNullOrEmpty(request.Search))
                {
                    oneChargingList = oneChargingList.Where(x => x.business_unit_name.ToLower().Contains(request.Search.ToLower())
                    || x.business_unit_code.ToLower().Contains(request.Search.ToLower()));
                }

                if (request.Status != null)
                {
                    oneChargingList = oneChargingList.Where(x => x.IsActive == request.Status);
                }

                var result = oneChargingList.GroupBy(x => x.business_unit_id).Select(x => new GetOneBusinessUnitResult
                {
                    BusinessUnitId = x.Key,
                    BusinessUnitCode = x.First().business_unit_code,
                    BusinessUnitName = x.First().business_unit_name,
                    deleted_at = x.First().deleted_at,
                    IsActive = x.First().IsActive,
                    UpdatedAt = x.First().UpdatedAt,
                });

                return await PagedList<GetOneBusinessUnitResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}
