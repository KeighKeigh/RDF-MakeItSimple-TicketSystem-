using MakeItSimple.WebApi.Models.Setup.FormSetup;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Pms_Transaction
{
    public class PmsApprovalConfiguration : IEntityTypeConfiguration<PmsApproval>
    {
        public void Configure(EntityTypeBuilder<PmsApproval> builder)
        {

            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(p => !p.IsDeleted);

        }
    }
}
