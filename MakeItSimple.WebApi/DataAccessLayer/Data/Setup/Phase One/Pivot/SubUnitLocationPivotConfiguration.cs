using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.Pivot;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Pivot
{
    public class SubUnitLocationConfiguration : IEntityTypeConfiguration<SubUnitLocationPivot>
    {
        public void Configure(EntityTypeBuilder<SubUnitLocationPivot> builder)
        {
            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(u => u.ModifiedByUser)
           .WithMany()
           .HasForeignKey(u => u.ModifiedBy)
           .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
