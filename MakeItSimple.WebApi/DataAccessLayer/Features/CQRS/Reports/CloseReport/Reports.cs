﻿namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.CloseReport
{
    public partial class TicketReports
    {
        public record class Reports
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public string Start_Date { get; set; }
            public string End_Date { get; set; }
            public string Personnel { get; set; }
            public int Ticket_Number { get; set; }
            public string Description { get; set; }
            public string Target_Date { get; set; }
            public string Actual { get; set; }
            public int Varience { get; set; }
            public string Efficeincy { get; set; }
            public string Status { get; set; }
            public string Remarks { get; set; }
            public string Category { get; set; }
            public string SubCategory { get; set; }
            public int Aging_Day { get; set; }
            public string StartDate { get; set; }
            public string ClosedDate { get; set;}
            public string ForClosedDate { get; set; }
            public string Department { get; set; }
            public bool? IsStore { get; set; }
            public string Technician1 { get; set; }
            public string Technician2 { get; set; }
            public string Technician3 { get; set; }
            public int? ServiceProviderId { get; set; }
            public int? ChannelId { get; set; }
            public string ChannelName { get; set; }
            public Guid? AssignTo { get; set; }
            public string Requestor { get; set; }
            public string CategoryConcern { get; set; }
            public string Notes { get; set; }

        }

    }
}
