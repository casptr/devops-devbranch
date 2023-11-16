using Domain.Formulas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Foodtruck.Persistence.Configurations.Formulas;

internal class FormulaConfiguration : IEntityTypeConfiguration<Formula>
{
    public void Configure(EntityTypeBuilder<Formula> builder)
    {
        builder.HasMany(e => e.Choices).WithMany(e => e.Formulas);
        builder.HasOne(e => e.Foodtruck).WithMany().HasForeignKey("FoodtruckId");
    }
}