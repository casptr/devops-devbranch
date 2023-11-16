using Domain.Formulas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Foodtruck.Persistence.Configurations.Formulas;

public class FormulaSupplementChoiceConfiguration : IEntityTypeConfiguration<FormulaSupplementChoice>
{
    public void Configure(EntityTypeBuilder<FormulaSupplementChoice> builder)
    {
        builder.HasOne(e => e.DefaultChoice).WithMany(e => e.FormulaSupplementDefaultChoices).HasForeignKey("DefaultChoiceId");
        builder.HasMany(e => e.SupplementsToChoose).WithMany(e => e.FormulaSupplementChoices);
    }
}
