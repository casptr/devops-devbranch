using Domain.Customers;
using Domain.Formulas;
using Domain.Quotations;
using Domain.Supplements;
using Microsoft.EntityFrameworkCore;

namespace Foodtruck.Persistence
{
    public class FoodtruckDbContext : DbContext
    {
        public DbSet<MarketingSubscriber> MarketingSubscribers { get; set; }

        public DbSet<Formula> Formulas => Set<Formula>();
        public DbSet<FormulaSupplementLine> FormulaSupplementLines => Set<FormulaSupplementLine>();
        public DbSet<FormulaSupplementChoice> FormulaSupplementChoices => Set<FormulaSupplementChoice>();
        public DbSet<Domain.Formulas.Foodtruck> Foodtruck => Set<Domain.Formulas.Foodtruck>();
        public DbSet<PricePerDayLine> PricePerDayLines => Set<PricePerDayLine>();

        public DbSet<Supplement> Supplements => Set<Supplement>();
        public DbSet<Category> Category => Set<Category>();

        public DbSet<Quotation> Quotations => Set<Quotation>();
        public DbSet<QuotationSupplementLine> QuotationSupplementLines => Set<QuotationSupplementLine>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<QuotationVersion> QuotationVersions => Set<QuotationVersion>();
        public DbSet<SupplementImage> SupplementImages => Set<SupplementImage>();

        public FoodtruckDbContext(DbContextOptions<FoodtruckDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FoodtruckDbContext).Assembly);

        }
    }
}
