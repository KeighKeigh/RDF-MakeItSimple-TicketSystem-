using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two
{
    public class QuestionTransactionIdConfiguration : IEntityTypeConfiguration<QuestionTransactionId>
    {
        public void Configure(EntityTypeBuilder<QuestionTransactionId> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);

        }
    }
}
