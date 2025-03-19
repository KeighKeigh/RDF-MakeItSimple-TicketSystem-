using MakeItSimple.WebApi.Models.Setup.FormSetup;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Pms_Transaction
{
    public class PmsHistoryConfiguration : IEntityTypeConfiguration<PmsHistory>
    {
        public void Configure(EntityTypeBuilder<PmsHistory> builder)
        {

            builder.HasOne(u => u.TransactedByUser)
           .WithMany()
           .HasForeignKey(u => u.TransactedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);

        }
    }
}
