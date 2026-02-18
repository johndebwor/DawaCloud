using DawaCloud.Web.Components;
using DawaCloud.Web.Data;
using DawaCloud.Web.Features.Auth;
using DawaCloud.Web.Features.Currency.Services;
using DawaCloud.Web.Features.Chat.Services;
using DawaCloud.Web.Features.Subscription;
using DawaCloud.Web.Features.Subscription.Services;
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
QuestPDF.Settings.License = LicenseType.Community;

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
        b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
              .EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null))
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
    options.Password.RequiredLength = 8;
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
    .AddPolicy("RequireFinanceAccess", policy => policy.RequireRole("SuperAdmin", "Accountant"))
    .AddPolicy("RequireHrAccess", policy => policy.RequireRole("SuperAdmin", "HrManager"));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Mapster
builder.Services.AddMapster();

// MudBlazor
builder.Services.AddMudServices();

// Localization
builder.Services.AddLocalization();

builder.Services.Configure<Microsoft.AspNetCore.Builder.RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new System.Globalization.CultureInfo("en-US"), // English (Default)
        new System.Globalization.CultureInfo("ar-SA"), // Arabic
        new System.Globalization.CultureInfo("fr-FR"), // French
        new System.Globalization.CultureInfo("de-DE"), // German
        new System.Globalization.CultureInfo("pt-PT"), // Portuguese
        new System.Globalization.CultureInfo("es-ES")  // Spanish
    };

    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // Culture providers (order matters)
    options.RequestCultureProviders = new List<Microsoft.AspNetCore.Localization.IRequestCultureProvider>
    {
        new Microsoft.AspNetCore.Localization.CookieRequestCultureProvider(),     // User preference cookie
        new Microsoft.AspNetCore.Localization.AcceptLanguageHeaderRequestCultureProvider()      // Browser language
    };
});

// Memory Cache for currency service
builder.Services.AddMemoryCache();

// Currency Service
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IRoundingService, RoundingService>();

// Localization Service
builder.Services.AddScoped<DawaCloud.Web.Services.ILocalizationService, DawaCloud.Web.Services.LocalizationService>();

// Auth Service
builder.Services.AddScoped<DawaCloud.Web.Features.Auth.Services.IAuthService, DawaCloud.Web.Features.Auth.Services.AuthService>();

// Invoice Service
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// Report Services
builder.Services.AddScoped<DawaCloud.Web.Features.Reports.Services.IReportService, DawaCloud.Web.Features.Reports.Services.ReportService>();
builder.Services.AddScoped<DawaCloud.Web.Features.Reports.Services.IReportExportService, DawaCloud.Web.Features.Reports.Services.ReportExportService>();

// Expense Export Service
builder.Services.AddScoped<DawaCloud.Web.Features.Expenses.Services.ExpenseExportService>();

// AI Chat Service
builder.Services.AddHttpClient();
builder.Services.Configure<AiChatSettings>(builder.Configuration.GetSection("AiChat"));
builder.Services.AddScoped<IAiChatService, AiChatService>();

// Stripe & Subscription Services
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<ITenantService, TenantService>();

// Theme Service
builder.Services.AddScoped<DawaCloud.Web.Services.ThemeService>();

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

// Request Localization
app.UseRequestLocalization();

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
app.UseMiddleware<TenantMiddleware>();

app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Auth API Endpoints
app.MapAuthEndpoints();

// Stripe Webhook Endpoint
app.MapStripeEndpoints();

app.Run();
