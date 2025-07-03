namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.OpenTicketConcern.GetOpenTicketSubUnit
{
    public partial class GetOpenTicketSubUnit
    {
        public class GetOpenTicketSubUnitResult
        {

            public int TicketConcernId { get; set; }
            public string Concern_Details { get; set; }
            public string Resolution { get; set; }
            public string Notes { get; set; }
            public int? DepartmentId { get; set; }
            public string Department_Name { get; set; }
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public Guid? UserId { get; set; }
            public string Fullname { get; set; }



            public List<GetOpenTicketSubUnitCategory> GetOpenTicketSubUnitCategories { get; set; }

            public class GetOpenTicketSubUnitCategory
            {
                public int? TicketCategoryId { get; set; }
                public int? CategoryId { get; set; }
                public string Category_Description { get; set; }

            }

            public List<GetOpenTicketSubUnitSubCategory> GetOpenTicketSubUnitSubCategories { get; set; }

            public class GetOpenTicketSubUnitSubCategory
            {
                public int? TicketSubCategoryId { get; set; }
                public int? SubCategoryId { get; set; }
                public string SubCategory_Description { get; set; }
            }

            public string SubCategoryDescription { get; set; }
            public int? Delay_Days { get; set; }
            public string Closed_By { get; set; }
            public DateTime? Closed_At { get; set; }
            public string Closed_Status { get; set; }
            public string Closed_Remarks { get; set; }
            public string RejectClosed_By { get; set; }
            public DateTime? RejectClosed_At { get; set; }
            public string Reject_Remarks { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public DateTime? Start_Date { get; set; }
            public DateTime? Target_Date { get; set; }

            public List<OpenTicketSubCategoryAttachment> OpenTicketSubCategoryAttachments { get; set; }

            public class OpenTicketSubCategoryAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public string Attachment { get; set; }
                public string FileName { get; set; }
                public decimal? FileSize { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }
            }
        }
    }
}
