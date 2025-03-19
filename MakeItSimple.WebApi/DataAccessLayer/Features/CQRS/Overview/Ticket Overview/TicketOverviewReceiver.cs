namespace MakeItSimple.WebApi.DataAccessLayer.Features.Overview.Ticket_Overview
{
    public partial class TicketOverview
    {
        public record TicketOverviewReceiver
        {
            public int? TicketConcernId { get; set; }
            public string Personnel { get; set; }
            public string Concerns { get; set; }
            public string Channel_Name { get; set; }
            public string Status { get; set; }
            public string Closing_Status { get; set; }
            public DateTime Created_At { get; set; }

        }
    }
}
