using ClosedXML.Excel;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.AllTicketReport.AllTicketReports;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.AllTicketExport
{
    public class AllTicketExport
    {

        public class AllTicketExportCommand : IRequest<Unit>
        {
            public string Search { get; set; }
            public int? Channel { get; set; }
            public int? ServiceProvider { get; set; }
            public Guid? UserId { get; set; }
            public string Remarks { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }

        }
        public record class AllTicketExportResult
        {
            public int? TicketConcernId { get; set; }
            public string Request_Type { get; set; }
            public int? BackJobId { get; set; }
            public string Requestor_Name { get; set; }
            public string Company_Code { get; set; }
            public string Company_Name { get; set; }
            public string BusinessUnit_Code { get; set; }
            public string BusinessUnit_Name { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public string Location_Code { get; set; }
            public string Location_Name { get; set; }
            public int? Personnel_Unit { get; set; }
            public Guid? Personnel_Id { get; set; }
            public string Personnel { get; set; }
            public string Concerns { get; set; }
            public string Channel_Name { get; set; }
            public string TicketCategoryDescriptions { get; set; }
            public string TicketSubCategoryDescriptions { get; set; }
            public DateTime? Date_Needed { get; set; }
            public string Contact_Number { get; set; }
            public string Notes { get; set; }
            public DateTime? Transaction_Date { get; set; }
            public DateTime? Target_Date { get; set; }
            public string Ticket_Status { get; set; }
            public string Remarks { get; set; }
            public int? Aging_Days { get; set; }
            public int? ChannelId { get; set; }
            public int? ServiceProvider { get; set; }
            public string ServiceProviderName { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? ClosedDate { get; set; }
            public string AssignTo { get; set; }


        }

        public class Handler : IRequestHandler<AllTicketExportCommand, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(AllTicketExportCommand request, CancellationToken cancellationToken)
            {
                var combineTicketReports = new List<AllTicketReportsResult>();


                var openTicketQuery = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(t => t.RequestConcern)
                    .AsSplitQuery()
                    .Where(t => t.IsApprove == true && t.IsTransfer != true && t.IsClosedApprove != true && t.OnHold != true && t.IsDone != true)
                    .Where(t => t.DateApprovedAt.Value.Date >= request.Date_From.Value.Date && t.DateApprovedAt.Value.Date <= request.Date_To.Value.Date)
                    .Select(o => new AllTicketReportsResult
                    {

                        TicketConcernId = o.Id,

                        Request_Type = o.RequestConcern.RequestType,
                        BackJobId = o.RequestConcern.BackJobId,
                        Requestor_Name = o.RequestorByUser.Fullname,
                        Company_Code = o.RequestConcern.Company.CompanyCode,
                        Company_Name = o.RequestConcern.Company.CompanyName,
                        BusinessUnit_Code = o.RequestConcern.BusinessUnit.BusinessCode,
                        BusinessUnit_Name = o.RequestConcern.BusinessUnit.BusinessName,
                        Department_Code = o.RequestConcern.Department.DepartmentCode,
                        Department_Name = o.RequestConcern.Department.DepartmentName,
                        Unit_Code = o.RequestConcern.Unit.UnitCode,
                        Unit_Name = o.RequestConcern.Unit.UnitName,
                        SubUnit_Code = o.RequestConcern.SubUnit.SubUnitCode,
                        SubUnit_Name = o.RequestConcern.SubUnit.SubUnitName,
                        Location_Code = o.RequestConcern.Location.LocationCode,
                        Location_Name = o.RequestConcern.Location.LocationName,
                        Personnel_Unit = o.User.UnitId,
                        Personnel_Id = o.UserId,
                        Personnel = o.User.Fullname,
                        Concerns = o.RequestConcern.Concern,
                        Channel_Name = o.RequestConcern.Channel.ChannelName,
                        TicketCategoryDescriptions = string.Join(", ", o.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)),
                        TicketSubCategoryDescriptions = string.Join(", ", o.RequestConcern.TicketSubCategories
                           .Select(x => x.SubCategory.SubCategoryDescription)),
                        Date_Needed = o.RequestConcern.DateNeeded,
                        Contact_Number = o.RequestConcern.ContactNumber,
                        Notes = o.RequestConcern.Notes,
                        Transaction_Date = o.CreatedAt.Date,
                        Target_Date = o.TargetDate.Value.Date,
                        Ticket_Status = "Open",
                        Remarks = o.Remarks,
                        Aging_Days = EF.Functions.DateDiffDay(o.DateApprovedAt.Value.Date, DateTime.Now.Date),
                        ChannelId = o.RequestConcern.ChannelId.Value,
                        StartDate = o.DateApprovedAt,
                        ClosedDate = o.Closed_At,
                        ServiceProvider = o.RequestConcern.ServiceProviderId.Value,
                        AssignTo = o.RequestConcern.AssignToUser.Fullname,
                        ServiceProviderName = o.RequestConcern.ServiceProvider.ServiceProviderName

                    }).ToListAsync();

                var transferTicketQuery = await _context.TransferTicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsTransfer == true && x.IsActive == true)
                    .Where(t => t.TransferAt.Value.Date >= request.Date_From.Value.Date && t.TransferAt.Value.Date <= request.Date_To.Value.Date)
                    .Select(ct => new AllTicketReportsResult
                    {
                        TicketConcernId = ct.TicketConcernId,
                        Request_Type = ct.TicketConcern.RequestConcern.RequestType,
                        BackJobId = ct.TicketConcern.RequestConcern.BackJobId,
                        Requestor_Name = ct.TicketConcern.RequestorByUser.Fullname,
                        Company_Code = ct.TicketConcern.RequestConcern.Company.CompanyCode,
                        Company_Name = ct.TicketConcern.RequestConcern.Company.CompanyName,
                        BusinessUnit_Code = ct.TicketConcern.RequestConcern.BusinessUnit.BusinessCode,
                        BusinessUnit_Name = ct.TicketConcern.RequestConcern.BusinessUnit.BusinessName,
                        Department_Code = ct.TicketConcern.RequestConcern.Department.DepartmentCode,
                        Department_Name = ct.TicketConcern.RequestConcern.Department.DepartmentName,
                        Unit_Code = ct.TicketConcern.RequestConcern.Unit.UnitCode,
                        Unit_Name = ct.TicketConcern.RequestConcern.Unit.UnitName,
                        SubUnit_Code = ct.TicketConcern.RequestConcern.SubUnit.SubUnitCode,
                        SubUnit_Name = ct.TicketConcern.RequestConcern.SubUnit.SubUnitName,
                        Location_Code = ct.TicketConcern.RequestConcern.Location.LocationCode,
                        Location_Name = ct.TicketConcern.RequestConcern.Location.LocationName,
                        Personnel_Unit = ct.TicketConcern.User.UnitId,
                        Personnel_Id = ct.TicketConcern.UserId,
                        Personnel = ct.TransferToUser.Fullname,
                        Concerns = ct.TicketConcern.RequestConcern.Concern,
                        Channel_Name = ct.TicketConcern.RequestConcern.Channel.ChannelName,
                        TicketCategoryDescriptions = string.Join(", ", ct.TicketConcern.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)),
                        TicketSubCategoryDescriptions = string.Join(", ", ct.TicketConcern.RequestConcern.TicketSubCategories
                               .Select(x => x.SubCategory.SubCategoryDescription)),
                        Date_Needed = ct.TicketConcern.RequestConcern.DateNeeded,
                        Contact_Number = ct.TicketConcern.RequestConcern.ContactNumber,
                        Notes = ct.TicketConcern.RequestConcern.Notes,
                        Transaction_Date = ct.TransferAt.Value.Date,
                        Target_Date = ct.Current_Target_Date.Value.Date,
                        Ticket_Status = "Transfer",
                        Remarks = ct.TransferRemarks,
                        Aging_Days = EF.Functions.DateDiffDay(ct.TicketConcern.DateApprovedAt.Value.Date, DateTime.Now.Date),
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.TicketConcern.DateApprovedAt,
                        ClosedDate = ct.TicketConcern.Closed_At,
                        ServiceProvider = ct.TicketConcern.RequestConcern.ServiceProviderId.Value,
                        AssignTo = ct.TransferToUser.Fullname,
                        ServiceProviderName = ct.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName
                    }).ToListAsync();

                var onHoldTicketQuery = await _context.TicketOnHolds
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsHold == true && x.IsActive == true)
                    .Where(t => t.CreatedAt.Date >= request.Date_From.Value.Date && t.CreatedAt.Date <= request.Date_To.Value.Date)
                    .Select(ct => new AllTicketReportsResult
                    {
                        TicketConcernId = ct.TicketConcernId,
                        Request_Type = ct.TicketConcern.RequestConcern.RequestType,
                        BackJobId = ct.TicketConcern.RequestConcern.BackJobId,
                        Requestor_Name = ct.TicketConcern.RequestorByUser.Fullname,
                        Company_Code = ct.TicketConcern.RequestConcern.Company.CompanyCode,
                        Company_Name = ct.TicketConcern.RequestConcern.Company.CompanyName,
                        BusinessUnit_Code = ct.TicketConcern.RequestConcern.BusinessUnit.BusinessCode,
                        BusinessUnit_Name = ct.TicketConcern.RequestConcern.BusinessUnit.BusinessName,
                        Department_Code = ct.TicketConcern.RequestConcern.Department.DepartmentCode,
                        Department_Name = ct.TicketConcern.RequestConcern.Department.DepartmentName,
                        Unit_Code = ct.TicketConcern.RequestConcern.Unit.UnitCode,
                        Unit_Name = ct.TicketConcern.RequestConcern.Unit.UnitName,
                        SubUnit_Code = ct.TicketConcern.RequestConcern.SubUnit.SubUnitCode,
                        SubUnit_Name = ct.TicketConcern.RequestConcern.SubUnit.SubUnitName,
                        Location_Code = ct.TicketConcern.RequestConcern.Location.LocationCode,
                        Location_Name = ct.TicketConcern.RequestConcern.Location.LocationName,
                        Personnel_Unit = ct.TicketConcern.User.UnitId,
                        Personnel_Id = ct.TicketConcern.UserId,
                        Personnel = ct.TicketConcern.User.Fullname,
                        Concerns = ct.TicketConcern.RequestConcern.Concern,
                        Channel_Name = ct.TicketConcern.RequestConcern.Channel.ChannelName,
                        TicketCategoryDescriptions = string.Join(", ", ct.TicketConcern.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)),
                        TicketSubCategoryDescriptions = string.Join(", ", ct.TicketConcern.RequestConcern.TicketSubCategories
                               .Select(x => x.SubCategory.SubCategoryDescription)),
                        Date_Needed = ct.TicketConcern.RequestConcern.DateNeeded,
                        Contact_Number = ct.TicketConcern.RequestConcern.ContactNumber,
                        Notes = ct.TicketConcern.RequestConcern.Notes,
                        Transaction_Date = ct.CreatedAt.Date,
                        Target_Date = ct.TicketConcern.TargetDate.Value.Date,
                        Ticket_Status = "On-Hold",
                        Remarks = ct.OnHoldRemarks,
                        Aging_Days = EF.Functions.DateDiffDay(ct.TicketConcern.DateApprovedAt.Value.Date, DateTime.Now.Date),
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.TicketConcern.DateApprovedAt,
                        ClosedDate = ct.TicketConcern.Closed_At,
                        ServiceProvider = ct.TicketConcern.RequestConcern.ServiceProviderId.Value,
                        AssignTo = ct.TicketConcern.RequestConcern.AssignToUser.Fullname,
                        ServiceProviderName = ct.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName

                    }).ToListAsync();

                var closingTicketQuery = await _context.ClosingTickets
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsClosing == true && x.IsActive == true)
                    .Where(t => t.ClosingAt.Value.Date >= request.Date_From.Value.Date && t.ClosingAt.Value.Date <= request.Date_To.Value.Date)
                    .Select(ct => new AllTicketReportsResult
                    {
                        TicketConcernId = ct.TicketConcernId,
                        Request_Type = ct.TicketConcern.RequestConcern.RequestType,
                        BackJobId = ct.TicketConcern.RequestConcern.BackJobId,
                        Requestor_Name = ct.TicketConcern.RequestorByUser.Fullname,
                        Company_Code = ct.TicketConcern.RequestConcern.Company.CompanyCode,
                        Company_Name = ct.TicketConcern.RequestConcern.Company.CompanyName,
                        BusinessUnit_Code = ct.TicketConcern.RequestConcern.BusinessUnit.BusinessCode,
                        BusinessUnit_Name = ct.TicketConcern.RequestConcern.BusinessUnit.BusinessName,
                        Department_Code = ct.TicketConcern.RequestConcern.Department.DepartmentCode,
                        Department_Name = ct.TicketConcern.RequestConcern.Department.DepartmentName,
                        Unit_Code = ct.TicketConcern.RequestConcern.Unit.UnitCode,
                        Unit_Name = ct.TicketConcern.RequestConcern.Unit.UnitName,
                        SubUnit_Code = ct.TicketConcern.RequestConcern.SubUnit.SubUnitCode,
                        SubUnit_Name = ct.TicketConcern.RequestConcern.SubUnit.SubUnitName,
                        Location_Code = ct.TicketConcern.RequestConcern.Location.LocationCode,
                        Location_Name = ct.TicketConcern.RequestConcern.Location.LocationName,
                        Personnel_Unit = ct.TicketConcern.User.UnitId,
                        Personnel_Id = ct.TicketConcern.UserId,
                        Personnel = ct.TicketConcern.User.Fullname,
                        Concerns = ct.TicketConcern.RequestConcern.Concern,
                        Channel_Name = ct.TicketConcern.RequestConcern.Channel.ChannelName,
                        TicketCategoryDescriptions = string.Join(", ", ct.TicketConcern.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)),
                        TicketSubCategoryDescriptions = string.Join(", ", ct.TicketConcern.RequestConcern.TicketSubCategories
                               .Select(x => x.SubCategory.SubCategoryDescription)),
                        Date_Needed = ct.TicketConcern.RequestConcern.DateNeeded,
                        Contact_Number = ct.TicketConcern.RequestConcern.ContactNumber,
                        Notes = ct.Notes,
                        Transaction_Date = ct.ClosingAt.Value.Date,
                        Target_Date = ct.TicketConcern.TargetDate.Value.Date,
                        Ticket_Status = "Closed",
                        Remarks = ct.ClosingRemarks,
                        Aging_Days = EF.Functions.DateDiffDay(ct.TicketConcern.DateApprovedAt.Value.Date, ct.ClosingAt.Value.Date),
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.TicketConcern.DateApprovedAt,
                        ClosedDate = ct.TicketConcern.Closed_At,
                        ServiceProvider = ct.TicketConcern.RequestConcern.ServiceProviderId.Value,
                        AssignTo = ct.TicketConcern.RequestConcern.AssignToUser.Fullname,
                        ServiceProviderName = ct.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName,
                        ClosingStatus = ct.TicketConcern.TargetDate.Value.Date < ct.TicketConcern.Closed_At.Value.Date ? "Delayed" : "On-Time"
                    }).ToListAsync();


                var closingTicketTechnicianQuery = await _context.TicketTechnicians
                    .AsNoTrackingWithIdentityResolution()
                    .Include(ct => ct.ClosingTicket)
                    .ThenInclude(ct => ct.TicketConcern)
                    .ThenInclude(ct => ct.RequestConcern)
                    .Include(ct => ct.TechnicianByUser)
                    .AsSplitQuery()
                    .Where(ct => ct.ClosingTicket.IsClosing == true && ct.ClosingTicket.IsActive == true)
                    .Where(t => t.ClosingTicket.ClosingAt.Value.Date >= request.Date_From.Value.Date && t.ClosingTicket.ClosingAt.Value.Date <= request.Date_To.Value.Date)
                    .Select(ct => new AllTicketReportsResult
                    {
                        TicketConcernId = ct.ClosingTicket.TicketConcernId,
                        Request_Type = ct.ClosingTicket.TicketConcern.RequestConcern.RequestType,
                        BackJobId = ct.ClosingTicket.TicketConcern.RequestConcern.BackJobId,
                        Requestor_Name = ct.ClosingTicket.TicketConcern.RequestorByUser.Fullname,
                        Company_Code = ct.ClosingTicket.TicketConcern.RequestConcern.Company.CompanyCode,
                        Company_Name = ct.ClosingTicket.TicketConcern.RequestConcern.Company.CompanyName,
                        BusinessUnit_Code = ct.ClosingTicket.TicketConcern.RequestConcern.BusinessUnit.BusinessCode,
                        BusinessUnit_Name = ct.ClosingTicket.TicketConcern.RequestConcern.BusinessUnit.BusinessName,
                        Department_Code = ct.ClosingTicket.TicketConcern.RequestConcern.Department.DepartmentCode,
                        Department_Name = ct.ClosingTicket.TicketConcern.RequestConcern.Department.DepartmentName,
                        Unit_Code = ct.ClosingTicket.TicketConcern.RequestConcern.Unit.UnitCode,
                        Unit_Name = ct.ClosingTicket.TicketConcern.RequestConcern.Unit.UnitName,
                        SubUnit_Code = ct.ClosingTicket.TicketConcern.RequestConcern.SubUnit.SubUnitCode,
                        SubUnit_Name = ct.ClosingTicket.TicketConcern.RequestConcern.SubUnit.SubUnitCode,
                        Location_Code = ct.ClosingTicket.TicketConcern.RequestConcern.Location.LocationCode,
                        Location_Name = ct.ClosingTicket.TicketConcern.RequestConcern.Location.LocationName,
                        Personnel_Unit = ct.TechnicianByUser.UnitId,
                        Personnel_Id = ct.TechnicianBy,
                        Personnel = ct.TechnicianByUser.Fullname,
                        Concerns = ct.ClosingTicket.TicketConcern.RequestConcern.Concern,
                        Channel_Name = ct.ClosingTicket.TicketConcern.RequestConcern.Channel.ChannelName,
                        TicketCategoryDescriptions = string.Join(", ", ct.ClosingTicket.TicketConcern.RequestConcern.TicketCategories
                           .Select(x => x.Category.CategoryDescription)),
                        TicketSubCategoryDescriptions = string.Join(", ", ct.ClosingTicket.TicketConcern.RequestConcern.TicketSubCategories
                            .Select(x => x.SubCategory.SubCategoryDescription)),
                        Date_Needed = ct.ClosingTicket.TicketConcern.RequestConcern.DateNeeded,
                        Contact_Number = ct.ClosingTicket.TicketConcern.RequestConcern.ContactNumber,
                        Notes = ct.ClosingTicket.Notes,
                        Transaction_Date = ct.ClosingTicket.ClosingAt.Value.Date,
                        Target_Date = ct.ClosingTicket.TicketConcern.TargetDate.Value.Date,
                        Ticket_Status = "Closed",
                        Remarks = ct.ClosingTicket.ClosingRemarks,
                        Aging_Days = EF.Functions.DateDiffDay(ct.ClosingTicket.TicketConcern.DateApprovedAt.Value.Date, ct.ClosingTicket.ClosingAt.Value.Date),
                        ChannelId = ct.ClosingTicket.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.ClosingTicket.TicketConcern.DateApprovedAt,
                        ClosedDate = ct.ClosingTicket.TicketConcern.Closed_At,
                        ServiceProvider = ct.ClosingTicket.TicketConcern.RequestConcern.ServiceProviderId.Value,
                        AssignTo = ct.ClosingTicket.TicketConcern.RequestConcern.AssignToUser.Fullname,
                        ServiceProviderName = ct.ClosingTicket.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName
                    }).ToListAsync();


                foreach (var list in openTicketQuery)
                {
                    combineTicketReports.Add(list);
                }
                foreach (var list in transferTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                foreach (var list in onHoldTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                foreach (var list in closingTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                foreach (var list in closingTicketTechnicianQuery)
                {
                    combineTicketReports.Add(list);
                }

                var results = combineTicketReports
                    .OrderBy(x => x.Transaction_Date.Value.Date)
                    .ThenBy(x => x.TicketConcernId)
                    .Select(r => new AllTicketReportsResult
                    {
                        TicketConcernId = r.TicketConcernId,
                        Request_Type = r.Request_Type,
                        BackJobId = r.BackJobId,
                        Requestor_Name = r.Requestor_Name,
                        Company_Code = r.Company_Code,
                        Company_Name = r.Company_Name,
                        BusinessUnit_Code = r.BusinessUnit_Code,
                        BusinessUnit_Name = r.BusinessUnit_Name,
                        Department_Code = r.Department_Code,
                        Department_Name = r.Department_Name,
                        Unit_Code = r.Unit_Code,
                        Unit_Name = r.Unit_Name,
                        SubUnit_Code = r.SubUnit_Code,
                        SubUnit_Name = r.SubUnit_Name,
                        Location_Code = r.Location_Code,
                        Location_Name = r.Location_Name,
                        Personnel = r.Personnel,
                        Concerns = r.Concerns,
                        Channel_Name = r.Channel_Name,
                        TicketCategoryDescriptions = r.TicketCategoryDescriptions,
                        TicketSubCategoryDescriptions = r.TicketSubCategoryDescriptions,
                        Date_Needed = r.Date_Needed,
                        Contact_Number = r.Contact_Number,
                        Notes = r.Notes,
                        Transaction_Date = r.Transaction_Date,
                        Target_Date = r.Target_Date,
                        Ticket_Status = r.Ticket_Status,
                        Remarks = r.Remarks,
                        Aging_Days = r.Aging_Days,
                        ChannelId = r.ChannelId,
                        StartDate = r.StartDate.Value.Date,
                        ClosedDate = r.ClosedDate,
                        AssignTo = r.AssignTo,
                        ServiceProvider = r.ServiceProvider,
                        ServiceProviderName = r.ServiceProviderName,
                        ClosingStatus = r.ClosingStatus,
                        Personnel_Id = r.Personnel_Id

                    }).ToList();


                if (request.ServiceProvider is not null)
                {
                    results = results
                           .Where(x => x.ServiceProvider == request.ServiceProvider)
                           .ToList();

                    if (request.Channel is not null)
                    {
                        results = results
                           .Where(x => x.ChannelId == request.Channel)
                           .ToList();

                        if (request.UserId is not null)
                        {
                            results = results
                                .Where(x => x.Personnel_Id == request.UserId)
                                .ToList();
                        }
                    }
                }


                if (!string.IsNullOrEmpty(request.Search))
                {
                    var normalizedSearch = System.Text.RegularExpressions.Regex.Replace(request.Search.ToLower().Trim(), @"\s+", " ");

                    results = results
                    .Where(x => x.TicketConcernId.ToString().ToLower().Contains(request.Search)
                        || x.Personnel.ToLower().Contains(request.Search)
                        || x.Request_Type.ToLower().Contains(request.Search)
                        || x.BackJobId.ToString().ToLower().Contains(request.Search)
                        || x.Requestor_Name.ToLower().Contains(request.Search)
                        || x.Company_Code.ToLower().Contains(request.Search)
                        || x.Company_Name.ToLower().Contains(request.Search)
                        || x.BusinessUnit_Code.ToLower().Contains(request.Search)
                        || x.BusinessUnit_Name.ToLower().Contains(request.Search)
                        || x.Department_Code.ToLower().ToString().Contains(request.Search)
                        || x.Department_Name.ToLower().Contains(request.Search)
                        || x.Unit_Code.ToLower().Contains(request.Search)
                        || x.Unit_Name.ToLower().Contains(request.Search)
                        || x.SubUnit_Code.ToLower().Contains(request.Search)
                        || x.SubUnit_Name.ToLower().Contains(request.Search)
                        || x.Location_Code.ToLower().ToString().Contains(request.Search)
                        || x.Location_Name.ToLower().Contains(request.Search)
                        || System.Text.RegularExpressions.Regex.Replace(x.Concerns.ToLower(), @"\s+", " ").Contains(normalizedSearch)
                        || x.AssignTo.ToLower().Contains(request.Search)).ToList();
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Closing Ticket Report");
                    var headers = new List<string>
                    {
                        "Ticket Number",
                        "Ticket Description",
                        "Category",
                        "Sub Catgory",
                        "Service Provider",
                        "Channel",
                        "Start Date",
                        "Target Date",
                        "Requestor",
                        "Company",
                        "Business Unit",
                        "Department",
                        "Unit",
                        "Sub Unit",
                        "Location",
                        "Issue Handler",
                        "Status"



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

                        row.Cell(1).Value = results[index - 1].TicketConcernId;
                        row.Cell(2).Value = results[index - 1].Concerns;
                        row.Cell(3).Value = results[index - 1].TicketCategoryDescriptions;
                        row.Cell(4).Value = results[index - 1].TicketSubCategoryDescriptions;
                        row.Cell(5).Value = results[index - 1].ServiceProviderName;
                        row.Cell(6).Value = results[index - 1].Channel_Name;
                        row.Cell(7).Value = results[index - 1].StartDate;
                        row.Cell(8).Value = results[index - 1].Target_Date;
                        row.Cell(9).Value = results[index - 1].Requestor_Name;
                        row.Cell(10).Value = $"{results[index - 1].Company_Code} - {results[index - 1].Company_Name}";
                        row.Cell(11).Value = $"{results[index - 1].BusinessUnit_Code} - {results[index - 1].BusinessUnit_Name}";
                        row.Cell(12).Value = $"{results[index - 1].Department_Code} - {results[index - 1].Department_Name}";
                        row.Cell(13).Value = $"{results[index - 1].Unit_Code} - {results[index - 1].Unit_Name}";
                        row.Cell(14).Value = $"{results[index - 1].SubUnit_Code} - {results[index - 1].SubUnit_Name}";
                        row.Cell(15).Value = $"{results[index - 1].Location_Code} - {results[index - 1].Location_Name}";
                        row.Cell(16).Value = results[index - 1].AssignTo;
                        row.Cell(17).Value = results[index - 1].Ticket_Status;


                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"AllTicketReports {request.Date_From:MM-dd-yyyy} - {request.Date_To:MM-dd-yyyy}.xlsx");

                }

                return Unit.Value;
            }
        }

    }
}
