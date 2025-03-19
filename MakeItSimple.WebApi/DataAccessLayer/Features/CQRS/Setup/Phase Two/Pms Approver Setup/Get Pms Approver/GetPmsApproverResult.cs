namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Get_Pms_Approver
{
    public partial class GetPmsApprover
    {
        public record GetPmsApproverResult
        {

            public int PmsFormId { get; set; } 
            public string Form_Name { get; set; }

            public List<PmsUserApprover> PmsUserApprovers {  get; set; }

            public record PmsUserApprover
            {
                public int Id { get; set; }
                public Guid ? UserId { get; set; }
                public string Fullname { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set;}
                public bool Is_Active { get; set; }

            }

        }
    }
}
