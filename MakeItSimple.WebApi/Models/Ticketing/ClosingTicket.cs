﻿using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MakeItSimple.WebApi.Models.Setup.UnitSetup;

namespace MakeItSimple.WebApi.Models.Ticketing
{
    public class ClosingTicket 
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }

        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int TicketConcernId { get; set; }
        public virtual TicketConcern TicketConcern { get; set; }

        public bool? IsClosing { get; set; }
        public DateTime? ClosingAt { get; set; }
        public Guid? ClosedBy { get; set; }
        public string ClosingRemarks { get; set; }
        public virtual User ClosedByUser { get; set; }

        public bool IsRejectClosed { get; set; }
        public DateTime? RejectClosedAt { get; set; }
        public Guid? RejectClosedBy { get; set; }
        public virtual User RejectClosedByUser { get; set; }
        public string RejectRemarks { get; set; }

        public Guid? TicketApprover { get; set; }
        public string Remarks { get; set; }
        public string Resolution { get; set; }
        public string Notes { get; set; }

        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int? SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }

        public ICollection<TicketAttachment> TicketAttachments { get; set; }
        public ICollection<ApproverTicketing> ApproverTickets { get; set; }

        public ICollection<TicketTechnician> ticketTechnicians { get; set; }



        

    }
}
