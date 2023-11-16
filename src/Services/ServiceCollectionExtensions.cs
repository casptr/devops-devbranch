using Foodtruck.Shared.Emails;
using Foodtruck.Shared.Formulas;
using Foodtruck.Shared.Pdfs;
using Foodtruck.Shared.Quotations;
using Foodtruck.Shared.Reservations;
using Foodtruck.Shared.Supplements;
using Microsoft.Extensions.DependencyInjection;
using Services.BlobFiles;
using Services.Emails;
using Services.Formulas;
using Services.Pdfs;
using Services.Quotations;
using Services.Reservations;
using Services.Supplements;

namespace Services;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all services to the DI container.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddFoodtruckServices(this IServiceCollection services)
    {
        services.AddScoped<IFormulaService, FormulaService>();
        services.AddScoped<ISupplementService, SupplementService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IQuotationService, QuotationService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped<IStorageService, BlobStorageService>();
        // Add more services here...

        return services;
    }
}