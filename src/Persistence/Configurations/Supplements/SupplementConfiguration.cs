using Domain.Supplements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Foodtruck.Persistence.Configurations.Supplements;

internal class SupplementConfiguration : IEntityTypeConfiguration<Supplement>
{
    public void Configure(EntityTypeBuilder<Supplement> builder)
    {
        builder.OwnsOne(x => x.Price).Property(x => x.Value).HasColumnName(nameof(Supplement.Price));
        builder.HasMany(x => x.ImageUrls).WithOne(x => x.Supplement).OnDelete(DeleteBehavior.Cascade);
    }
}