using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.AllTicketReport
{
    public partial class AllTicketReports
    {

        public class Handler : IRequestHandler<AllTicketReportsQuery, PagedList<AllTicketReportsResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<AllTicketReportsResult>> Handle(AllTicketReportsQuery request, CancellationToken cancellationToken)
            {

                var combineTicketReports = new List<AllTicketReportsResult>();


                var openTicketQuery = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(t => t.RequestConcern)
                    .AsSplitQuery()
                    .Where(t => t.IsApprove == true && t.IsTransfer != true && t.IsClosedApprove != true && t.OnHold != true)
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
                        Aging_Days = EF.Functions.DateDiffDay(o.TargetDate.Value.Date, DateTime.Now.Date),
                        ChannelId = o.RequestConcern.ChannelId.Value,
                        StartDate = o.DateApprovedAt,
                        ClosedDate = o.Closed_At
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
                        Transaction_Date = ct.TransferAt.Value.Date,
                        Target_Date = ct.Current_Target_Date.Value.Date,
                        Ticket_Status = "Transfer",
                        Remarks = ct.TransferRemarks,
                        Aging_Days = EF.Functions.DateDiffDay(ct.TicketConcern.TargetDate.Value.Date, DateTime.Now.Date),
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.TicketConcern.DateApprovedAt,
                        ClosedDate = ct.TicketConcern.Closed_At
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
                        Aging_Days = EF.Functions.DateDiffDay(ct.TicketConcern.TargetDate.Value.Date, DateTime.Now.Date),
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.TicketConcern.DateApprovedAt,
                        ClosedDate = ct.TicketConcern.Closed_At

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
                        Aging_Days = EF.Functions.DateDiffDay(ct.TicketConcern.TargetDate.Value.Date, ct.ClosingAt.Value.Date),
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.TicketConcern.DateApprovedAt,
                        ClosedDate = ct.TicketConcern.Closed_At

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
                        TicketCategoryDescriptions = string.Join(", ",ct.ClosingTicket.TicketConcern.RequestConcern.TicketCategories
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
                        Aging_Days = EF.Functions.DateDiffDay(ct.ClosingTicket.TicketConcern.TargetDate.Value.Date, ct.ClosingTicket.ClosingAt.Value.Date),
                        ChannelId = ct.ClosingTicket.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.ClosingTicket.TicketConcern.DateApprovedAt,
                        ClosedDate = ct.ClosingTicket.TicketConcern.Closed_At
                    }).ToListAsync();


                if (request.Channel is not null)
                {
                     openTicketQuery = openTicketQuery
                        .Where(x => x.ChannelId == request.Channel)
                        .ToList();

                    transferTicketQuery = transferTicketQuery
                       .Where(x => x.ChannelId == request.Channel)
                       .ToList();

                    onHoldTicketQuery = onHoldTicketQuery
                       .Where(x => x.ChannelId == request.Channel)
                       .ToList();

                    closingTicketQuery = closingTicketQuery
                       .Where(x => x.ChannelId == request.Channel)
                       .ToList();

                    closingTicketTechnicianQuery = closingTicketTechnicianQuery
                       .Where(x => x.ChannelId == request.Channel)
                       .ToList();



                    if (request.UserId is not null)
                    {
                        openTicketQuery = openTicketQuery
                            .Where(x => x.Personnel_Id == request.UserId)
                            .ToList();

                        transferTicketQuery = transferTicketQuery
                                .Where(x => x.Personnel_Id == request.UserId)
                            .ToList();

                        onHoldTicketQuery = onHoldTicketQuery
                                .Where(x => x.Personnel_Id == request.UserId)
                            .ToList();

                        closingTicketQuery = closingTicketQuery
                                .Where(x => x.Personnel_Id == request.UserId)
                            .ToList();

                        closingTicketTechnicianQuery = closingTicketTechnicianQuery
                                .Where(x => x.Personnel_Id == request.UserId)
                            .ToList();
                    }
                }


                if (!string.IsNullOrEmpty(request.Search))
                {
                    openTicketQuery = openTicketQuery
                    .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Personnel.Contains(request.Search)
                        || x.Request_Type.Contains(request.Search)
                        || x.BackJobId.ToString().Contains(request.Search)
                        || x.Requestor_Name.Contains(request.Search)
                        || x.Company_Code.Contains(request.Search)
                        || x.Company_Name.Contains(request.Search)
                        || x.BusinessUnit_Code.Contains(request.Search)
                        || x.BusinessUnit_Name.Contains(request.Search)
                        || x.Department_Code.ToString().Contains(request.Search)
                        || x.Department_Name.Contains(request.Search)
                        || x.Unit_Code.Contains(request.Search)
                        || x.Unit_Name.Contains(request.Search)
                        || x.SubUnit_Code.Contains(request.Search)
                        || x.SubUnit_Name.Contains(request.Search)
                        || x.Location_Code.ToString().Contains(request.Search)
                        || x.Location_Name.Contains(request.Search)
                        || x.Concerns.Contains(request.Search))




                        .ToList();

                    transferTicketQuery = transferTicketQuery
                    .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Personnel.Contains(request.Search)
                        || x.Request_Type.Contains(request.Search)
                        || x.BackJobId.ToString().Contains(request.Search)
                        || x.Requestor_Name.Contains(request.Search)
                        || x.Company_Code.Contains(request.Search)
                        || x.Company_Name.Contains(request.Search)
                        || x.BusinessUnit_Code.Contains(request.Search)
                        || x.BusinessUnit_Name.Contains(request.Search)
                        || x.Department_Code.ToString().Contains(request.Search)
                        || x.Department_Name.Contains(request.Search)
                        || x.Unit_Code.Contains(request.Search)
                        || x.Unit_Name.Contains(request.Search)
                        || x.SubUnit_Code.Contains(request.Search)
                        || x.SubUnit_Name.Contains(request.Search)
                        || x.Location_Code.ToString().Contains(request.Search)
                        || x.Location_Name.Contains(request.Search)
                        || x.Concerns.Contains(request.Search))
                    .ToList();

                    onHoldTicketQuery = onHoldTicketQuery
                   .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Personnel.Contains(request.Search)
                        || x.Request_Type.Contains(request.Search)
                        || x.BackJobId.ToString().Contains(request.Search)
                        || x.Requestor_Name.Contains(request.Search)
                        || x.Company_Code.Contains(request.Search)
                        || x.Company_Name.Contains(request.Search)
                        || x.BusinessUnit_Code.Contains(request.Search)
                        || x.BusinessUnit_Name.Contains(request.Search)
                        || x.Department_Code.ToString().Contains(request.Search)
                        || x.Department_Name.Contains(request.Search)
                        || x.Unit_Code.Contains(request.Search)
                        || x.Unit_Name.Contains(request.Search)
                        || x.SubUnit_Code.Contains(request.Search)
                        || x.SubUnit_Name.Contains(request.Search)
                        || x.Location_Code.ToString().Contains(request.Search)
                        || x.Location_Name.Contains(request.Search)
                        || x.Concerns.Contains(request.Search))
                    .ToList();

                    closingTicketQuery = closingTicketQuery
                   .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Personnel.Contains(request.Search)
                        || x.Request_Type.Contains(request.Search)
                        || x.BackJobId.ToString().Contains(request.Search)
                        || x.Requestor_Name.Contains(request.Search)
                        || x.Company_Code.Contains(request.Search)
                        || x.Company_Name.Contains(request.Search)
                        || x.BusinessUnit_Code.Contains(request.Search)
                        || x.BusinessUnit_Name.Contains(request.Search)
                        || x.Department_Code.ToString().Contains(request.Search)
                        || x.Department_Name.Contains(request.Search)
                        || x.Unit_Code.Contains(request.Search)
                        || x.Unit_Name.Contains(request.Search)
                        || x.SubUnit_Code.Contains(request.Search)
                        || x.SubUnit_Name.Contains(request.Search)
                        || x.Location_Code.ToString().Contains(request.Search)
                        || x.Location_Name.Contains(request.Search)
                        || x.Concerns.Contains(request.Search))
                    .ToList();

                    closingTicketTechnicianQuery = closingTicketTechnicianQuery
                    .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Personnel.Contains(request.Search)
                        || x.Request_Type.Contains(request.Search)
                        || x.BackJobId.ToString().Contains(request.Search)
                        || x.Requestor_Name.Contains(request.Search)
                        || x.Company_Code.Contains(request.Search)
                        || x.Company_Name.Contains(request.Search)
                        || x.BusinessUnit_Code.Contains(request.Search)
                        || x.BusinessUnit_Name.Contains(request.Search)
                        || x.Department_Code.ToString().Contains(request.Search)
                        || x.Department_Name.Contains(request.Search)
                        || x.Unit_Code.Contains(request.Search)
                        || x.Unit_Name.Contains(request.Search)
                        || x.SubUnit_Code.Contains(request.Search)
                        || x.SubUnit_Name.Contains(request.Search)
                        || x.Location_Code.ToString().Contains(request.Search)
                        || x.Location_Name.Contains(request.Search)
                        || x.Concerns.Contains(request.Search))
                    .ToList();
                }

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
                        StartDate = r.StartDate,
                        ClosedDate = r.ClosedDate,

                    }).AsQueryable();

                return PagedList<AllTicketReportsResult>.Create(results, request.PageNumber, request.PageSize);


            }
        }

    }
}
