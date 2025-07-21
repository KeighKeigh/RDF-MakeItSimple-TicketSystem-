//using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
//using MediatR;

//namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.AllTicketExport
//{
//    public class AllTicketExport
//    {

//        public class AllTicketExportCommand : IRequest<Unit>
//        {
//            public string Search { get; set; }
//            public int? Channel { get; set; }
//            public int? ServiceProvider { get; set; }
//            public Guid? UserId { get; set; }
//            public string Remarks { get; set; }
//            public DateTime? Date_From { get; set; }
//            public DateTime? Date_To { get; set; }

//        }
//        public record class AllTicketExportResult
//        {
//            public int? TicketConcernId { get; set; }
//            public string Request_Type { get; set; }
//            public int? BackJobId { get; set; }
//            public string Requestor_Name { get; set; }
//            public string Company_Code { get; set; }
//            public string Company_Name { get; set; }
//            public string BusinessUnit_Code { get; set; }
//            public string BusinessUnit_Name { get; set; }
//            public string Department_Code { get; set; }
//            public string Department_Name { get; set; }
//            public string Unit_Code { get; set; }
//            public string Unit_Name { get; set; }
//            public string SubUnit_Code { get; set; }
//            public string SubUnit_Name { get; set; }
//            public string Location_Code { get; set; }
//            public string Location_Name { get; set; }
//            public int? Personnel_Unit { get; set; }
//            public Guid? Personnel_Id { get; set; }
//            public string Personnel { get; set; }
//            public string Concerns { get; set; }
//            public string Channel_Name { get; set; }
//            public string TicketCategoryDescriptions { get; set; }
//            public string TicketSubCategoryDescriptions { get; set; }
//            public DateTime? Date_Needed { get; set; }
//            public string Contact_Number { get; set; }
//            public string Notes { get; set; }
//            public DateTime? Transaction_Date { get; set; }
//            public DateTime? Target_Date { get; set; }
//            public string Ticket_Status { get; set; }
//            public string Remarks { get; set; }
//            public int? Aging_Days { get; set; }
//            public int? ChannelId { get; set; }
//            public int? ServiceProvider { get; set; }
//            public string ServiceProviderName { get; set; }
//            public DateTime? StartDate { get; set; }
//            public DateTime? ClosedDate { get; set; }
//            public string AssignTo { get; set; }


//        }

//        public class Handler : IRequestHandler<AllTicketExportResult, Unit>
//        {
//            private readonly IUnitOfWork _unitOfWork;

//            public Handler
//        }

//    }
//}
