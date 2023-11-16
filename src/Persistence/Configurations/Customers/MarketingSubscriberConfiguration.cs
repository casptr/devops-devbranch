using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Customers;

namespace Foodtruck.Persistence.Configurations.Customers;

internal class MarketingSubscriberConfiguration : IEntityTypeConfiguration<MarketingSubscriber>
{
    public void Configure(EntityTypeBuilder<MarketingSubscriber> builder)
    {
        builder.OwnsOne(x => x.Email).Property(x => x.Value).HasColumnName(nameof(MarketingSubscriber.Email));
    }
}
