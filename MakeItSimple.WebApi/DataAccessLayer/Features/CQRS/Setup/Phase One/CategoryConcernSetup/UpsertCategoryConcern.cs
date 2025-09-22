using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_One;
using MakeItSimple.WebApi.Models.Setup.Phase_One.CategoryConcernSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.CategoryConcernSetup
{
    public class UpsertCategoryConcern
    {

        public class UpsertCategoryConcernCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string concernCategory { get; set; }
            public DateTime? DateAdded { get; set; }
            public DateTime? DateUpdated { get; set; }

        }

        public class Handler : IRequestHandler<UpsertCategoryConcernCommand, Result>
        {
            private readonly MisDbContext _context;
            

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertCategoryConcernCommand command, CancellationToken cancellationToken)
            {

                var categoryConcernExist = await _context.CategoryConcerns.FirstOrDefaultAsync(x => x.CategoryConcernName == command.concernCategory, cancellationToken);

                if (categoryConcernExist != null)
                {
                    return Result.Failure(CategoryConcernError.CategoryConcernAlreadyExist(command.concernCategory));
                }

                var categoryConcern = await _context.CategoryConcerns.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
                if (categoryConcern != null)
                {
                    if (categoryConcern.CategoryConcernName == command.concernCategory)
                    {
                        return Result.Failure(CategoryConcernError.CategoryConcernNochanges());
                    }

                    categoryConcern.CategoryConcernName = command.concernCategory;
                    categoryConcern.DateUpdated = command.DateUpdated;

                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success();
                }
                else
                {
                    var addCategoryConcern = new CategoryConcern
                    {
                        CategoryConcernName = command.concernCategory,
                        DateAdded = DateTime.Now,
                        IsActive = true
                    };

                    await _context.CategoryConcerns.AddAsync(addCategoryConcern, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success();
                }
            }
        }

        
    }
}
