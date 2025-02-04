using MedisatERP.Areas.AdministratorSystem.Models;
using MedisatERP.Data;
using MedisatERP.Hubs;
using MedisatERP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register MedisatErpDbContext for your application data
builder.Services.AddDbContext<AdministratorSystemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MedisatConnection")));

// Register MedisatErpDbContext for your application data
builder.Services.AddDbContext<NutritionSystemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MedisatConnection")));

// Register MedisatErpDbContext for your application data
builder.Services.AddDbContext<SharedDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MedisatConnection")));

// Register ApplicationDbContext for Identity data
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MedisatConnection")));

// Configure and Register IdentityFramework
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configure password settings
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // Configure user settings
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    // Lockout settings
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    // Configure token providers
    options.Tokens.EmailConfirmationTokenProvider = "Default";
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
})
.AddEntityFrameworkStores<UserDbContext>()
.AddDefaultTokenProviders();

// Configure the token lifespan for all token providers
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
    opt.TokenLifespan = TimeSpan.FromMinutes(5));  // Ensures 5-minute expiry for tokens

// Add Session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register EmailSender and SmsSender services
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Register RoleRedirectService
builder.Services.AddTransient<RoleRedirectService>();

// Register NotificationService
builder.Services.AddTransient<NotificationService>();

// Register HttpClient as a service
builder.Services.AddHttpClient();

// Register ErrorCodeService
builder.Services.AddSingleton<IErrorCodeService, ErrorCodeService>();

// Configure SignalR
builder.Services.AddSignalR();

// Add services to the container
builder.Services.AddRazorPages().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddControllersWithViews();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Create the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Enable session middleware
app.UseSession();

// Enable areas support
app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Map the API controller route for the LogoutAPI
app.MapControllerRoute(
    name: "LogoutAPI",
    pattern: "api/{controller}/{action}");

// Map routes for the "NutritionCompanySystem" area
app.MapAreaControllerRoute(
    name: "NutritionCompanySystem",
    areaName: "NutritionCompanySystem",
    pattern: "NutritionCompanySystem/{controller=Home}/{action=Index}/{userId}/{companyId:guid?}");

// Specifically routes to the "CrmDashboard" controller within the "NutritionCompanySystem" area
app.MapAreaControllerRoute(
    name: "nutritionSystemRoute",
    areaName: "NutritionCompanySystem",
    pattern: "NutritionCompanySystem/{controller=CrmDashboard}/{action=Index}/{userId}/{companyId:guid}");

// Map the default controller route (for non-area routes)
app.MapDefaultControllerRoute();

// Map SignalR hub
/* The app.MapHub<NotificationHub>("/notificationHub") line is crucial for defining the route that clients will use to connect to your SignalR hub. This setup is what allows your application to support real-time notifications and communication. */
app.MapHub<NotificationHub>("/notificationHub");

// Map Razor Pages
app.MapRazorPages();

app.Run();
