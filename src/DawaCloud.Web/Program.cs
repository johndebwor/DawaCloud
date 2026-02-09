using DawaCloud.Web.Components;
using DawaCloud.Web.Data;
using DawaCloud.Web.Features.Auth;
using DawaCloud.Web.Features.Currency.Services;
using DawaCloud.Web.Features.Wholesale.Services;
using DawaCloud.Web.Infrastructure.BackgroundServices;
using DawaCloud.Web.Infrastructure.Data;
using DawaCloud.Web.Infrastructure.Identity;
using DawaCloud.Web.Infrastructure.Middleware;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using QuestPDF.Infrastructure;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure QuestPDF License (Community Edition)
try { QuestPDF.Settings.License = LicenseType.Community; } catch { /* unsupported runtime (e.g. win-arm64) */ }

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .WriteTo.Console()
    .Enrich.FromLogContext());

// Add services to the container.

// HTTP Context Accessor
builder.Services.AddHttpContextAccessor();

// Audit Interceptor
builder.Services.AddScoped<AuditSaveChangesInterceptor>();

// Database - Transient lifetime prevents Blazor Server concurrent DbContext access errors
// Each component/handler gets its own DbContext instance
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
    .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()),
    ServiceLifetime.Transient);

// Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password Policy
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
    options.Password.RequiredUniqueChars = 4;

    // Lockout Policy
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User Policy
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddClaimsPrincipalFactory<AppUserClaimsPrincipalFactory>();

// Cookie Configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    options.AccessDeniedPath = "/auth/access-denied";
});

// Authorization Policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdmin", policy => policy.RequireRole("SuperAdmin"))
    .AddPolicy("RequireInventoryAccess", policy => policy.RequireRole("SuperAdmin", "InventoryManager"))
    .AddPolicy("RequireSalesAccess", policy => policy.RequireRole("SuperAdmin", "Pharmacist", "Cashier"))
    .AddPolicy("RequireFinanceAccess", policy => policy.RequireRole("SuperAdmin", "Accountant"));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Mapster
builder.Services.AddMapster();

// MudBlazor
builder.Services.AddMudServices();

// Memory Cache for currency service
builder.Services.AddMemoryCache();

// Currency Service
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IRoundingService, RoundingService>();

// Auth Service
builder.Services.AddScoped<DawaCloud.Web.Features.Auth.Services.IAuthService, DawaCloud.Web.Features.Auth.Services.AuthService>();

// Invoice Service
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// Background Services
builder.Services.AddHostedService<ExpiryAlertService>();
builder.Services.AddHostedService<LowStockAlertService>();

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Apply migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully");

        // Seed Identity (roles and users)
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        logger.LogInformation("Seeding Identity (roles and users)...");
        await IdentitySeeder.SeedAsync(userManager, roleManager);
        logger.LogInformation("Identity seeding completed successfully");

        logger.LogInformation("Seeding database with sample data...");
        await DbSeeder.SeedAsync(context);
        logger.LogInformation("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying database migrations or seeding");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy",
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
    await next();
});

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Auth API Endpoints
app.MapAuthEndpoints();

app.Run();
