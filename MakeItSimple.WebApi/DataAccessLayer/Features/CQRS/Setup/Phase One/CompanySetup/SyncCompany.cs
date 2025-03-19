using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.CompanySetup.SyncCompany.SyncCompanyCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.CompanySetup
{
    public class SyncCompany
    {
        public class SyncCompanyCommand : IRequest<Result>
        {
            public Guid Modified_By { get; set; }
            public Guid? Added_By { get; set; }
            public DateTime? SyncDate { get; set; }
            public ICollection<CompanyResultCommand> Result { get; set; }

            public class CompanyResultCommand
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public string Code { get; set; }
                public DateTime Updated_at { get; set; }
                public DateTime? Deleted_at { get; set; }
            }
        }

        public class Handler : IRequestHandler<SyncCompanyCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context) => _context = context;

            public async Task<Result> Handle(SyncCompanyCommand command, CancellationToken cancellationToken)
            {

                if (command.Result == null || !command.Result.Any())
                {
                    return Result.Warning("Invalid payload");
                }

                var companyCodes = command.Result.Select(c => c.Code).ToList();
                var existingCompanies = await _context.Companies
                    .Where(c => companyCodes.Contains(c.CompanyCode))
                    .ToDictionaryAsync(c => c.CompanyCode, cancellationToken);

                var companies = command.Result.Select(c =>
                {
                    if (existingCompanies.TryGetValue(c.Code, out var existingCompany))
                    {
                        existingCompany.CompanyNo = c.Id;
                        existingCompany.CompanyName = c.Name;
                        existingCompany.UpdatedAt = c.Updated_at;
                        existingCompany.ModifiedBy = command.Modified_By;
                        existingCompany.SyncStatus = "Updated";
                        existingCompany.SyncDate = DateTime.Now;
                        _context.Entry(existingCompany).State = EntityState.Modified;
                        return null;
                    }
                    return new Company
                    {
                        CompanyNo = c.Id,
                        CompanyCode = c.Code,
                        CompanyName = c.Name,
                        CreatedAt = c.Updated_at,
                        AddedBy = command.Added_By,
                        SyncDate = DateTime.Now,
                        SyncStatus = "New Added"
                    };
                }).Where(c => c != null).ToList();


                if (companies.Any())
                    await _context.Companies.AddRangeAsync(companies!, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Successfully synced data");
            }
        }
    }
}
