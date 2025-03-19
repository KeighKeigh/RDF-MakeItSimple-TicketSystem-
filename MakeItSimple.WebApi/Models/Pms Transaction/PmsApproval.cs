using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;

namespace MakeItSimple.WebApi.Models.Setup.FormSetup
{
    public class PmsApproval : BaseIdEntity
    {
        public Guid? UserId { get; set; }
        public virtual User User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public int? ApproverLevel { get; set; }
        public int? PmsId { get; set; }
        public virtual Pms Pms{ get; set; }

        public bool? IsApproved { get; set; }
        public bool IsDeleted { get; set; } = false;
        

    }
}
