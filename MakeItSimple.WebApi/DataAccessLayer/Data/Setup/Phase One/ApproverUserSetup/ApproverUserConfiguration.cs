using MakeItSimple.WebApi.Models.Setup.Phase_One.ApproverUsersSetup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_One.ApproverUserSetup
{
    public class ApproverUserConfiguration : IEntityTypeConfiguration<ApproverUser>
    {

        public void Configure(EntityTypeBuilder<ApproverUser> builder)
        {
            builder.HasOne(u => u.Approver)
                .WithMany(x => x.ApproverUsers)
                .HasForeignKey(u => u.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.User)
                .WithMany(x => x.IssueHandlerUsers)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
