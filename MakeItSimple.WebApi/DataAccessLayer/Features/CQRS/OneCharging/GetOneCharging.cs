using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.OneCharging;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging
{
    public class GetOneCharging
    {

        public class GetOneChargingResult
        {
            public int? sync_id { get; set; }
            public string code { get; set; }
            public string name { get; set; }
            public int? CompanyId { get; set; }
            public string CompanyCode { get; set; }
            public string CompanyName { get; set; }
            public int? BusinessUnitId { get; set; }
            public string BusinessUnitCode { get; set; }
            public string businessUnitName { get; set; }
            public int? DepartmentId { get; set; }
            public string DepartmentCode { get; set; }
            public string DeparmentName { get; set; }
            public int? UnitId { get; set; }
            public string UnitCode { get; set; }

            public string UnitName { get; set; }
            public int? SubUnitId { get; set; }
            public string SubUnitCode { get; set; }
            public string SubUnitName { get; set; }
            public int? LocationId { get; set; }
            public string LocationCode { get; set; }
            public string LocationName { get; set; }
            public DateTime? deleted_at { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class GetOneChargingQuery : UserParams, IRequest<PagedList<GetOneChargingResult>>
        {
            public string Search { get; set; }

            public bool? Status { get; set; }
        }


        public class Handler : IRequestHandler<GetOneChargingQuery, PagedList<GetOneChargingResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOneChargingResult>> Handle(GetOneChargingQuery request, CancellationToken cancellationToken)
            {
                IQueryable<OneChargingMIS> oneChargingList = _context.OneChargings;

                if (!string.IsNullOrEmpty(request.Search))
                {
                    oneChargingList = oneChargingList.Where(x => x.name.ToLower().Contains(request.Search.ToLower())
                    || x.code.ToLower().Contains(request.Search.ToLower()));
                }

                if (request.Status != null)
                {
                    oneChargingList = oneChargingList.Where(x => x.IsActive == request.Status);
                }

                var result = oneChargingList.Select(x => new  GetOneChargingResult
                {
                    sync_id = x.sync_id,
                    code = x.code,
                    name = x.name,
                    CompanyId = x.company_id,
                    CompanyCode = x.company_code,
                    CompanyName = x.company_name,
                    BusinessUnitId = x.business_unit_id,
                    BusinessUnitCode = x.business_unit_code,
                    businessUnitName = x.business_unit_name,
                    DepartmentId = x.department_id,
                    DepartmentCode = x.department_code,
                    DeparmentName = x.department_name,
                    UnitId = x.department_unit_id,
                    UnitCode = x.department_unit_code,
                    UnitName = x.department_unit_name,
                    SubUnitId = x.sub_unit_id,
                    SubUnitCode = x.sub_unit_code,
                    SubUnitName = x.sub_unit_name,
                    LocationId = x.location_id,
                    LocationCode = x.location_code,
                    LocationName = x.location_name,
                    deleted_at = x.deleted_at,
                    IsActive = x.IsActive,
                    UpdatedAt = x.UpdatedAt,
                });

                return await PagedList<GetOneChargingResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}
