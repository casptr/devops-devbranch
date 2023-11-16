using Domain.Quotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Foodtruck.Persistence.Configurations.QuotationsVersion;

internal class QuotationVersionConfiguration : IEntityTypeConfiguration<QuotationVersion>
{
    public void Configure(EntityTypeBuilder<QuotationVersion> builder)
    {
        builder.OwnsOne(x => x.Price).Property(x => x.Value).HasColumnName(nameof(QuotationVersion.Price));
        builder.OwnsOne(x => x.VatTotal).Property(x => x.Value).HasColumnName(nameof(QuotationVersion.VatTotal));
        builder.OwnsOne(x => x.FoodtruckPrice).Property(x => x.Value).HasColumnName(nameof(QuotationVersion.FoodtruckPrice));

        builder.OwnsOne(x => x.EventAddress, address =>
        {
            // Without this mapping EF Core does not save the properties since they're getters only.
            // This can be omitted by making them private set, but then you're lying to the domain model.
            address.Property(x => x.Zip);
            address.Property(x => x.City);
            address.Property(x => x.Street);
            address.Property(x => x.HouseNumber);
        });

        builder.OwnsOne(x => x.BillingAddress, address =>
        {
            // Without this mapping EF Core does not save the properties since they're getters only.
            // This can be omitted by making them private set, but then you're lying to the domain model.
            address.Property(x => x.Zip);
            address.Property(x => x.City);
            address.Property(x => x.Street);
            address.Property(x => x.HouseNumber);
        });

        builder.HasMany(x => x.QuotationSupplementLines).WithOne().HasForeignKey(x => x.QuotationVersionId).IsRequired();

    }
}