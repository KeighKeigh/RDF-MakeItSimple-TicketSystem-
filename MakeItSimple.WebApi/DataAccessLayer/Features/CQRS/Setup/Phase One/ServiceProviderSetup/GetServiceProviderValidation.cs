using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ServiceProviderSetup
{
    public class GetServiceProviderValidation
    {
        public class GetServiceProviderValidationCommand : IRequest<Result>
        {
            public string ServiceProviderName { get; set; }
        }


        public class Handler : IRequestHandler<GetServiceProviderValidationCommand, Result>
        {

            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetServiceProviderValidationCommand command, CancellationToken cancellationToken)
            {
                var channels = await _context.ServiceProviders.FirstOrDefaultAsync(x => x.ServiceProviderName == command.ServiceProviderName);
                if (channels != null)
                {
                    return Result.Failure(ServiceError.ServiceProviderNameAlreadyExist(command.ServiceProviderName));
                }

                return Result.Success();

            }
        }
    }
}
