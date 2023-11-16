using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Formulas;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Foodtruck.Persistence.Configurations.Formulas;

internal class PricePerDayLineConfiguration : IEntityTypeConfiguration<PricePerDayLine>
{
	public void Configure(EntityTypeBuilder<PricePerDayLine> builder)
	{
		builder.OwnsOne(x => x.Price).Property(x => x.Value).HasColumnName(nameof(PricePerDayLine.Price));
	}
}