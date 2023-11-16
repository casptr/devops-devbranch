using Domain.Common;
using Domain.Quotations;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodtruck.Persistence.Triggers;

public class QuotationVersionBeforeSaveTrigger : IBeforeSaveTrigger<QuotationVersion>
{
    private readonly FoodtruckDbContext dbContext;

    public QuotationVersionBeforeSaveTrigger(FoodtruckDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task BeforeSave(ITriggerContext<QuotationVersion> context, CancellationToken cancellationToken)
    {
        // TODO review this code, maybe there is a better way to copy the quotation on update
        if (context.ChangeType == ChangeType.Modified)
        {
            context.Entity.Id = 0;
            dbContext.Entry(context.Entity).State = EntityState.Added;

            dbContext.Entry(context.Entity.Price).State = EntityState.Added;
            dbContext.Entry(context.Entity.VatTotal).State = EntityState.Added;
            dbContext.Entry(context.Entity.FoodtruckPrice).State = EntityState.Added;

            dbContext.Entry(context.Entity.BillingAddress).State = EntityState.Added;
            dbContext.Entry(context.Entity.EventAddress).State = EntityState.Added;

            context.Entity.Reservation.Id = 0;
            dbContext.Entry(context.Entity.Reservation).State = EntityState.Added;

            context.Entity.VersionNumber = context.Entity.VersionNumber + 1;

            context.Entity.QuotationSupplementLines.ToList().ForEach(line =>
            {
                line.Id = 0;
                dbContext.Entry(line).State = EntityState.Added;
                dbContext.Entry(line.SupplementPrice).State = EntityState.Added;
                dbContext.Entry(line.SupplementVat).State = EntityState.Added;
            });
        }
        return Task.CompletedTask;
    }
}
