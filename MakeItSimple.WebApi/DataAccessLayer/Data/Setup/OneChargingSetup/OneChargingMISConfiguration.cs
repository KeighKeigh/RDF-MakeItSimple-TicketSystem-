using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.OneCharging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.OneChargingSetup
{
    public class OneChargingMISConfiguration : IEntityTypeConfiguration<OneChargingMIS>
    {
        public void Configure(EntityTypeBuilder<OneChargingMIS> builder)
        {
            builder.HasKey(o => o.Id);
            builder.HasAlternateKey(o => o.code);

            // Configure relationship from this side
            builder.HasMany<User>() // Specify the dependent entity type
                   .WithOne(u => u.OneChargingMIS)
                   .HasForeignKey(u => u.OneChargingCode)
                   .HasPrincipalKey(o => o.code);
        }
    }
}
