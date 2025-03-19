using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;

namespace MakeItSimple.WebApi.Models.Setup.FormSetup
{
    public class Pms : BaseIdEntity
    {

        public int? PmsFormId { get; set; }
        public virtual PmsForm PmsForm { get; set; }

        public Guid? Requestor { get; set; }
        public virtual User RequestorByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool IsApproved { get; set; } = false;
        public bool? IsRejected { get; set; } = false;


        public ICollection<PmsDetail> PmsDetails { get; set; }
        public ICollection<PmsApproval> PmsApprovals { get; set; } 
        public ICollection<PmsAttachment> PmsAttachments { get; set; }
        public ICollection<PmsTechnician> PmsTechnicians { get; set; }
        public ICollection<PmsHistory> PmsHistories { get; set; }
  
    }
}
