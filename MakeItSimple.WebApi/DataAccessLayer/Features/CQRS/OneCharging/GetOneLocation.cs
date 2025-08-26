using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.OneCharging;
using MediatR;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneDepartment;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging
{
    public class GetOneLocation
    {
        public class GetOneLocationResult
        {
            public int? LocationId { get; set; }
            public string LocationCode { get; set; }
            public string LocationName { get; set; }
            public DateTime? deleted_at { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class GetOneLocationQuery : UserParams, IRequest<PagedList<GetOneLocationResult>>
        {
            public string Search { get; set; }

            public bool? Status { get; set; }
        }


        public class Handler : IRequestHandler<GetOneLocationQuery, PagedList<GetOneLocationResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOneLocationResult>> Handle(GetOneLocationQuery request, CancellationToken cancellationToken)
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

                var result = oneChargingList.GroupBy(x => x.location_id).Select(x => new GetOneLocationResult
                {
                    LocationId = x.Key,
                    LocationCode = x.First().location_code,
                    LocationName = x.First().location_name,
                    deleted_at = x.First().deleted_at,
                    IsActive = x.First().IsActive,
                    UpdatedAt = x.First().UpdatedAt,
                });

                return await PagedList<GetOneLocationResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}
