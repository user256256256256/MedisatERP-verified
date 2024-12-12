using MedisatERP.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register MedisatErpDbContext for your application data
builder.Services.AddDbContext<MedisatErpDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MedisatConnection")));

// Register ApplicationDbContext for Identity data
builder.Services.AddDbContext<ApplicationDbContext>(options =>
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

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    // Lockout settings
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

//  Register HttpClient as a service
builder.Services.AddHttpClient(); // Registers the HttpClient service globally

// Add services to the container.
builder.Services
    .AddRazorPages()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

// Register Areas
builder.Services.AddControllersWithViews();

// Create the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


// Add this line to enable areas support
app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Maps routes for the "NutritionCompany" area, allowing a specific controller and action.
// The {companyId:guid?} is an optional GUID parameter for company-specific routing.
app.MapAreaControllerRoute(
    name: "NutritionCompany",
    areaName: "NutritionCompany",
    pattern: "NutritionCompany/{controller=Home}/{action=Index}/{companyId:guid?}");

// Specifically routes to the "NutritionSystem" controller within the "NutritionCompany" area.
// The {companyId:guid?} is a required GUID parameter for company-specific routing.
app.MapAreaControllerRoute(
    name: "nutritionSystemRoute",
    areaName: "NutritionCompany",
    pattern: "NutritionCompany/{controller=NutritionSystem}/{action=Index}/{companyId:guid}");

// Map the default controller route (for non-area routes)
app.MapDefaultControllerRoute();

// Map Razor Pages
app.MapRazorPages();

//Hashing Password 
//var password = "@Lyfex?";
//var passwordHasher = new PasswordHasher<object>();
//var hashedPassword = passwordHasher.HashPassword(null, password);
//Console.WriteLine("Hashed Password " + hashedPassword);

app.Run();
