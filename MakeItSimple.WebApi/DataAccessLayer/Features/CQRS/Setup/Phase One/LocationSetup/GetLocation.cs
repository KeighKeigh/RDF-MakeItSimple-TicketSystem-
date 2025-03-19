using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.Pivot;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.CompanySetup.GetCompany;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.LocationSetup
{
    public class GetLocation
    {
        public class GetLocationResult
        {
            public int Location_No { get; set; }
            public string Location_Code { get; set; }
            public string Location_Name { get; set; }
            //public string SubUnit_Code { get; set; }
            //public string SubUnit_Name { get; set; }    
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public DateTime? SyncDate { get; set; }
            public string Sync_Status { get; set; }

            public ICollection<SubUnit> SubUnits { get; set; }
            public class SubUnit
            {
                public int? LocationId { get; set; }
                public int? SubUnitId { get; set; }
                public string SubUnit_Code { get; set; }
                public string SubUnit_Name { get; set; }
            }
        }

        public class GetLocationQuery : UserParams, IRequest<PagedList<GetLocationResult>>
        {
            public string Search { get; set; }

            public bool? Status { get; set; }
        }

        public class IHandler : IRequestHandler<GetLocationQuery, PagedList<GetLocationResult>>
        {
            private readonly MisDbContext _context;

            public IHandler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetLocationResult>> Handle(GetLocationQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Location> locationQuery = _context.Locations.Include(x => x.AddedByUser).Include(x => x.ModifiedByUser)/*.Include(x => x.SubUnits)*/;

                if (!string.IsNullOrEmpty(request.Search))
                {
                    locationQuery = locationQuery.Where(x => x.LocationCode.Contains(request.Search) || x.LocationName.Contains(request.Search));

                }

                if (request.Status != null)
                {
                    locationQuery = locationQuery.Where(x => x.IsActive == request.Status);
                }

                var results = locationQuery
                            .Join(_context.SubUnitLocations,
                                  loc => loc.Id,
                                  pivot => pivot.LocationId,
                                  (loc, pivot) => new { loc, pivot })
                            .Join(_context.SubUnits,
                                  lp => lp.pivot.SubUnitId,
                                  sub => sub.Id,
                                  (lp, sub) => new { lp.loc, lp.pivot, sub })
                            .GroupBy(x => x.loc.LocationCode)
                            .Select(g => new GetLocationResult
                            {
                                Location_No = g.First().loc.LocationNo,
                                Location_Code = g.First().loc.LocationCode,
                                Location_Name = g.First().loc.LocationName,
                                Added_By = g.First().loc.AddedByUser.Fullname,
                                Created_At = g.First().loc.CreatedAt,
                                Modified_By = g.First().loc.ModifiedByUser.Fullname,
                                Updated_At = g.First().loc.UpdatedAt,
                                Sync_Status = g.First().loc.SyncStatus,
                                SyncDate = g.First().loc.SyncDate,
                                SubUnits = g.Select(x => new GetLocationResult.SubUnit
                                {
                                    LocationId = x.loc.Id,
                                    SubUnitId = x.pivot.SubUnitId,
                                    SubUnit_Code = x.sub.SubUnitCode,
                                    SubUnit_Name = x.sub.SubUnitName,
                                }).ToList()
                            });


                return await PagedList<GetLocationResult>.CreateAsync(results, request.PageNumber, request.PageSize);

            }
        }
    }   
}
