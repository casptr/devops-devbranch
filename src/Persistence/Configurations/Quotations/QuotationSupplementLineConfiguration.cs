using Domain.Quotations;
using Domain.Supplements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Foodtruck.Persistence.Configurations.Quotations;

internal class QuotationSupplementLineConfiguration : IEntityTypeConfiguration<QuotationSupplementLine>
{
	public void Configure(EntityTypeBuilder<QuotationSupplementLine> builder)
	{
		builder.OwnsOne(x => x.SupplementPrice).Property(x => x.Value).HasColumnName(nameof(QuotationSupplementLine.SupplementPrice));
        builder.OwnsOne(x => x.SupplementVat).Property(x => x.Value).HasColumnName(nameof(QuotationSupplementLine.SupplementVat));
        builder.Property(x => x.Quantity);
        builder.Property(x => x.Description);
        builder.Property(x => x.Name);
    }
}