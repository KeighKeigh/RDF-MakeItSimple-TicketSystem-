using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using UnitModel = MakeItSimple.WebApi.Models.Setup.UnitSetup.Unit; 
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.UnitSetup
{
    public class SyncUnit
    {
        public class SyncUnitCommand : IRequest<Result>
        {
            public Guid Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime? SyncDate { get; set; }
            public ICollection<UnitResultCommand> Result { get; set; }

            public class UnitResultCommand
            {
                public int Id { get; set; }
                public string Code { get; set; }
                public string Name { get; set; }
                public DepartmentInfo Department { get; set; }
                public ICollection<SubUnitsInfo> Sub_unit { get; set;}
                public DateTime? Updated_at { get; set; }
            }

            public class DepartmentInfo
            {
                public string Code { get; set; }
                public string Name { get; set; }
                public int Id { get; set; }

            }
            public class SubUnitsInfo
            {
                public string Code { get; set; }
                public string Name { get; set; }
                public int Id { get; set; }

            }

        }

        public class Handler : IRequestHandler<SyncUnitCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context) => _context = context;

            public async Task<Result> Handle(SyncUnitCommand command, CancellationToken cancellationToken)
            {
                if (command.Result == null || !command.Result.Any())
                {
                    return Result.Warning("Invalid Payload");
                }

                var existingDepartments = await _context.Departments
                    .Where(dep => command.Result.Select(d => d.Department.Code).Contains(dep.DepartmentCode))
                    .ToDictionaryAsync(dep => (dep.DepartmentCode, dep.DepartmentNo), cancellationToken);

                var departmentNotExist = command.Result
                    .Where(d => !existingDepartments.ContainsKey((d.Department.Code, d.Department.Id)))
                    .Select(dep => new {dep.Department.Code, dep.Department.Id, dep.Department.Name})
                    .ToList();

                if (departmentNotExist.Any())
                {
                    return Result.NotExist(departmentNotExist.Select(d => new { Code = d.Code, Name=d.Name }).ToList());
                }

                var existingUnits = await _context.Units
                    .Where(unit => command.Result.Select(u => u.Code).Contains(unit.UnitCode)
                    && command.Result.Select(d => d.Department.Code).Contains(unit.DepartmentCode)
                    && command.Result.Select(n => n.Id).Contains(unit.UnitNo)
                    )
                    .ToDictionaryAsync(unit => (unit.UnitNo, unit.UnitCode, unit.DepartmentCode), cancellationToken);

                var units = command.Result
                    .Where(d => !existingUnits.TryGetValue((d.Id, d.Code, d.Department.Code), out var existingUnit) ||
                       existingUnit.UnitNo != d.Id ||
                       existingUnit.UnitName != d.Name)
                    .Select(d =>
                {
                    if (existingUnits.TryGetValue((d.Id, d.Code, d.Department.Code), out var existingUnit))
                    {
                        existingUnit.DepartmentCode = existingDepartments[(d.Department.Code, d.Department.Id)].DepartmentCode;
                        existingUnit.DepartmentId = existingDepartments[(d.Department.Code, d.Department.Id)].Id;
                        existingUnit.UnitNo = d.Id;
                        existingUnit.UnitCode = d.Code;
                        existingUnit.UnitName = d.Name;
                        existingUnit.UpdatedAt = d.Updated_at ?? DateTime.Now;
                        existingUnit.ModifiedBy = command.Modified_By;
                        existingUnit.SyncStatus = "Updated";
                        existingUnit.SyncDate = DateTime.Now;
                        _context.Entry(existingUnit).State = EntityState.Modified;
                        return null;
                    }
                    return new UnitModel
                    {

                        UnitNo = d.Id,
                        UnitCode = d.Code,
                        UnitName = d.Name,
                        DepartmentCode = existingDepartments[(d.Department.Code, d.Department.Id)].DepartmentCode,
                        DepartmentId = existingDepartments[(d.Department.Code, d.Department.Id)].Id,
                        CreatedAt = d.Updated_at ?? DateTime.Now,
                        AddedBy = command.Added_By,
                        SyncDate = DateTime.Now,
                        SyncStatus = "New Added"
                    };
                }).Where(u => u != null).ToList();

                if (units.Any())
                    await _context.Units.AddRangeAsync(units!, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Successfully synced data");
            }
        }
    }
}
