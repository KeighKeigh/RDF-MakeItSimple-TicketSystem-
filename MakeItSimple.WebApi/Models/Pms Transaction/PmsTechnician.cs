using MakeItSimple.WebApi.Models.Ticketing;

namespace MakeItSimple.WebApi.Models.Setup.FormSetup
{
    public class PmsTechnician : BaseIdEntity
    {

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }
        public int? PmsId { get; set; }
        public virtual Pms Pms { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
