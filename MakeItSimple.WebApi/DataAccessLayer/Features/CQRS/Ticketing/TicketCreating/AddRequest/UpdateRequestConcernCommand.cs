using MakeItSimple.WebApi.Common;
using MediatR;
using System.Text.Json.Serialization;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddRequest
{
    public partial class UpdateRequestConcern
    {
        public class UpdateRequestConcernCommand : IRequest<Result>
        {

            //public List<TicketConcernIdCherryList> ticketConcernIdCherryLists {  get; set; }
            //public class TicketConcernIdCherryList
            //{
            //    [JsonPropertyName("ticketConcernId")]
                public int? TicketConcernId { get; set; }


                //[JsonPropertyName("requestConcernId")]

                public int? RequestConcernId { get; set; }
            //}
            
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
           
            public int? ChannelId { get; set; }
            public DateTime? TargetDate { get; set; }
            public Guid? AssignTo { get; set; }

            public Guid? RequestorBy { get; set; }



            //public List<AddRequestTicketCategoriess> AddRequestTicketCategory { get; set; }
            //public class AddRequestTicketCategoriess
            //{
            //    public int? TicketCategoryId { get; set; }
            //    public int? CategoryId { get; set; }

            //}
            //public List<AddRequestTicketSubCategoriess> AddRequestTicketSubCategory { get; set; }
            //public class AddRequestTicketSubCategoriess
            //{
            //    public int? TicketSubCategoryId { get; set; }
            //    public int? SubCategoryId { get; set; }

            //}

            public DateTime? DateNeeded { get; set; }
            public Guid? UserId { get; set; }


            
            
            public string Concern { get; set; }
            
            public string Remarks { get; set; }
            public string Notes { get; set; }
            public string Contact_Number { get; set; }
            public string Request_Type { get; set; }
            public string Severity { get; set; }

            public int? BackJobId { get; set; }

            public List<RequestAttachmentsFilee> RequestAttachmentsFile { get; set; }

            public class RequestAttachmentsFilee
            {
                public int? TicketAttachmentId { get; set; }
                public IFormFile Attachment { get; set; }
            }
            public int? ServiceProviderId { get; set; }
        }
    }
}
