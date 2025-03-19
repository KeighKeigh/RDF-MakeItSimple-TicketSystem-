using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.Pivot;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.SubUnitSetup
{
    public class UpsertSubUnit
    {
        public class UpsertSubUnitCommand : IRequest<Result>
        {
            public string? UnitCode { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public string Location_Code { get; set; }

        }

        public class Handler : IRequestHandler<UpsertSubUnitCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertSubUnitCommand command, CancellationToken cancellationToken)
            {
               
                
                var existingUnit = await _context.Units.FirstOrDefaultAsync(x => x.UnitCode == command.UnitCode, cancellationToken);
                if (existingUnit == null)
                {
                    return Result.Failure(SubUnitError.UnitNotExist());
                }

                var existingLocation = await _context.Locations.FirstOrDefaultAsync(x => x.LocationCode == command.Location_Code, cancellationToken);
                if (existingLocation == null)
                {
                    return Result.Failure(SubUnitError.LocationNotExist());
                }

                var existingSubUnit = await _context.SubUnits.FirstOrDefaultAsync(x => x.SubUnitCode == command.SubUnit_Code, cancellationToken);


                if (existingSubUnit != null)
                {
                    return Result.Failure(SubUnitError.SubUnitCodeAlreadyExist(command.SubUnit_Code));
                }
                


                var latestSubUnitId = await _context.SubUnits
                     .OrderByDescending(s => s.Id)
                     .Select(s => s.Id)
                     .FirstOrDefaultAsync();

                var subUnit = new SubUnit
                {
                    UnitId = existingUnit.Id,
                    UnitCode = existingUnit.UnitCode,
                    DepartmentId = existingUnit.DepartmentId,
                    SubUnitCode = command.SubUnit_Code,
                    SubUnitName = command.SubUnit_Name,
                    SubUnitNo = latestSubUnitId +1,
                    AddedBy = command.Added_By,
                    Manual = "Manual"
                };

                await _context.SubUnits.AddAsync(subUnit);
                await _context.SaveChangesAsync(cancellationToken);


                var locationPivot = new SubUnitLocationPivot
                {
                    CreatedAt = DateTime.Now,
                    AddedBy = command.Added_By,
                    SubUnitId = subUnit.Id,
                    LocationId = existingLocation.Id
                };

                await _context.SubUnitLocations.AddAsync(locationPivot);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}