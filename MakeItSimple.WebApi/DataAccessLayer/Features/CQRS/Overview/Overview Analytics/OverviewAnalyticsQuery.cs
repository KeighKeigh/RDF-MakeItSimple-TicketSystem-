using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Overview.Overview_Analytics
{
    public partial class OverviewAnalytics
    {
        public sealed class OverviewAnalyticsQuery : IRequest<Result>
        {
            public Guid? UserId { get; set; }
            public string UserRole { get; set; }
            public DateTime? DateFrom { get; set; }
            public DateTime? DateTo { get; set; }

        }


    }
}
