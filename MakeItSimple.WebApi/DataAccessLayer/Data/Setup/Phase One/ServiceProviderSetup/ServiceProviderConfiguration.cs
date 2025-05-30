using MakeItSimple.WebApi.Models.Setup.Phase_One.ServiceProviderSetup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_One.ServiceProviderSetup
{
    public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProviders>
    {
        public void Configure(EntityTypeBuilder<ServiceProviders> builder)
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
