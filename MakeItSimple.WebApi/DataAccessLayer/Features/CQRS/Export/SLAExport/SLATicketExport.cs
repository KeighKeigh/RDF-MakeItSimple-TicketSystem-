using Azure.Core;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.SLAExport.SLAReport;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.SLAExport
{
    public class SLATicketExport
    {

        public class SLATicketExportCommand : UserParams, IRequest<Unit>
        {
            public string Search { get; set; }
            public int? Channel { get; set; }
            public int? ServiceProvider { get; set; }
            public Guid? UserId { get; set; }
            public string Remarks { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }
        }

        public class SLATicketExportResult
        {
            public string Year { get; set; }
            public string Month { get; set; }
            public int? TicketNo { get; set; }
            public string Assign { get; set; }
            public string Store { get; set; }
            public string Description { get; set; }
            public DateTime? OpenDate { get; set; }
            public DateTime? TargetDate { get; set; }
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

        public class Handler : IRequestHandler<SLATicketExportCommand, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(SLATicketExportCommand command, CancellationToken cancellationToken)
            {
                var combineTicketReports = new List<SLAReportResult>();
                var dateToday = DateTime.Now;

                var openTicketQuery = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(t => t.RequestConcern)
                    .AsSplitQuery()
                .Where(t => t.IsApprove == true && t.IsTransfer != true && t.IsClosedApprove != true && t.OnHold != true && t.IsDone != true)
                .Where(t => t.DateApprovedAt.Value.Date >= command.Date_From.Value.Date && t.DateApprovedAt.Value.Date <= command.Date_To.Value.Date)
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
                        Rating = o.TargetDate.Value.Date < dateToday ? "Delay" : "On Time",
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
                .Where(t => t.ClosingAt.Value.Date >= command.Date_From.Value.Date && t.ClosingAt.Value.Date <= command.Date_To.Value.Date)
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
                        Rating = ct.TicketConcern.TargetDate.Value.Date >= ct.TicketConcern.Closed_At.Value.Date ? "On Time" : "Delay",
                        Category = string.Join(", ", ct.TicketConcern.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        SubCategory = string.Join(", ", ct.TicketConcern.RequestConcern.TicketSubCategories.Select(rc => rc.SubCategory.SubCategoryDescription)),
                        Position = "",
                        ServiceProviderId = ct.TicketConcern.RequestConcern.ServiceProviderId,
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId,
                        AssignTo = ct.TicketConcern.AssignTo,
                    }).ToListAsync();




                if (command.ServiceProvider is not null)
                {
                    openTicketQuery = openTicketQuery
                           .Where(x => x.ServiceProviderId == command.ServiceProvider)
                    .ToList();



                    closingTicketQuery = closingTicketQuery
                       .Where(x => x.ServiceProviderId == command.ServiceProvider)
                    .ToList();

                    if (command.Channel is not null)
                    {
                        openTicketQuery = openTicketQuery
                           .Where(x => x.ChannelId == command.Channel)
                        .ToList();



                        closingTicketQuery = closingTicketQuery
                           .Where(x => x.ChannelId == command.Channel)
                           .ToList();



                        if (command.UserId is not null)
                        {
                            openTicketQuery = openTicketQuery
                                .Where(x => x.AssignTo == command.UserId)
                            .ToList();

                            closingTicketQuery = closingTicketQuery
                                    .Where(x => x.AssignTo == command.UserId)
                                .ToList();
                        }
                    }
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
                        TargetDate = r.TargetDate,
                        ClosedDate = r.ClosedDate,
                        Solution = r.Solution,
                        RequestType = r.RequestType,
                        Status = r.Status,
                        Rating = r.Rating,
                        Category = r.Category,
                        SubCategory = r.SubCategory,
                        Position = r.Position,
                    }).ToList();


                if (!string.IsNullOrEmpty(command.Remarks))
                {
                    switch (command.Remarks)
                    {
                        case TicketingConString.OnTime:
                            results = results
                                .Where(x => x.TargetDate > x.ClosedDate)
                            .ToList();
                            break;

                        case TicketingConString.Delay:
                            results = results
                                .Where(x => x.TargetDate < x.ClosedDate)
                                .ToList();
                            break;

                        default:
                            return Unit.Value;

                    }
                }

                if (!string.IsNullOrEmpty(command.Search))
                {
                    var normalizedSearch = System.Text.RegularExpressions.Regex.Replace(command.Search.ToLower().Trim(), @"\s+", " ");

                    results = results
                    .Where(x => x.TicketNo.ToString().ToLower().Contains(command.Search)
                        || x.Assign.ToLower().Contains(command.Search)
                        || x.Store.ToLower().Contains(command.Search)
                        || x.Solution.ToLower().Contains(command.Search)
                        || x.RequestType.ToLower().Contains(command.Search)
                        || x.Status.ToLower().Contains(command.Search)
                        || x.Rating.ToLower().Contains(command.Search)
                        || System.Text.RegularExpressions.Regex.Replace(x.Description.ToLower(), @"\s+", " ").Contains(normalizedSearch)).ToList();


                }


                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"SLA Report");
                    var headers = new List<string>
                    {
                        "Year",
                        "Month",
                        "Ticket No",
                        "Assign",
                        "Store",
                        "Description",
                        "Open Date",
                        "Target Date",
                        "Closed Date",
                        "Solution",
                        "Request Type",
                        "Status",
                        "Rating",
                        "Category",
                        "Sub Category",
                        "Position"

                    };


                    var range = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                    range.Style.Fill.BackgroundColor = XLColor.LavenderPurple;
                    range.Style.Font.Bold = true;
                    range.Style.Font.FontColor = XLColor.Black;
                    range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    for (var index = 1; index <= headers.Count; index++)
                    {
                        worksheet.Cell(1, index).Value = headers[index - 1];
                    }
                    for (var index = 1; index <= results.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = results[index - 1].Year;
                        row.Cell(2).Value = results[index - 1].Month;
                        row.Cell(3).Value = results[index - 1].TicketNo;
                        row.Cell(4).Value = results[index - 1].Assign;
                        row.Cell(5).Value = results[index - 1].Store;
                        row.Cell(6).Value = results[index - 1].Description;
                        row.Cell(7).Value = results[index - 1].OpenDate;
                        row.Cell(8).Value = results[index - 1].TargetDate;
                        row.Cell(9).Value = results[index - 1].ClosedDate;
                        row.Cell(10).Value = results[index - 1].Solution;
                        row.Cell(11).Value = results[index - 1].RequestType;
                        row.Cell(12).Value = results[index - 1].Status;
                        row.Cell(13).Value = results[index - 1].Rating;
                        row.Cell(14).Value = results[index - 1].Category;
                        row.Cell(15).Value = results[index - 1].SubCategory;
                        row.Cell(16).Value = results[index - 1].Position;

                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"SLATicketReports {command.Date_From:MM-dd-yyyy} - {command.Date_To:MM-dd-yyyy}.xlsx");
                }

                return Unit.Value;

            }
        }
    }
}
