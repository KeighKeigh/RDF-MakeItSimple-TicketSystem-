using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_One;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.CategoryConcernSetup
{
    public class UpdateCategoryConcernStatus
    {
        public class UpdateCategoryConcernStatusResult
        {
            public int Id { get; set; }
            public bool? Is_Active { get; set; }

        }

        public class UpdateCategoryConcernStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<UpdateCategoryConcernStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateCategoryConcernStatusCommand command, CancellationToken cancellationToken)
            {
                var categoryConcern = await _context.CategoryConcerns.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
                if (categoryConcern == null)
                {
                    return Result.Failure(CategoryConcernError.CategoryConcernNotExist());
                }

                categoryConcern.IsActive = !categoryConcern.IsActive;
                categoryConcern.DateUpdated = DateTime.Now;
                await _context.SaveChangesAsync(cancellationToken);

                var result = new UpdateCategoryConcernStatusResult
                {
                    Id = categoryConcern.Id,
                    Is_Active = categoryConcern.IsActive,
                };

                return Result.Success(result);
            }
        }
        
    }
}
