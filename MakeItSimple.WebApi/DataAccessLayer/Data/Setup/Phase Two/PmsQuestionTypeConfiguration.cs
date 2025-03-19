using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two
{
    public class PmsQuestionTypeConfiguration : IEntityTypeConfiguration<PmsQuestionType>
    {
        public void Configure(EntityTypeBuilder<PmsQuestionType> builder)
        {
            builder.HasQueryFilter(x => !x.Is_Deleted);

        }
    }
}
