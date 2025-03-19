using MakeItSimple.WebApi.Models.Ticketing;

namespace MakeItSimple.WebApi.Models.Setup.FormSetup
{
    public class PmsHistory : BaseIdEntity
    {

        public string Status { get; set; }
        public string Request { get; set; }
        public Guid? TransactedBy { get; set; }
        public virtual User TransactedByUser { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string Remarks { get; set; }
        public int? Approver_Level { get; set; }
        public int? PmsId { get; set; }
        public virtual Pms Pms { get; set; }

        public bool IsDeleted { get; set; } = false;

    }
}
