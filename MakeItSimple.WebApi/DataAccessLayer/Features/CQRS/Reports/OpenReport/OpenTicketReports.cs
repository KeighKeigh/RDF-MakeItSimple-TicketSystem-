using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OpenReport
{
    public partial class OpenTicketReports
    {

        public class Handler : IRequestHandler<OpenTicketReportsQuery, PagedList<OpenTicketReportsResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<OpenTicketReportsResult>> Handle(OpenTicketReportsQuery request, CancellationToken cancellationToken)
            {

                //IQueryable<TicketConcern> ticketQuery = _context.TicketConcerns
                //    .AsNoTrackingWithIdentityResolution()
                //    .Include(x => x.AddedByUser)
                //    .Include(x => x.ModifiedByUser)
                //    .Include(x => x.RequestorByUser)
                //    .Include(x => x.User)
                //    .ThenInclude(x => x.SubUnit)
                //    .Include(x => x.ClosingTickets)
                //    .ThenInclude(x => x.TicketAttachments)
                //    .Include(x => x.TransferTicketConcerns)
                //    .ThenInclude(x => x.TicketAttachments)
                //    .Include(x => x.RequestConcern)
                //    .ThenInclude(x => x.TicketCategories)
                //    .ThenInclude(x => x.Category)
                //    .Include(x => x.RequestConcern)
                //    .ThenInclude(x => x.TicketSubCategories)
                //    .ThenInclude(x => x.SubCategory)

                //    .AsSplitQuery();

                var results = _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.RequestorByUser)
                    .Include(x => x.User)
                    .ThenInclude(x => x.SubUnit)
                    .Include(x => x.ClosingTickets)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.TransferTicketConcerns)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.RequestConcern)
                    .ThenInclude(x => x.TicketCategories)
                    .ThenInclude(x => x.Category)
                    .Include(x => x.RequestConcern)
                    .ThenInclude(x => x.TicketSubCategories)
                    .ThenInclude(x => x.SubCategory)

                    .AsSplitQuery()
                    .Where(x => x.IsApprove == true && x.IsClosedApprove != true && x.OnHold != true && x.IsTransfer != true)
                    .Where(x => x.TargetDate.Value.Date >= request.Date_From.Value.Date && x.TargetDate.Value.Date <= request.Date_To.Value.Date)

                    .Select(t => new OpenTicketReportsResult
                    {
                        TicketConcernId = t.Id,
                        Concern_Description = t.RequestConcern.Concern,
                        Requestor_Name = t.RequestorByUser.Fullname,
                        CompanyName = t.RequestConcern.Company.CompanyName,
                        Business_Unit_Name = t.RequestConcern.BusinessUnit.BusinessName,
                        Department_Name = t.RequestConcern.Department.DepartmentName,
                        Unit_Name = t.RequestConcern.Unit.UnitName,
                        SubUnit_Name = t.RequestConcern.SubUnit.SubUnitName,
                        Location_Name = t.RequestConcern.Location.LocationName,
                        //Category_Description = string.Join(", ", t.RequestConcern.Category.CategoryDescription),
                        Category_Description = string.Join(", ", t.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        SubCategory_Description = string.Join(", ", t.RequestConcern.TicketSubCategories.Select(rc => rc.SubCategory.SubCategoryDescription)),
                        Issue_Handler = t.User.Fullname,
                        Channel_Name = t.RequestConcern.Channel.ChannelName,
                        Target_Date = t.TargetDate,
                        Created_At = t.CreatedAt,
                        Modified_By = t.ModifiedByUser.Fullname,
                        Updated_At = t.UpdatedAt,
                        Remarks = t.Remarks,
                        Aging_Days = EF.Functions.DateDiffDay(t.TargetDate.Value.Date, DateTime.Now.Date),
                        Personnel_Unit = t.User.UnitId,
                        Personnel_Id = t.User.Id,
                        Personnel = t.User.Fullname
                        

                    });

                if (request.Unit is not null)
                {
                    results = results.Where(x => x.Personnel_Unit == request.Unit);

                    if (request.UserId is not null)
                    {
                        results = results.Where(x => x.Personnel_Id == request.UserId);
                    }
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    results = results
                        .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Personnel.Contains(request.Search));
                }

                /*.GroupJoin(_context.TicketCategories, query => query.RequestConcernId, cat => cat.RequestConcernId, (query, cat) => new {query, cat })
                    .SelectMany(x => x.cat.DefaultIfEmpty(), (x , cat) => new {x.query, cat })
                    .GroupJoin(_context.TicketSubCategories, x => x.cat.RequestConcernId , subcat => subcat.RequestConcernId, (x, subcat) => new { x.query, x.cat, subcat})
                    .SelectMany(x => x.subcat.DefaultIfEmpty(), (x, subcat) => new { x.query,x.cat, subcat })*/

                var final = results
                    .OrderBy(x => x.Target_Date.Value.Date)
                    .ThenBy(x => x.TicketConcernId)
                    .Select(f => new OpenTicketReportsResult
                    {
                        TicketConcernId = f.TicketConcernId,
                        Concern_Description = f.Concern_Description,
                        Requestor_Name = f.Requestor_Name,
                        CompanyName = f.CompanyName,
                        Business_Unit_Name = f.Business_Unit_Name,
                        Department_Name = f.Department_Name,
                        Unit_Name = f.Unit_Name,
                        SubUnit_Name = f.SubUnit_Name,
                        Location_Name = f.Location_Name,
                        Category_Description = f.Category_Description,
                        SubCategory_Description = f.SubCategory_Description,
                        Issue_Handler = f.Issue_Handler,
                        Channel_Name = f.Channel_Name,
                        Target_Date = f.Target_Date,
                        Created_At = f.Created_At,
                        Modified_By = f.Modified_By,
                        Updated_At = f.Updated_At,
                        Remarks = f.Remarks,
                        Aging_Days = f.Aging_Days,
                        Personnel_Unit = f.Personnel_Unit,
                        Personnel_Id = f.Personnel_Id,
                        Personnel = f.Personnel

                    }).AsQueryable();

                return await PagedList<OpenTicketReportsResult>.CreateAsync(final, request.PageNumber, request.PageSize);
            }

        }
    }
}
