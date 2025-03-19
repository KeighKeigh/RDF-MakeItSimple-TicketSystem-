using MakeItSimple.WebApi.Models.Setup.FormSetup;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Pms_Transaction
{
    public class PmsTechnicianConfiguration : IEntityTypeConfiguration<PmsTechnician>
    {
        public void Configure(EntityTypeBuilder<PmsTechnician> builder)
        {
            builder.HasQueryFilter(p => !p.IsDeleted);

        }
    }
}
