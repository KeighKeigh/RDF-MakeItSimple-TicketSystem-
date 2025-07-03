﻿using MakeItSimple.WebApi.Models.Ticketing;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket.GetOpenTicket.GetOpenTicketResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket
{
    public partial class GetOpenTicket
    {
        public class GetOpenTicketResult
        {
            public int? TicketConcernId { get; set; }
            public int? RequestConcernId { get; set; }
            public string Concern_Description { get; set; }

            public string Company_Code { get; set; }
            public string Company_Name { get; set; }

            public string BusinessUnit_Code { get; set; }
            public string BusinessUnit_Name { get; set; }

            public string Department_Code { get; set; }
            public string Department_Name { get; set; }

            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }

            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }

            public string Location_Code { get; set; }
            public string Location_Name { get; set; }

            public Guid? Requestor_By { get; set; }
            public string Requestor_Name { get; set; }



            public List<GetOpenTicketCategory> GetOpenTicketCategories { get; set; }

            public class GetOpenTicketCategory
            {
                public int? TicketCategoryId { get; set; }
                public int? CategoryId { get; set; }
                public string Category_Description { get; set; }

            }

            public List<GetOpenTicketSubCategory> GetOpenTicketSubCategories { get; set; }

            public class GetOpenTicketSubCategory
            {
                public int? TicketSubCategoryId { get; set; }
                public int? SubCategoryId { get; set; }
                public string SubCategory_Description { get; set; }
            }


            public DateTime? Date_Needed { get; set; }
            public string Notes { get; set; }
            public string Contact_Number { get; set; }
            public string Request_Type { get; set; }
            public int? BackJobId { get; set; }
            public string Back_Job_Concern { get; set; }

            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public Guid? UserId { get; set; }
            public string Issue_Handler { get; set; }
            public DateTime? Target_Date { get; set; }

            public string Ticket_Status { get; set; }
            public string Concern_Type { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public bool IsActive { get; set; }
            public string Remarks { get; set; }
            public bool? Done { get; set; }

            public bool? Is_Transfer { get; set; }
            public DateTime? Transfer_At { get; set; }
            public bool? Is_Closed { get; set; }
            public DateTime? Closed_At { get; set; }

            public string Closed_Status { get; set; }

            public List<GetForClosingTicket> GetForClosingTickets { get; set; }
            public List<GetForTransferTicket> GetForTransferTickets { get; set; }

            //public List<TransferApprovalTicket> TransferApprovalTickets { get; set; }

            public List<GetForOnHold> GetForOnHolds { get; set; }
            public List<GetOnHold> GetOnHolds { get; set; }

            public class GetForClosingTicket
            {
                public int? ClosingTicketId { get; set; }
                public string Resolution { get; set; }
                public List<ForClosingTicketTechnician> ForClosingTicketTechnicians { get; set; }
                public class ForClosingTicketTechnician
                {
                    public int? TicketTechnicianId { get; set; }
                    public Guid? Technician_By { get; set; }
                    public string Fullname { get; set; }
                }

                public List<GetForClosingTicketCategory> GetForClosingTicketCategories { get; set; }

                public class GetForClosingTicketCategory
                {
                    public int? TicketCategoryId { get; set; }
                    public int? CategoryId { get; set; }
                    public string Category_Description { get; set; }

                }

                public List<GetForClosingTicketSubCategory> GetForClosingTicketSubCategories { get; set; }

                public class GetForClosingTicketSubCategory
                {
                    public int? TicketSubCategoryId { get; set; }
                    public int? SubCategoryId { get; set; }
                    public string SubCategory_Description { get; set; }
                }

                public string Notes { get; set; }
                public string Remarks { get; set; }
                public bool? IsApprove { get; set; }
                public string Approver { get; set; }

                public List<ApproverList> ApproverLists { get; set; }

                public class ApproverList
                {
                    public string ApproverName { get; set; }
                    public int? Approver_Level { get; set; }
                }

                public List<GetAttachmentForClosingTicket> GetAttachmentForClosingTickets { get; set; }
                public class GetAttachmentForClosingTicket
                {
                    public int? TicketAttachmentId { get; set; }
                    public string Attachment { get; set; }
                    public string FileName { get; set; }
                    public decimal? FileSize { get; set; }
                }

            }

            public class GetForTransferTicket
            {
                public int? TransferTicketConcernId { get; set; }
                
                public Guid? Transfer_To { get; set; }

                public string Transfer_To_Name { get; set; }

                public string Transfer_Remarks { get; set; }

                public bool? IsApprove { get; set; }

                public DateOnly ? Target_Date { get; set; }

                public DateTime ? Current_Target_Date { get; set; }

                public List<GetAttachmentForTransferTicket> GetAttachmentForTransferTickets { get; set; }
                public class GetAttachmentForTransferTicket
                {
                    public int? TicketAttachmentId { get; set; }

                    public string Attachment { get; set; }

                    public string FileName { get; set; }

                    public decimal? FileSize { get; set; }
                }

            }

            //public class TransferApprovalTicket
            //{
            //    public int? TransferTicketConcernId { get; set; }
            //    public Guid? Transfer_By { get; set; }
            //    public string Transfer_By_Name { get; set; }
            //    public string Transfer_Remarks { get; set; }
            //    public bool? IsApprove { get; set; }

            //    public List<GetAttachmentTransferApprovalTicket> GetAttachmentTransferApprovalTickets { get; set; }
            //    public class GetAttachmentTransferApprovalTicket
            //    {
            //        public int? TicketAttachmentId { get; set; }
            //        public string Attachment { get; set; }
            //        public string FileName { get; set; }
            //        public decimal? FileSize { get; set; }
            //    }

            //}
            public class GetForOnHold
            {
                public int Id { get; set; }
                public string Reason { get; set; }
                public string AddedBy { get; set; }
                public DateTime CreatedAt { get; set; }
                public bool? IsHold { get; set; }
                public DateTime? ResumeAt { get; set; }
                public bool? IsApprove { get; set; }

                public List<GetAttachmentForOnHoldOpenTicket> GetAttachmentForOnHoldOpenTickets { get; set; }
                public class GetAttachmentForOnHoldOpenTicket
                {
                    public int? TicketAttachmentId { get; set; }
                    public string Attachment { get; set; }
                    public string FileName { get; set; }
                    public decimal? FileSize { get; set; }
                }


            }

            public class GetOnHold
            {
                public int Id { get; set; }
                public string Reason { get; set; }
                public string AddedBy { get; set; }
                public DateTime CreatedAt { get; set; }
                public bool? IsHold { get; set; }
                public DateTime? ResumeAt { get; set; }
                public bool? IsApprove { get; set; }

                public List<GetAttachmentOnHoldOpenTicket> GetAttachmentOnHoldOpenTickets { get; set; }
                public class GetAttachmentOnHoldOpenTicket
                {
                    public int? TicketAttachmentId { get; set; }
                    public string Attachment { get; set; }
                    public string FileName { get; set; }
                    public decimal? FileSize { get; set; }
                }


            }

           


            public DateTime ? Transaction_Date { get; set; }

            public int? Aging_Days { get; set; }


            //public List<GetApprovalTargetDate> GetApprovalTargetDates { get; set; }

            //public List<GetForApprovalTicketCategory> GetForApprovalTicketCategories { get; set; }

            //public class GetForApprovalTicketCategory
            //{
            //    public int? TicketCategoryId { get; set; }
            //    public int? CategoryId { get; set; }
            //    public string Category_Description { get; set; }

            //}

            //public List<GetForApprovalTicketSubCategory> GetForApprovalTicketSubCategories { get; set; }

            //public class GetForApprovalTicketSubCategory
            //{
            //    public int? TicketSubCategoryId { get; set; }
            //    public int? SubCategoryId { get; set; }
            //    public string SubCategory_Description { get; set; }
            //}

            //public string Approver { get; set; }

            //public List<ApproverListTargetDate> ApproverListTargetDates { get; set; }

            //public class ApproverListTargetDate
            //{
            //    public string ApproverName { get; set; }
            //    public int? Approver_Level { get; set; }
            //}
        }
    }
}
