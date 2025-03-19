using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.DepartmentSetup
{
    public class SyncDepartment
    {
        public class SyncDepartmentCommand : IRequest<Result>
        {
            public Guid Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime? SyncDate { get; set; }
            public ICollection<DepartmentResultCommand> Result { get; set; }

            public class DepartmentResultCommand
            {
                public int Id { get; set; }
                public string Code { get; set; }
                public string Name { get; set; }
                public BusinessUnitInfo Business_unit { get; set; }
                public DateTime? Updated_at { get; set; }
            }

            public class BusinessUnitInfo
            {
                public string Code { get; set; }
                public string Name { get; set; }

            }
        }

        public class Handler : IRequestHandler<SyncDepartmentCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context) => _context = context;

            public async Task<Result> Handle(SyncDepartmentCommand command, CancellationToken cancellationToken)
            {

                if (command.Result == null || !command.Result.Any())
                {
                    return Result.Warning("Invalid Payload");
                }

                var existingBusinessUnits = await _context.BusinessUnits
                    .Where(bu => command.Result.Select(b => b.Business_unit.Code).Contains(bu.BusinessCode))
                    .ToDictionaryAsync(bu => bu.BusinessCode, cancellationToken);

                var businessUnitNotExist = command.Result
                    .Where(d => !existingBusinessUnits.ContainsKey(d.Business_unit.Code))
                    .Select(bu => new {bu.Business_unit.Code, bu.Business_unit.Name})
                    .ToList();

                if (businessUnitNotExist.Any())
                {
                    return Result.NotExist(businessUnitNotExist.Select(d => new { business_unit = d.Code, name= d.Name }).ToList());
                }
                var existingDepartments = await _context.Departments
                    .Where(dep => command.Result.Select(d => d.Code).Contains(dep.DepartmentCode)
                    && command.Result.Select(b => b.Business_unit.Code).Contains(dep.BusinessUnitCode))
                    .ToDictionaryAsync(dep => ( dep.DepartmentCode, dep.BusinessUnitCode), cancellationToken);

                var departments = command.Result
                    .Where(d => !existingDepartments.TryGetValue((d.Code, d.Business_unit.Code), out var existingDept) ||
                                existingDept.DepartmentNo != d.Id ||
                                existingDept.DepartmentName != d.Name)
                    .Select(d =>
                {
                    if (existingDepartments.TryGetValue((d.Code, d.Business_unit.Code), out var existingDept))
                    {
                        existingDept.BusinessUnitCode = d.Business_unit.Code;
                        existingDept.BusinessUnitId = existingBusinessUnits[d.Business_unit.Code].Id;
                        existingDept.DepartmentNo = d.Id;
                        existingDept.DepartmentName = d.Name;
                        existingDept.UpdatedAt = d.Updated_at ?? DateTime.Now;
                        existingDept.ModifiedBy = command.Modified_By;
                        existingDept.SyncStatus = "Updated";
                        existingDept.SyncDate = DateTime.Now;
                        _context.Entry(existingDept).State = EntityState.Modified;
                        return null;
                    }
                    return new Department
                    {
                        DepartmentNo = d.Id,
                        DepartmentCode = d.Code,
                        DepartmentName = d.Name,
                        BusinessUnitCode = existingBusinessUnits[d.Business_unit.Code].BusinessCode,
                        BusinessUnitId = existingBusinessUnits[d.Business_unit.Code].Id,
                        CreatedAt = d.Updated_at ?? DateTime.Now,
                        AddedBy = command.Added_By,
                        SyncDate = DateTime.Now,
                        SyncStatus = "New Added"
                    };
                }).Where(d => d != null).ToList();

                if (departments.Any())
                    await _context.Departments.AddRangeAsync(departments!, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Successfully synced data");
            }
        }
    }
}
