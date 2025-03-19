using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.Pivot;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.LocationSetup.SyncLocation.SyncLocationCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.LocationSetup
{
    public class SyncLocation
    {
        public class SyncLocationCommand : IRequest<Result>
        {
            public ICollection<LocationResultCommand> Result { get; set; }

            public Guid Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime? SyncDate { get; set; }
            public class LocationResultCommand
            {

                public int Id { get; set; }
                public string Code { get; set; }
                public string Name { get; set; }
                public string Sync_Status { get; set; }
                public DateTime Created_At { get; set; }
                public DateTime? Updated_at { get; set; }

                public List<SubUnitInfo> Sub_units { get; set; } = new List<SubUnitInfo>();
            }

            public class SubUnitInfo
            {
                public int Id { get; set; }
                public string Code { get; set; }
                public string Name { get; set; }
                public DateTime? Updated_at { get; set; }
            }

        }


        public class Handler : IRequestHandler<SyncLocationCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(SyncLocationCommand command, CancellationToken cancellationToken)
            {
                if (command.Result == null || !command.Result.Any())
                {
                    return Result.Warning("Invalid Payload");
                }
                var subUnitCodes = command.Result
                     .SelectMany(d => d.Sub_units)
                     .Select(su => su.Code)
                     .Distinct()
                     .ToList();

                var existingSubUnits = await _context.SubUnits
                    .Where(su => subUnitCodes.Contains(su.SubUnitCode))
                    .ToDictionaryAsync(su => (su.SubUnitCode, su.SubUnitNo), cancellationToken);


                var subUnitsNotExist = command.Result
                    .SelectMany(d => d.Sub_units)
                    .Where(su => !existingSubUnits.ContainsKey((su.Code, su.Id)))
                    .ToList();



                if (subUnitsNotExist.Any())
                {
                    return Result.NotExist(subUnitsNotExist.Select(d => new { Sub_unit_id = d.Id,Sub_unit = d.Code, Name = d.Name }).ToList());
                }


                var existingLocation = await _context.Locations
                   .Where(loc => command.Result.Select(l => l.Code).Contains(loc.LocationCode))
                   .ToDictionaryAsync(loc => loc.LocationCode, cancellationToken);

                var location = command.Result
                     .Where(l => !existingLocation.TryGetValue(l.Code, out var existingSub_Unit) ||
                        existingSub_Unit.LocationName != l.Name)
                    .Select(d => 
                {
                    if (existingLocation.TryGetValue(d.Code, out var existingLoc))
                    {
                    
                        existingLoc.LocationNo = d.Id;
                        existingLoc.SubUnitId = d.Id;
                        existingLoc.LocationCode = d.Code;
                        existingLoc.LocationName = d.Name;
                        existingLoc.UpdatedAt = d.Updated_at ?? DateTime.Now;
                        existingLoc.ModifiedBy = command.Modified_By;
                        existingLoc.SyncStatus = "Updated";
                        existingLoc.SyncDate = DateTime.Now;
                        _context.Entry(existingLoc).State = EntityState.Modified;
                        return null;
                    }
                    return new Location
                    {
                        LocationNo = d.Id,
                        LocationCode = d.Code,
                        LocationName = d.Name,
                        CreatedAt = d.Updated_at ?? DateTime.Now,
                        AddedBy = command.Added_By,
                        SyncDate = DateTime.Now,
                        SyncStatus = "New Added"
                    };
                }).Where(u => u != null).ToList();


                if (location.Any())
                    await _context.Locations.AddRangeAsync(location!, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);


                var existingNewLocation = await _context.Locations
                 .Where(loc => command.Result.Select(l => l.Code).Contains(loc.LocationCode))
                 .ToDictionaryAsync(loc => loc.LocationCode, cancellationToken);

                var newPivots = command.Result
                      .SelectMany(loc => loc.Sub_units
                          .Select(sub => new SubUnitLocationPivot
                          {
                              LocationId = existingNewLocation[loc.Code].Id,
                              SubUnitId = existingSubUnits[(sub.Code, sub.Id)].Id,
                              CreatedAt = sub.Updated_at ?? DateTime.Now
                          }))
                      .ToList();

                var existingPivots = await _context.SubUnitLocations
                    .Where(p => newPivots.Select(np => np.LocationId).Contains(p.LocationId) &&
                    newPivots.Select(np => np.SubUnitId).Contains(p.SubUnitId))
                   .ToDictionaryAsync(pv => (pv.SubUnitId, pv.LocationId), cancellationToken);

                var pivot = newPivots
                    .Where(np => !existingPivots.TryGetValue((np.SubUnitId, np.LocationId), out var existingPiv))
                    .Select(p =>
                    {
                        if(existingPivots.TryGetValue((p.SubUnitId, p.LocationId), out var existingPv))
                        {
                            existingPv.LocationId = p.LocationId;
                            existingPv.SubUnitId = p.SubUnitId;
                            existingPv.UpdatedAt = DateTime.Now;
                            existingPv.ModifiedBy = command.Modified_By;
                            _context.Entry(existingPv).State = EntityState.Modified;
                            return null;
                        }
                        return new SubUnitLocationPivot
                        {
                            LocationId = p.LocationId,
                            SubUnitId = p.SubUnitId,
                            AddedBy = command.Added_By,
                            CreatedAt = DateTime.Now
                        };
                    }).Where(u => u != null).Distinct().ToList();

                if (pivot.Any())
                    await _context.SubUnitLocations.AddRangeAsync(pivot!, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Successfully synced data");

            }
        }

    }
}
