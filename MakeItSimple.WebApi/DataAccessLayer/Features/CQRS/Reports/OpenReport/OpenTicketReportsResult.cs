﻿namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OpenReport
{
    public partial class OpenTicketReports
    {
        public record OpenTicketReportsResult
        {

            public int? TicketConcernId { get; set; }
            public string Concern_Description { get; set; }
            public string Requestor_Name { get; set; }
            public string CompanyName { get; set; }
            public string Business_Unit_Name { get; set; }
            public string Department_Name { get; set; }
            public string Unit_Name { get; set; }
            public string SubUnit_Name { get; set; }
            public string Location_Name { get; set; }
            public string Category_Description { get; set; }
            public string SubCategory_Description { get; set; }
            public string Issue_Handler { get; set; }
            public string Channel_Name { get; set; }
            public DateTime? Target_Date { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public string Remarks { get; set; }
            public int Aging_Days { get; set; }
            public int? Personnel_Unit { get; set; }
            public Guid? Personnel_Id { get; set; }
            public string Personnel { get; set; }
            public int? ServiceProvider { get; set; }
            public int? ChannelId { get; set; }
            public DateTime? StartDate { get; set; }
            public string AssigTo { get; set; }
            public Guid? AssignTo { get; set; }


        }
    }
}
