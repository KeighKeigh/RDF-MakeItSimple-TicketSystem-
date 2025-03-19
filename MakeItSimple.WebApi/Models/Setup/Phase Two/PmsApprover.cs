using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;

namespace MakeItSimple.WebApi.Models.Setup.Phase_Two
{
    public class PmsApprover : BaseIdEntity
    {
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public int? PmsFormId { get; set; }
        public virtual PmsForm PmsForms { get; set; }
        public Guid? UserId { get; set; }
        public virtual User User { get; set; }
        public int? ApproverLevel { get; set; }

    }
}
