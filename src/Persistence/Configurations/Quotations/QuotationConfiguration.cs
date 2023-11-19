using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Quotations;
using System.Reflection.Metadata;
using Domain.Customers;

namespace Foodtruck.Persistence.Configurations.Quotations
{
    internal class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
    {
        public void Configure(EntityTypeBuilder<Quotation> builder)
        {

            builder.OwnsOne(x => x.Customer, customer =>
            {
                customer.OwnsOne(x => x.Email).Property(x => x.Value).HasColumnName(nameof(Customer.Email));
            });

            builder.OwnsOne(x => x.Price).Property(x => x.Value).HasColumnName(nameof(Quotation.Price));
            builder.OwnsOne(x => x.VatTotal).Property(x => x.Value).HasColumnName(nameof(Quotation.VatTotal));
            builder.OwnsOne(x => x.FoodtruckPrice).Property(x => x.Value).HasColumnName(nameof(Quotation.FoodtruckPrice));

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

            builder.HasMany(x => x.QuotationSupplementLines).WithOne().HasForeignKey(x => x.QuotationId).IsRequired();

            builder.HasMany(x => x.PreviousVersions).WithOne(x => x.MostRecentVersion);
        }
    }
}
