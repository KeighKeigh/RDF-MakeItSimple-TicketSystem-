namespace MakeItSimple.WebApi.Models.Setup.Phase_One.ApproverUsersSetup
{
    public class ApproverUser
    {
        public int Id { get; set; }

        public Guid? ApproverId { get; set; }
        public virtual User Approver { get; set; }

        public Guid? UserId { get; set; }
        public virtual User User { get; set; }


        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
