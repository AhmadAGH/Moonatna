using DbUp;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Moonatna.Repositories.Families;
using Moonatna.Repositories.Items;
using Moonatna.Repositories.Lookups;
using Moonatna.Repositories.Recipes;
using Moonatna.Repositories.SqlConnectionFactory;
using Moonatna.Repositories.Users;
using Moonatna.Services.Families;
using Moonatna.Services.Items;
using Moonatna.Services.Localization;
using Moonatna.Services.Recipes;
using Moonatna.Services.Users;
using Serilog;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration)
          .WriteTo.Seq(context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341"));

// ============ Firebase Admin SDK ============
// Verifies ID tokens posted to /Auth/Token. The service-account JSON path comes
// from config and the file must never be committed (gitignore: *firebase-adminsdk*.json).
var firebaseCredentialsPath = builder.Configuration["Firebase:CredentialsPath"]
    ?? throw new InvalidOperationException("Firebase:CredentialsPath is not configured.");

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(firebaseCredentialsPath)
});

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });

// Repositories and services get registered here as we build each feature.
builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IFamiliesRepository, FamiliesRepository>();
builder.Services.AddScoped<IItemsRepository, ItemsRepository>();
builder.Services.AddScoped<IRecipesRepository, RecipesRepository>();
builder.Services.AddScoped<ILookupsRepository, LookupsRepository>();

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IFamiliesService, FamiliesService>();
builder.Services.AddScoped<IItemsService, ItemsService>();
builder.Services.AddScoped<IRecipesService, RecipesService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();

var app = builder.Build();

// ============ Database migrations (DbUp) ============
var connectionString = app.Configuration.GetConnectionString("DefaultConnection");

EnsureDatabase.For.SqlDatabase(connectionString);

var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    throw new InvalidOperationException("Database migration failed.", result.Error);
}

var supportedCultures = new[] { new CultureInfo("ar-SA"), new CultureInfo("en") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ar-SA"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();