using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.OneCharging;
using MediatR;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.GetOneBusinessUnit;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging
{
    public class GetOneCompany
    {
        public class GetOneCompanyResult
        {
            public int? CompanyId { get; set; }
            public string CompanyCode { get; set; }
            public string CompanyName { get; set; }
            public DateTime? deleted_at { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class GetOneCompanyQuery : UserParams, IRequest<PagedList<GetOneCompanyResult>>
        {
            public string Search { get; set; }

            public bool? Status { get; set; }
        }


        public class Handler : IRequestHandler<GetOneCompanyQuery, PagedList<GetOneCompanyResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOneCompanyResult>> Handle(GetOneCompanyQuery request, CancellationToken cancellationToken)
            {
                IQueryable<OneChargingMIS> oneChargingList = _context.OneChargings;

                if (!string.IsNullOrEmpty(request.Search))
                {
                    oneChargingList = oneChargingList.Where(x => x.company_code.ToLower().Contains(request.Search.ToLower())
                    || x.company_name.ToLower().Contains(request.Search.ToLower()));
                }

                if (request.Status != null)
                {
                    oneChargingList = oneChargingList.Where(x => x.IsActive == request.Status);
                }

                var result = oneChargingList.GroupBy(x => x.company_id).Select(x => new GetOneCompanyResult
                {
                    CompanyId = x.Key,
                    CompanyCode = x.First().company_code,
                    CompanyName = x.First().company_name,
                    deleted_at = x.First().deleted_at,
                    IsActive = x.First().IsActive,
                    UpdatedAt = x.First().UpdatedAt,
                });

                return await PagedList<GetOneCompanyResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}
