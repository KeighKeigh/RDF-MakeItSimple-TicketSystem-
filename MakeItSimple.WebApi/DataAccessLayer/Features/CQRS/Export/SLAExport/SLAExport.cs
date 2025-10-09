using Azure.Core;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.AllTicketReport.AllTicketReports;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.SLAExport
{
    public class SLAReport
    {

        public class SLAReportQuery : UserParams, IRequest<PagedList<SLAReportResult>>
        {
            public string Search { get; set; }
            public int? Channel { get; set; }
            public int? ServiceProvider { get; set; }
            public Guid? UserId { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }
        }

        public class SLAReportResult
        {
            public string Year { get; set; }
            public string Month { get; set; }
            public int? TicketNo { get; set; }
            public string Assign { get; set; }
            public string Store { get; set; }
            public string Description { get; set; }
            public DateTime? OpenDate { get; set; }
            public DateTime? TargetDate { get; set; }
            public DateTime? ForClosingDate { get; set; }
            public DateTime? ClosedDate { get; set; }
            public string Solution { get; set; }
            public string RequestType { get; set; }
            public string Status { get; set; }
            public string Rating { get; set; }
            
            public string Category { get; set; }
            
            public string SubCategory { get; set; }
            public string Position { get; set; }
            public int? ServiceProviderId { get; set; }
            public int? ChannelId { get; set; }
            public Guid? AssignTo { get; set; }
        }
       

        public class Handler : IRequestHandler<SLAReportQuery, PagedList<SLAReportResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }


            public async Task<PagedList<SLAReportResult>> Handle(SLAReportQuery request, CancellationToken cancellationToken)
            {
                var combineTicketReports = new List<SLAReportResult>();
                var dateToday = DateTime.Now;

                var openTicketQuery = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(t => t.RequestConcern)
                    .AsSplitQuery()
                .Where(t => t.IsApprove == true && t.IsTransfer != true && t.IsClosedApprove != true && t.OnHold != true && t.IsDone != true)
                    .Where(t => t.DateApprovedAt.Value.Date >= request.Date_From.Value.Date && t.DateApprovedAt.Value.Date <= request.Date_To.Value.Date)
                    .Select(o => new SLAReportResult
                    {


                        Year = o.DateApprovedAt.Value.ToString("yyyy"),
                        Month = o.DateApprovedAt.Value.ToString("MMMM"),
                        TicketNo = o.Id,
                        Assign = o.User.Fullname,
                        Store = o.RequestorByUser.Fullname,
                        Description = o.RequestConcern.Concern,
                        OpenDate = o.DateApprovedAt,
                        TargetDate = o.TargetDate,
                        ClosedDate = o.Closed_At,
                        Solution = o.RequestConcern.Resolution,
                        RequestType = o.RequestConcern.RequestType,
                        Status = o.RequestConcern.ConcernStatus,
                        Rating = o.TargetDate.Value.Date >= dateToday.Date ? "On Time" : "Delay",
                        Category = string.Join(", ", o.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        SubCategory = string.Join(", ", o.RequestConcern.TicketSubCategories.Select(rc => rc.SubCategory.SubCategoryDescription)),
                        Position = "",
                        ServiceProviderId = o.RequestConcern.ServiceProviderId,
                        ChannelId = o.RequestConcern.ChannelId,
                        AssignTo = o.AssignTo,




                    }).ToListAsync();


                var closingTicketQuery = await _context.ClosingTickets
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                .AsSplitQuery()
                .Where(x => x.IsClosing == true && x.IsActive == true)
                .Where(t => t.ClosingAt.Value.Date >= request.Date_From.Value.Date && t.ClosingAt.Value.Date <= request.Date_To.Value.Date)
                    .Select(ct => new SLAReportResult
                    {
                        Year = ct.TicketConcern.DateApprovedAt.Value.ToString("yyyy"),
                        Month = ct.TicketConcern.DateApprovedAt.Value.ToString("MMMM"),
                        TicketNo = ct.TicketConcernId,
                        Assign = ct.TicketConcern.User.Fullname,
                        Store = ct.TicketConcern.RequestorByUser.Fullname,
                        Description = ct.TicketConcern.RequestConcern.Concern,
                        OpenDate = ct.TicketConcern.DateApprovedAt,
                        TargetDate = ct.TicketConcern.TargetDate,
                        ClosedDate = ct.TicketConcern.Closed_At,
                        Solution = ct.TicketConcern.RequestConcern.Resolution,
                        RequestType = ct.TicketConcern.RequestConcern.RequestType,
                        Status = ct.TicketConcern.RequestConcern.ConcernStatus,
                        Rating = ct.TicketConcern.TargetDate.Value.Date >= ct.TicketConcern.Closed_At .Value.Date ? "On Time" : "Delay",
                        Category = string.Join(", ", ct.TicketConcern.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        SubCategory = string.Join(", ", ct.TicketConcern.RequestConcern.TicketSubCategories.Select(rc => rc.SubCategory.SubCategoryDescription)),
                        Position = "",
                        ServiceProviderId = ct.TicketConcern.RequestConcern.ServiceProviderId,
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId,
                        AssignTo = ct.TicketConcern.AssignTo,
                        ForClosingDate = ct.ForClosingAt,
                    }).ToListAsync();




                if (request.ServiceProvider is not null)
                {
                    openTicketQuery = openTicketQuery
                           .Where(x => x.ServiceProviderId == request.ServiceProvider)
                    .ToList();



                    closingTicketQuery = closingTicketQuery
                       .Where(x => x.ServiceProviderId == request.ServiceProvider)
                    .ToList();

                    if (request.Channel is not null)
                    {
                        openTicketQuery = openTicketQuery
                           .Where(x => x.ChannelId == request.Channel)
                        .ToList();



                        closingTicketQuery = closingTicketQuery
                           .Where(x => x.ChannelId == request.Channel)
                           .ToList();



                        if (request.UserId is not null)
                        {
                            openTicketQuery = openTicketQuery
                                .Where(x => x.AssignTo == request.UserId)
                            .ToList();

                            closingTicketQuery = closingTicketQuery
                                    .Where(x => x.AssignTo == request.UserId)
                                .ToList();
                        }
                    }
                }


                if (!string.IsNullOrEmpty(request.Search))
                {
                    var normalizedSearch = System.Text.RegularExpressions.Regex.Replace(request.Search.ToLower().Trim(), @"\s+", " ");

                    openTicketQuery = openTicketQuery
                    .Where(x => x.TicketNo.ToString().ToLower().Contains(request.Search)
                        || x.Assign.ToLower().Contains(request.Search)
                        || x.Store.ToLower().Contains(request.Search)
                        || x.Solution.ToLower().Contains(request.Search)
                        || x.RequestType.ToLower().Contains(request.Search)
                        || x.Status.ToLower().Contains(request.Search)
                        || x.Rating.ToLower().Contains(request.Search)
                        || System.Text.RegularExpressions.Regex.Replace(x.Description.ToLower(), @"\s+", " ").Contains(normalizedSearch)).ToList();



                    closingTicketQuery = closingTicketQuery
                   .Where(x => x.TicketNo.ToString().ToLower().Contains(request.Search)
                        || x.Assign.ToLower().Contains(request.Search)
                        || x.Store.ToLower().Contains(request.Search)
                        || x.Solution.ToLower().Contains(request.Search)
                        || x.RequestType.ToLower().Contains(request.Search)
                        || x.Status.ToLower().Contains(request.Search)
                        || x.Rating.ToLower().Contains(request.Search)
                        || System.Text.RegularExpressions.Regex.Replace(x.Description.ToLower(), @"\s+", " ").Contains(normalizedSearch)).ToList();



                }

                foreach (var list in openTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                foreach (var list in closingTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                var results = combineTicketReports
                    .OrderBy(x => x.OpenDate.Value.Date)
                    .ThenBy(x => x.TicketNo)
                    .Select(r => new SLAReportResult
                    {
                        Year = r.Year,
                        Month = r.Month,
                        TicketNo = r.TicketNo,
                        Assign = r.Assign,
                        Store = r.Store,
                        Description = r.Description,
                        OpenDate = r.OpenDate,
                        TargetDate  = r.TargetDate,
                        ForClosingDate = r.ForClosingDate,
                        ClosedDate = r.ClosedDate,
                        Solution = r.Solution,
                        RequestType = r.RequestType,
                        Status = r.Status,
                        Rating = r.Rating,
                        Category = r.Category,
                        SubCategory = r.SubCategory,
                        Position = r.Position,
                    }).AsQueryable();
                return PagedList<SLAReportResult>.Create(results, request.PageNumber, request.PageSize);
            }
        }

    }
}
