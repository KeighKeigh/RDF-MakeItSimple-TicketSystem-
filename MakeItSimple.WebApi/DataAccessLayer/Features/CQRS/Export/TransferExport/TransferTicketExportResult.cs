﻿namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.TransferExport
{
    public partial class TransferTicketExport
    {
        public record TransferTicketExportResult
        {
            public Guid? UserId { get; set; }
            public int? Unit { get; set; }
            public int? TicketConcernId { get; set; }
            public int? TransferTicketId { get; set; }
            public string Concern_Details { get; set; }
            public string Transfered_By { get; set; }
            public string Transfered_To { get; set; }
            public DateTime Current_Target_Date { get; set; }
            public DateTime? Target_Date { get; set; }
            public DateTime? Transfer_At { get; set; }
            public string Transfer_Remarks { get; set; }
            public string Remarks { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public string ApprovedBy { get; set; }
            public int? ChannnelId { get; set; }
            public string ChannnelName { get; set; }
            public int? ServiceProviderId { get; set; }
            public string ServiceProviderName { get; set;}
        }
    }
}
