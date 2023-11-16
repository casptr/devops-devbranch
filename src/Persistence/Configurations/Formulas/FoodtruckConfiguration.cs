using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Foodtruck.Persistence.Configurations.Formulas;

internal class FoodtruckConfiguration : IEntityTypeConfiguration<Domain.Formulas.Foodtruck>
{
	public void Configure(EntityTypeBuilder<Domain.Formulas.Foodtruck> builder)
	{
		builder.OwnsOne(x => x.ExtraPricePerDay).Property(x => x.Value).HasColumnName(nameof(Domain.Formulas.Foodtruck.ExtraPricePerDay));
	}
}
