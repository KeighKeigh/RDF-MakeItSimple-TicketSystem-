using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.BusinessUnitSetup.SyncBusinessUnit;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.SubUnitSetup.SyncSubUnit.SyncSubUnitCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.SubUnitSetup
{
    public class SyncSubUnit
    {
        public class SyncSubUnitCommand : IRequest<Result>
        {
            public ICollection<SyncSubUnitsResult> Result { get; set; }

            public Guid Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime? SyncDate { get; set; }


            public class SyncSubUnitsResult
            {
                public int? Id { get; set; }
                public string Code { get; set; }
                public string Name { get; set; }
                public UnitDepartmentInfo Department_unit { get; set; }
                public string Sync_Status { get; set; }
                public DateTime? Updated_at { get; set; }

            }

            public class UnitDepartmentInfo
            {
                public string Code { get; set; }
                public string Name { get; set; }
                public int Id { get; set; }

            }
        }

        public class Handler : IRequestHandler<SyncSubUnitCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(SyncSubUnitCommand command, CancellationToken cancellationToken)
            {

                if (command.Result == null || !command.Result.Any())
                {
                    return Result.Warning("Invalid Payload");
                }

                var existingUnits = await _context.Units
                    .Where(u => command.Result.Select(un => un.Department_unit.Code).Contains(u.UnitCode))
                    .ToDictionaryAsync(un => (un.UnitCode, un.UnitNo), cancellationToken);

                var unitNotExist = command.Result
                  .Where(u => !existingUnits.ContainsKey((u.Department_unit.Code, u.Department_unit.Id)))
                  .Select(un => new {un.Department_unit.Code, un.Department_unit.Id, un.Department_unit.Name})
                  .ToList();

                if (unitNotExist.Any())
                {
                    return Result.NotExist(unitNotExist.Select(d => new { code = d.Code, id = d.Id, name = d.Name }).ToList());
                }

                var existingSub_Units = await _context.SubUnits
                   .Where(unit => command.Result.Select(su => su.Code).Contains(unit.SubUnitCode)
                   && command.Result.Select(sb => sb.Department_unit.Code).Contains(unit.UnitCode))
                   .ToDictionaryAsync(unit => (unit.SubUnitNo, unit.SubUnitCode, unit.UnitCode), cancellationToken);

                var sub_unit = command.Result
                    .Where(d => !existingSub_Units.TryGetValue((d.Id, d.Code, d.Department_unit.Code), out var existingSub_Unit) ||
                        existingSub_Unit.SubUnitNo != d.Id || 
                        existingSub_Unit.SubUnitName != d.Name)
                    .Select(d =>
                {
                    if (existingSub_Units.TryGetValue((d.Id, d.Code, d.Department_unit.Code), out var existingSub_Unit))
                    {
                        existingSub_Unit.UnitId = d.Department_unit.Id;
                        existingSub_Unit.UnitCode = d.Department_unit.Code;
                        existingSub_Unit.DepartmentId = existingUnits[(d.Department_unit.Code, d.Department_unit.Id)].DepartmentId;
                        existingSub_Unit.SubUnitNo = d.Id;
                        existingSub_Unit.SubUnitCode = d.Code;
                        existingSub_Unit.SubUnitName = d.Name;
                        existingSub_Unit.UpdatedAt = d.Updated_at ?? DateTime.Now;
                        existingSub_Unit.ModifiedBy = command.Modified_By;
                        existingSub_Unit.SyncStatus = "Updated";
                        existingSub_Unit.SyncDate = DateTime.Now;
                        _context.Entry(existingSub_Unit).State = EntityState.Modified;
                        return null;
                    }
                    return new SubUnit
                    {
                        DepartmentId = existingUnits[(d.Department_unit.Code, d.Department_unit.Id)].DepartmentId,
                        UnitId = existingUnits[(d.Department_unit.Code, d.Department_unit.Id)].Id,
                        SubUnitNo = d.Id,
                        SubUnitCode = d.Code,
                        SubUnitName = d.Name,
                        UnitCode = d.Department_unit.Code,
                        CreatedAt = d.Updated_at ?? DateTime.Now,
                        AddedBy = command.Added_By,
                        SyncDate = DateTime.Now,
                        SyncStatus = "New Added"
                    };
                }).Where(u => u != null).Distinct().ToList();

                if (sub_unit.Any())
                    await _context.SubUnits.AddRangeAsync(sub_unit!, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Successfully synced data");

            }
        }

    }
}
