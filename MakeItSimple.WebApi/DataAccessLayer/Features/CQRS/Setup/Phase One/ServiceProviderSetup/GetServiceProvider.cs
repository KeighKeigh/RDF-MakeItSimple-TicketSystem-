//using MakeItSimple.WebApi.Common.Pagination;
//using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
//using MakeItSimple.WebApi.Models.Setup.Phase_One.ServiceProviderSetup;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using System.Diagnostics;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.ChannelSetup.GetChannel.GetChannelResult;

//namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ServiceProviderSetup
//{
//    public class GetServiceProvider
//    {
//        public record GetServiceProviderResult
//        {
//            public int Id { get; set; }
//            public string serviceProviderName { get; set; }
//            public int? noOfChannels { get; set; }

//            public bool? isActive { get; set; }
//            public string addedBy { get; set; }
//            public DateTime Created_At { get; set; }
//            public string Modified_By { get; set; }
//            public DateTime? Updated_At { get; set; }

//            public List<ServiceProviderChannel> serviceChannel { get; set; }

//            public record ServiceProviderChannel
//            {
//                public int? serviceProviderId { get; set; }
//                public int? ChannelId { get; set; }
//                public string ChannelName { get; set; }
//                public int No_Of_Members { get; set; }
//                public bool Is_Active { get; set; }
//                public string Added_By { get; set; }
//                public DateTime Created_At { get; set; }
//                public string Modified_By { get; set; }
//                public DateTime? Updated_At { get; set; }

//                public List<ChannelUser> channelUsers { get; set; }

//                public record ChannelUser
//                {
//                    public int? ChannelId { get; set; }
//                    public int? DepartmentId { get; set; }
//                    public string Department_Code { get; set; }
//                    public string Department_Name { get; set; }
//                    public int ChannelUserId { get; set; }
//                    public Guid? UserId { get; set; }
//                    public string Fullname { get; set; }
//                    public string UserRole { get; set; }
//                }
//                public List<CategoriesList> Categories { get; set; }

//                public class CategoriesList
//                {
//                    public int Id { get; set; }
//                    public string Category_Description { get; set; }

//                }

//            }


//        }

//        public class GetServiceProviderQuery : UserParams, IRequest<PagedList<GetServiceProviderResult>>
//        {
//            public string Search { get; set; }
//            public bool? Status { get; set; }
//        }

//        public class Handler  : IRequestHandler<GetServiceProviderQuery, PagedList<GetServiceProviderResult>>
//        {
//            private readonly MisDbContext _context;

//            public Handler(MisDbContext context)
//            {
//                _context = context;
//            }

//            public async Task<PagedList<GetServiceProviderResult>> Handle(GetServiceProviderQuery request, CancellationToken cancellationToken)
//            {
//                IQueryable<ServiceProvider> serviceQuery = _context.ServiceProviders
//                    .AsNoTracking()
//                    .Include(x => x.AddedByUser)
//                    .Include(x => x.ModifiedByUser)
//                    .Include(x => x.ServiceProviderChannels)
//                    .ThenInclude(x => x.Channel)
//                    .ThenInclude(x => x.ChannelUsers)
//                    .Include(x => x.ServiceProviderChannels)
//                    .ThenInclude(x => x.Channel)
//                    .ThenInclude(x => x.User)
//                    .ThenInclude(x => x.Department).AsQueryable();



//            }
//        }
//    }
//}
