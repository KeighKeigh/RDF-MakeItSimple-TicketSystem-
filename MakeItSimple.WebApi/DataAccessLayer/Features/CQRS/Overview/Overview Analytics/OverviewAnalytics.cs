using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Overview.Overview_Analytics
{
    public partial class OverviewAnalytics
    {

        public class Handler : IRequestHandler<OverviewAnalyticsQuery, Result>
        {
            private readonly MisDbContext context;

            public Handler(MisDbContext context)
            {
                this.context = context;
            }

            public async Task<Result> Handle(OverviewAnalyticsQuery request, CancellationToken cancellationToken)
            {
                var query = await context.TicketConcerns
                    .Include(x => x.User)
                    .ThenInclude(x => x.Department)
                    .Where(x => x.UserId != null && x.IsApprove == true)
                    .ToListAsync();

                var totalTickets = query.Count();
                var totalDelay = query.Count(x => x.IsClosedApprove == true && (x.TargetDate.Value.Date < x.Closed_At || x.TargetDate.Value.Date < DateTime.Now.Date));

                if(request.DateFrom is not null && request.DateTo is not null) 
                    query = query.Where(x => x.CreatedAt.Date <= request.DateFrom.Value.Date && x.CreatedAt >= request.DateTo.Value.Date).ToList();

                var results = query 
                    .GroupBy(x => x.User.DepartmentId)
                    .Select(x => new OverviewAnalyticsResult
                    {
                        Department = x.First().User.Department.DepartmentName,
                        TotalTicket = totalTickets,
                        TotalDelay = totalDelay,
                        OverviewAnalyticsDetails = x.GroupBy(x => x.UserId)
                        .Select(x => new OverviewAnalyticsResult.OverviewAnalyticsDetail
                        {
                            Id = x.Key.Value,
                            FullName = x.First().User.Fullname,
                            NumberOfTicket = x.Count(),
                            PercentageTicket =Math.Round(((decimal)x.Count() / totalTickets) * 100,2),
                            NumberOfDelay = x.Count(x => x.IsClosedApprove == true && (x.TargetDate.Value.Date < x.Closed_At || x.TargetDate.Value.Date < DateTime.Now.Date)),
                            DelayTicketPercentage = Math.Round(((decimal)x.Count(x => x.IsClosedApprove == true && (x.TargetDate.Value.Date < x.Closed_At || x.TargetDate.Value.Date < DateTime.Now.Date)) / totalDelay) * 100,2),

                        }).OrderByDescending(x => x.PercentageTicket).ToList(),
                    });

                return Result.Success(results);
            }
        }


    }
}
