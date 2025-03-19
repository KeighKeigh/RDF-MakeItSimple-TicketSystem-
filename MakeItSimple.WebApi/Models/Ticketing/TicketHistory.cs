﻿namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class TicketHistory
    {
        public int Id { get; set; }

        public Guid ? TransactedBy { get; set; }
        public virtual User TransactedByUser { get; set; }

        public string Status { get; set; }
        public string Request {  get; set; }
        public DateTime ? TransactionDate { get; set; }

        public int ? TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

        public string Remarks { get; set; }
        public int ? Approver_Level { get; set; }
        public bool ? IsApprove { get; set; }

    }

}
