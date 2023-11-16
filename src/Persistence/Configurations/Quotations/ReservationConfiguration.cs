using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Quotations;

namespace Foodtruck.Persistence.Configurations.Quotations;

internal class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasOne(x => x.QuotationVersion).WithOne(x => x.Reservation).HasForeignKey<QuotationVersion>("ReservationId");
    }

}
