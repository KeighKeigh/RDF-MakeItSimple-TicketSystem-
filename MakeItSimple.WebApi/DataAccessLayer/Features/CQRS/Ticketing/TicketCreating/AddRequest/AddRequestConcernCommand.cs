﻿using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest
{
    public partial class AddRequestConcern
    {
        public class AddRequestConcernCommand : IRequest<Result>
        {

            public int? TicketConcernId { get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public int? RequestConcernId { get; set; }
            public int? CompanyId { get; set; }
            public int? BusinessUnitId { get; set; }
            public int? DepartmentId { get; set; }
            public int? UnitId { get; set; }
            public int? SubUnitId { get; set; }
            public string Location_Code { get; set; }
            public int? ChannelId { get; set; }
            public DateTime? TargetDate { get; set; }
            public Guid? AssignTo { get; set; }
    
            public Guid? RequestorBy { get; set; }
            public string OneChargingCode { get; set; }
            public string OneChargingName { get; set; }

            

            public List<AddRequestTicketCategory> AddRequestTicketCategories { get; set; }
            public class AddRequestTicketCategory
            {
                public int? TicketCategoryId { get; set; }
                public int? CategoryId { get; set; }

            }
            public List<AddRequestTicketSubCategory> AddRequestTicketSubCategories { get; set; }
            public class AddRequestTicketSubCategory
            {
                public int? TicketSubCategoryId { get; set; }
                public int? SubCategoryId { get; set; }

            }

            public DateTime? DateNeeded { get; set; }
            public Guid? UserId { get; set; }

            
            public List<ListOfConcern> ListOfConcerns { get; set; }
            public class ListOfConcern
            {
                public string Concern { get; set; }
            }
            public string Remarks { get; set; }
            public string Notes { get; set; }
            public string Contact_Number { get; set; }
            public string Request_Type { get; set; }
            public string Severity { get; set; }

            public int ? BackJobId { get; set; }

            public List<RequestAttachmentsFile> RequestAttachmentsFiles { get; set; }

            public class RequestAttachmentsFile
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }
            public int? ServiceProviderId { get; set; }


        }
    }
}
