namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class ApproverDate
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }

        public int TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

        public bool? IsApproved { get; set; }
        public Guid? ApprovedDateBy { get; set; }
        public string DateRemarks { get; set; }
        public virtual User ApprovedDateByUser { get; set; }

        public bool IsRejectDate { get; set; }
        public DateTime? RejectDateAt { get; set; }
        public Guid? RejectDateBy { get; set; }
        public virtual User RejectDateByUser { get; set; }
        public string RejectRemarks { get; set; }

        public Guid? TicketApprover { get; set; }
        public string Remarks { get; set; }
        public string Resolution { get; set; }
        public string Notes { get; set; }
        public DateTime? ApprovedDateAt { get; set; }

        public ICollection<TicketAttachment> TicketAttachments { get; set; }
        public ICollection<ApproverTicketing> ApproverTickets { get; set; }
    }
}
