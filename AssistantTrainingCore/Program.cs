using AssistantTrainingCore.Controllers;
using AssistantTrainingCore.Data;
using AssistantTrainingCore.Models;
using AssistantTrainingCore.Repositories;
using AssistantTrainingCore.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

ConfigureConfiguration(builder.Configuration);
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureMiddleware(app, app.Services, app.Environment);
ConfigureEndpoints(app, app.Services);

app.Run();

void ConfigureConfiguration(ConfigurationManager configuration) { };
void ConfigureServices(IServiceCollection services, ConfigurationManager configuratio)
{
    services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        configuratio.GetConnectionString("DefaultConnection")));

    services.AddIdentity<IdentityUser, IdentityRole>(
            options => {
                options.SignIn.RequireConfirmedAccount = false;

            //Other options go here
        }
            )
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        // Cookie settings
        options.Cookie.HttpOnly = true;
        //options.Cookie.Expiration

        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        //options.ReturnUrlParameter=""
    });

    services.AddTransient<IWorkerRepository, WorkerRepository>();

    services.Configure<RequestLocalizationOptions>(
    options =>
    {

        options.DefaultRequestCulture = new RequestCulture(culture: "pl-PL", uiCulture: "pl-PL");
        options.SupportedCultures = GetSupportedCultures();
        options.SupportedUICultures = GetSupportedCultures();
    });

    builder.Services.AddControllersWithViews()
                    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver())
                    .AddViewLocalization()
                   .AddDataAnnotationsLocalization(options =>
                   {
                       options.DataAnnotationLocalizerProvider = (type, factory) =>
                       {
                           var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                           return factory.Create("SharedResource", assemblyName.Name);
                       };
                   });
    builder.Services.AddKendo();
};
void ConfigureMiddleware(IApplicationBuilder app, IServiceProvider services, IWebHostEnvironment environment)
{
    if (!environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    var requestLocalizationOptions = new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("pl-PL"),
        SupportedCultures = GetSupportedCultures(),
        SupportedUICultures = GetSupportedCultures(),
    };


    requestLocalizationOptions.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());
    app.UseRequestLocalization(requestLocalizationOptions);

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(name: "default", pattern: "{culture=pl-PL}/{controller=Home}/{action=Index}/{id?}");
    });
};
void ConfigureEndpoints(IEndpointRouteBuilder app, IServiceProvider services)
{
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
};

static List<CultureInfo> GetSupportedCultures()
{
    return new List<CultureInfo>
            {
                    new CultureInfo("en-US"),
                    new CultureInfo("pl-PL"),

            };
}