using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Foodtruck.Client;
using MudBlazor.Services;
using Foodtruck.Shared.Formulas;
using Foodtruck.Client.Formulas;
using Foodtruck.Shared.Supplements;
using Foodtruck.Client.Supplements;
using Foodtruck.Client.Infrastructure;

using Foodtruck.Shared.Reservations;
using Foodtruck.Client.QuotationProcess.Helpers;
using Foodtruck.Client.QuotationProcess;

using Foodtruck.Client.Quotations;
using Foodtruck.Shared.Quotations;
using Foodtruck.Shared.Pdfs;
using Services.Pdfs;
using Foodtruck.Client.Admin;
using Foodtruck.Client.BlobFiles;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


// Auth0
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
});
// Old auth

// builder.Services.AddSingleton<FakeAuthenticationProvider>();
// builder.Services.AddAuthorizationCore();
// builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<FakeAuthenticationProvider>());
// builder.Services.AddTransient<FakeAuthorizationMessageHandler>();

builder.Services.AddTransient<CleanErrorHandler>();

builder.Services.AddHttpClient("Project.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CleanErrorHandler>();


builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Project.ServerAPI"));

builder.Services.AddScoped<IFormulaService, FormulaService>();
builder.Services.AddScoped<ISupplementService, SupplementService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IQuotationService, QuotationService>();
builder.Services.AddScoped<QuotationProcessState>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IStorageService, AzureBlobStorageService>();

builder.Services.AddMudServices();
await builder.Build().RunAsync();
