using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.Phase_One.ServiceProviderSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.ChannelSetup.GetChannel;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.ChannelSetup.GetChannel.GetChannelResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ServiceProviderSetup
{
    public class GetServiceProvider
    {
        public record GetServiceProviderResult
        {
            public int Id { get; set; }
            public string serviceProviderName { get; set; }
            public int? noOfChannels { get; set; }

            public bool? isActive { get; set; }
            public string addedBy { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }

            public List<ServiceProviderChannel> serviceChannel { get; set; }

            public record ServiceProviderChannel
            {
                public int? serviceChannelId { get; set; }
                public int? serviceProviderId { get; set; }
                public int? ChannelId { get; set; }
                public string ChannelName { get; set; }
                public bool? Is_Active { get; set; }
                


                

            }


        }

        public class GetServiceProviderQuery : UserParams, IRequest<PagedList<GetServiceProviderResult>>
        {
            public string Search { get; set; }
            public bool? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetServiceProviderQuery, PagedList<GetServiceProviderResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetServiceProviderResult>> Handle(GetServiceProviderQuery request, CancellationToken cancellationToken)
            {

                IQueryable<ServiceProviders> serviceQuery = _context.ServiceProviders
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.ServiceProviderChannels)
                    .ThenInclude(x => x.Channel)
                    .ThenInclude(x => x.ChannelUsers)
                    .Include(x => x.ServiceProviderChannels)
                    ;

                //.Include(x => x.ServiceProviderChannels).ThenInclude(x => x.Id)

                if (!string.IsNullOrEmpty(request.Search))
                {
                    serviceQuery = serviceQuery.Where(x => x.ServiceProviderName.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    serviceQuery = serviceQuery.Where(x => x.IsActive == request.Status);
                }


                var results = serviceQuery.Select(x => new GetServiceProviderResult
                {
                    Id = x.Id,
                    serviceProviderName = x.ServiceProviderName,
                    addedBy = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Updated_At = x.UpdatedAt,
                    Modified_By = x.ModifiedByUser.Fullname,
                    noOfChannels = x.ServiceProviderChannels.Count(),
                    isActive = x.IsActive,
                    serviceChannel = x.ServiceProviderChannels.Where(x => x.IsActive == true).Select(x => new GetServiceProviderResult.ServiceProviderChannel
                    {
                        
                        serviceProviderId = x.ServiceProviderId,
                        ChannelId = x.ChannelId,
                        ChannelName = x.Channel.ChannelName,
                        serviceChannelId = x.Id,
                        Is_Active = x.IsActive,
                        
                        


                    }).ToList()

                });

                return await PagedList<GetServiceProviderResult>.CreateAsync(results, request.PageNumber, request.PageSize);

            }
        }
    }
}
