using Domain.Formulas;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodtruck.Persistence.Configurations.Formulas;

public class FormulaSupplementLineConfiguration : IEntityTypeConfiguration<FormulaSupplementLine>
{
    public void Configure(EntityTypeBuilder<FormulaSupplementLine> builder)
    {
        builder.HasOne(e => e.Supplement).WithMany().HasForeignKey("SupplementId");
        builder.Property(e => e.Quantity);
    }
}