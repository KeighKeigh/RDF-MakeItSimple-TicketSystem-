namespace MakeItSimple.WebApi.DataAccessLayer.Features.Overview.Ticket_Overview
{
    public partial class TicketOverview
    {
        public record TicketOverviewIssueHandler
        {
            public int? TicketConcernId { get; set; }
            public string Concerns { get; set; }
            public DateTime? Target_Date { get; set; }
            public string Status { get; set; }
            public string Closing_Status { get; set; }
            public DateTime Created_At { get; set; }


        }
    }
}
