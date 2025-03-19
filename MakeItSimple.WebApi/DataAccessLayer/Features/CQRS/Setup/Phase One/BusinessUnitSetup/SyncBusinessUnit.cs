using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.BusinessUnitSetup.SyncBusinessUnit.SyncBusinessUnitCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.BusinessUnitSetup
{
    public class SyncBusinessUnit
    {
        public class SyncBusinessUnitCommand : IRequest<Result>
        {
            public Guid Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime? SyncDate { get; set; }
            public ICollection<BusinessUnitResultCommand> Result { get; set; }

            public class BusinessUnitResultCommand
            {
                public int Id { get; set; }
                public string Code { get; set; }
                public string Name { get; set; }
                public CompanyInfo Company { get; set; }
                public DateTime Updated_at { get; set; }
            }

            public class CompanyInfo
            {
                public string Code { get; set; }
                public string Name { get; set; }

            }
        }

        public class Handler : IRequestHandler<SyncBusinessUnitCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context) => _context = context;

            public async Task<Result> Handle(SyncBusinessUnitCommand command, CancellationToken cancellationToken)
            {
                if (command.Result == null || !command.Result.Any())
                {
                    return Result.Warning("Invalid Payload");
                }

                var existingCompanies = await _context.Companies
                       .Where(c => command.Result.Select(b => b.Company.Code).Contains(c.CompanyCode))
                       .ToDictionaryAsync(c => c.CompanyCode, cancellationToken);

                var missingCompanies = command.Result
                        .Where(b => !existingCompanies.ContainsKey(b.Company.Code))
                        .Select(c => new { c.Company.Code, c.Company.Name })
                        .ToList();

                if (missingCompanies.Any())
                {
                    return Result.NotExist(missingCompanies.Select(c => new { code = c.Code , name = c.Name}).ToList());
                }

                var existingBusiness = await _context.BusinessUnits
                    .Where(c => command.Result.Select(b => b.Code ).Contains(c.BusinessCode)
                    && command.Result.Select(b => b.Company.Code).Contains(c.CompanyCode)
                    )
                    .ToDictionaryAsync(c => (c.BusinessCode, c.CompanyCode), cancellationToken);

                var businessUnits = command.Result
                   .Where(b => !existingBusiness.TryGetValue((b.Code, b.Company.Code), out var existingBU) ||
                                existingBU.Business_No != b.Id ||
                                existingBU.BusinessName != b.Name )
                    .Select(b =>
                    {
                        if (existingBusiness.TryGetValue((b.Code, b.Company.Code), out var existingBU))
                        {
                            existingBU.CompanyCode = b.Company.Code;
                            existingBU.CompanyId = existingCompanies[b.Company.Code].Id;
                            existingBU.Business_No = b.Id;
                            existingBU.BusinessName = b.Name;
                            existingBU.UpdatedAt = b.Updated_at;
                            existingBU.ModifiedBy = command.Modified_By;
                            existingBU.SyncStatus = "Updated";
                            existingBU.SyncDate = DateTime.Now;
                            _context.Entry(existingBU).State = EntityState.Modified;
                            return null;
                        }
                        return new BusinessUnit
                        {
                            Business_No = b.Id,
                            BusinessCode = b.Code,
                            BusinessName = b.Name,
                            CompanyCode = existingCompanies[b.Company.Code].CompanyCode,
                            CompanyId = existingCompanies[b.Company.Code].Id,
                            CreatedAt = b.Updated_at,
                            AddedBy = command.Added_By,
                            SyncDate = DateTime.Now,
                            SyncStatus = "New Added"
                        };
                    }).Where(b => b != null).ToList();

                if (businessUnits.Any())
                    await _context.BusinessUnits.AddRangeAsync(businessUnits!, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Successfully synced data");
            }
        }
    }
}