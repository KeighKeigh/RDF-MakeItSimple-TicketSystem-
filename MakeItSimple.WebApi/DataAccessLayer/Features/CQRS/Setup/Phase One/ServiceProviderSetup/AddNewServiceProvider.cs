//using MakeItSimple.WebApi.Common;
//using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
//using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
//using MakeItSimple.WebApi.Models.Setup.Phase_One.ServiceProviderSetup;
//using MediatR;
//using Microsoft.EntityFrameworkCore;

//namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ServiceProviderSetup
//{
//    public class AddNewServiceProvider
//    {
//        public class AddNewServiceProviderCommands : IRequest<Result>
//        {
//            public int? serviceProviderId { get; set; }
//            public string serviceProviderName { get; set;}
//            public Guid? addedBy { get; set; }
//            public Guid? modifiedBy { get; set; }
            
//            public List<ChannelList> ChannelLists { get; set; }

//            public class ChannelList
//            {
//                public int? servisChannelId { get; set; }
//                public int? channelId { get; set; }
//            }
//        }

//        public class Handler : IRequestHandler<AddNewServiceProviderCommands, Result>
//        {
//            private readonly MisDbContext _context;

//            public Handler(MisDbContext context)
//            {
//                _context = context;
//            }


//            public async Task<Result> Handle(AddNewServiceProviderCommands command, CancellationToken cancellationToken)
//            {

//                var listDelete = new List<ServiceProviderChannel>();
//                var listServiceProvider = new List<ServiceProviders>();

//                var serviceId = await _context.ServiceProviders.FirstOrDefaultAsync(x => x.Id == command.serviceProviderId, cancellationToken);

//                if(serviceId != null)
//                {
//                    var servicenameAlreadyExist = await _context.ServiceProviders.FirstOrDefaultAsync(x => x.ServiceProviderName == command.serviceProviderName, cancellationToken);

//                    if (servicenameAlreadyExist != null && servicenameAlreadyExist.ServiceProviderName != serviceId.ServiceProviderName)
//                    {
//                        return Result.Failure(ServiceError.ServiceProviderNameAlreadyExist(command.serviceProviderName));
//                    }

//                    bool hasChange = false;

//                    if(serviceId.ServiceProviderName != command.serviceProviderName)
//                    {
//                        serviceId.ServiceProviderName = command.serviceProviderName;
//                        hasChange = true;
//                    }

//                    if(hasChange)
//                    {
//                        serviceId.ModifiedBy = command.modifiedBy;
//                        serviceId.UpdatedAt = DateTime.Now;
//                    }

//                    listServiceProvider.Add(serviceId);
//                    await _context.SaveChangesAsync();
//                }

//                else
//                {
//                    var servicenameAlreadyExist = await _context.ServiceProviders.FirstOrDefaultAsync(x => x.ServiceProviderName == command.serviceProviderName, cancellationToken);

//                    if (servicenameAlreadyExist != null)
//                    {
//                        return Result.Failure(ServiceError.ServiceProviderNameAlreadyExist(command.serviceProviderName));
//                    }

//                    var services = new ServiceProviders
//                    {
//                        ServiceProviderName = command.serviceProviderName,
//                        AddedBy = command.addedBy,
//                    };

//                    await _context.ServiceProviders.AddAsync(services, cancellationToken);
//                    await _context.SaveChangesAsync(cancellationToken);

//                    listServiceProvider.Add(services);
//                }

//                foreach (var service in command.ChannelLists) 
//                {
//                    var channelNoExist = await _context.Channels.FirstOrDefaultAsync(x => x.Id == service.channelId, cancellationToken);

//                    if(channelNoExist == null)
//                    {
//                        return Result.Failure(ServiceError.ChannelNotExist());
//                    }

//                    var serviceName = await _context.ServiceProviders.FirstOrDefaultAsync(x => x.ServiceProviderName == command.serviceProviderName || x.Id == command.serviceProviderId, cancellationToken);
//                    var serviceChannel = await _context.ServiceProviderChannels.Include(x => x.Channel).FirstOrDefaultAsync(x => x.ChannelId == service.channelId && x.ServiceProviderId == serviceName.Id);

//                    i/*f()*/
//                }
//            }
//        }
//    }
//}
