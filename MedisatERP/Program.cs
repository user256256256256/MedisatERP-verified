
using MedisatERP.Data;
using MedisatERP.Hubs;
using MedisatERP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register MedisatErpDbContext for your application data
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MedisatConnection")));


// Register ApplicationDbContext for Identity data
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MedisatConnection")));

// Configure and Register IdentityFramework
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configure password settings
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Configure user settings
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    // Lockout settings
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true; // Added to align with Medisat
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

// Register the PhoneNumberNormalizationService as a singleton or transient service
builder.Services.AddTransient<IPhoneNumberNormalizationService, PhoneNumberNormalizationService>();

// Register RoleRedirectService 
builder.Services.AddTransient<IRoleRedirectService, RoleRedirectService>();

// Register UserSessionService
builder.Services.AddTransient<IUserSessionService, UserSessionService>();

// Register EmailSender and SmsSender services
builder.Services.AddTransient<IEmailSender, EmailSenderService>();

// Register ErrorCodeService
builder.Services.AddSingleton<IErrorCodeService, ErrorCodeService>();

// Register the validation service
builder.Services.AddScoped<IValidationService, ValidationService>();

// Register NotificationService
builder.Services.AddTransient<NotificationService>();

// Register HandelRoleRedirectService
builder.Services.AddTransient<HandelRoleRedirectService>();

// Register ExceptionHandlerService as a service
builder.Services.AddScoped<ExceptionHandlerService>();

// Register 2fa services
builder.Services.AddScoped<TwoFactorService>();

// Register DecodingUrl as a service
builder.Services.AddScoped<UserService>();

// Register SessionHelper as a service
builder.Services.AddScoped<ValidateSessionService>();

// Add IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add SignalR services
builder.Services.AddSignalR();

// Add services to the container
builder.Services
    .AddRazorPages()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

// Add Session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseSession();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// Map the API controller route for the LogoutAPI
app.MapControllerRoute(
    name: "LogoutAPI",
    pattern: "api/{controller}/{action}");

// Map SignalR hub
/* The app.MapHub<NotificationHub>("/notificationHub") line is crucial for defining the route that clients will use to connect to your SignalR hub. This setup is what allows your application to support real-time notifications and communication. */
app.MapHub<NotificationHub>("/notificationHub");

// Map Razor Pages
app.MapRazorPages();

app.Run();
