using Foodtruck.Persistence.Triggers;
using FluentValidation;
using FluentValidation.AspNetCore;
using Foodtruck.Client.BlobFiles;
using Foodtruck.Persistence;
using Foodtruck.Server.Authentication;
using Foodtruck.Server.Middleware;
using Foodtruck.Shared.Formulas;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistence;
using SendGrid.Extensions.DependencyInjection;
using Services;
using Services.Quotations.Handlers;

namespace Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<FoodtruckDbContext>(options =>
            {
                var conStrBuilder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("Foodtruck"))
                {
                    UserID = Configuration["MySql:User"],
                    Password = Configuration["MySql:Password"]
                };

                if (Configuration["MySql:Server"] != null)
                    conStrBuilder["Server"] = Configuration["MySql:Server"];

                var connectionString = conStrBuilder.ConnectionString;

                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions => mySqlOptions.EnableStringComparisonTranslations());

                if (Environment.IsDevelopment())
                {
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                    options.UseTriggers(options =>
                    {
                        options.AddTrigger<QuotationVersionBeforeSaveTrigger>();
                        options.AddTrigger<EntityBeforeSaveTrigger>();
                    });
                }

            });


            services.AddControllersWithViews();


            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<FormulaDto.Mutate.Validator>();

            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.DeclaringType is null ? $"{type.Name}" : $"{type.DeclaringType?.Name}.{type.Name}");
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Foodtruck API", Version = "v1" });
            }).AddFluentValidationRulesToSwagger();

            //// (Fake) Authentication
            services.AddAuthentication("Fake Authentication")
                            .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>("Fake Authentication", null);

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(options =>
            //{
            //    options.Authority = Configuration["Auth0:Authority"];
            //    options.Audience = Configuration["Auth0:ApiIdentifier"];
            //});

            //services.AddAuth0AuthenticationClient(config =>
            //{
            //    config.Domain = Configuration["Auth0:Authority"];
            //    config.ClientId = Configuration["Auth0:ClientId"];
            //    config.ClientSecret = Configuration["Auth0:ClientSecret"];
            //});

            //services.AddAuth0ManagementClient().AddManagementAccessToken();

            services.AddRazorPages();
            services.AddFoodtruckServices();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(QuotationCreatedAdminEmailHandler).Assembly));
            services.AddScoped<FoodTruckDataInitializer>();
            services.AddSendGrid(opt => opt.ApiKey = Configuration["SendGrid:ApiKey"]);
            services.AddHttpClient<IStorageService, AzureBlobStorageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, FoodTruckDataInitializer dataInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Foodtruck API"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            dataInitializer.SeedData();

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
