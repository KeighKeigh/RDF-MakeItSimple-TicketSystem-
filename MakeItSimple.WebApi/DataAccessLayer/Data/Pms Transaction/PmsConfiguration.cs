using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.FormSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.DataContext
{
    public class PmsConfiguration : IEntityTypeConfiguration<Pms>
    {
        public void Configure(EntityTypeBuilder<Pms> builder)
        {

            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ModifiedByUser)
           .WithMany()
           .HasForeignKey(u => u.ModifiedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.RequestorByUser)
           .WithMany()
           .HasForeignKey(u => u.Requestor)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(p => !p.IsDeleted);

        }
    }
}
