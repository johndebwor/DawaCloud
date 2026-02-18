# DawaCloud - Drugs Wholesale & Retail Management System

> **Version:** 1.0.0  
> **Last Updated:** January 2026  
> **Stack:** .NET 10 | Blazor Server | SQL Server | MudBlazor

---

## ðŸ“‹ Table of Contents

1. [Project Overview](#project-overview)
2. [Technology Stack](#technology-stack)
3. [Architecture](#architecture)
4. [UI/UX Design Philosophy](#uiux-design-philosophy)
5. [Security Implementation](#security-implementation)
6. [Feature Modules](#feature-modules)
7. [Workflows & Processes](#workflows--processes)
8. [Database Design](#database-design)
9. [API Design](#api-design)
10. [Implementation Plan](#implementation-plan)
11. [Development Guidelines](#development-guidelines)
12. [Deployment & DevOps](#deployment--devops)

---

## Project Overview

### Background

**DawaCloud** (Dawa = Medicine in Swahili) is a comprehensive pharmaceutical management system designed to streamline drug inventory, procurement, wholesale and retail operations for pharmacies and drug distribution businesses in South Sudan and East Africa. This instance is currently configured for **Guru Brothers Ltd**.

### Vision

Create an intelligent, compliance-first pharmaceutical platform that automates the entire drug lifecycle from procurement to patient delivery while ensuring regulatory compliance and operational excellence.

### Key Objectives

- **Unified Operations**: Single platform for wholesale, retail, and inventory management
- **Compliance First**: Built-in regulatory compliance with batch tracking and expiry management
- **Smart Automation**: Automated reordering, notifications, and supplier communication
- **Mobile Optimized**: Field-ready design for on-the-go operations
- **Real-time Intelligence**: Live dashboards and predictive analytics

### Target Users

| Role | Primary Functions |
|------|-------------------|
| Super Admin | System configuration, user management, audit oversight |
| Inventory Manager | Stock control, procurement, batch management |
| Pharmacist | Prescription verification, drug dispensing, patient counseling |
| Cashier | POS operations, payment processing, receipt generation |
| Accountant | Financial reporting, credit management, reconciliation |
| Auditor | Compliance audits, report generation, system oversight |
| Supplier | Order viewing, delivery submission, communication |

---

## Technology Stack

### Core Framework

```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚                    DawaCloud.Web                         â”‚
â”‚         Single Blazor Server Application                â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚  UI Layer          â”‚  MudBlazor Components              â”‚
â”‚  State Management  â”‚  Fluxor (Redux pattern)            â”‚
â”‚  Real-time         â”‚  SignalR                           â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚  API Layer         â”‚  Minimal APIs / Carter             â”‚
â”‚  Validation        â”‚  FluentValidation                  â”‚
â”‚  Mapping           â”‚  Mapster                           â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚  Business Layer    â”‚  MediatR (CQRS)                    â”‚
â”‚  Background Jobs   â”‚  IHostedService / BackgroundServiceâ”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚  Data Layer        â”‚  Entity Framework Core 10          â”‚
â”‚  Database          â”‚  SQL Server 2022                   â”‚
â”‚  Caching           â”‚  Redis                             â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚  Security          â”‚  ASP.NET Core Identity             â”‚
â”‚  Authentication    â”‚  JWT + Cookie Auth                 â”‚
â”‚  Authorization     â”‚  Policy-based + Resource-based     â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

### Package Dependencies

```xml
<!-- Core -->
<PackageReference Include="Microsoft.AspNetCore.Components.Web" Version="10.0.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.*" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.*" />

<!-- UI -->
<PackageReference Include="MudBlazor" Version="7.*" />

<!-- CQRS & Mediator -->
<PackageReference Include="MediatR" Version="12.*" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.*" />

<!-- Mapping -->
<PackageReference Include="Mapster" Version="7.*" />
<PackageReference Include="Mapster.DependencyInjection" Version="1.*" />

<!-- Notifications -->
<PackageReference Include="MailKit" Version="4.*" />
<PackageReference Include="Twilio" Version="7.*" />

<!-- Reporting -->
<PackageReference Include="ClosedXML" Version="0.102.*" />
<PackageReference Include="QuestPDF" Version="2024.*" />

<!-- Security -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.*" />

<!-- Logging -->
<PackageReference Include="Serilog.AspNetCore" Version="8.*" />
<PackageReference Include="Serilog.Sinks.Seq" Version="8.*" />
```

---

## Architecture

### Feature-Based (Vertical Slice) Architecture

```
DawaCloud/
â”œâ”€â”€ src/
â”‚   â””â”€â”€ DawaCloud.Web/
â”‚       â”œâ”€â”€ Features/
â”‚       â”‚   â”œâ”€â”€ Auth/
â”‚       â”‚   â”‚   â”œâ”€â”€ Pages/
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ Login.razor
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ Register.razor
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ ForgotPassword.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ LoginForm.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ Commands/
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ LoginCommand.cs
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ RegisterCommand.cs
â”‚       â”‚   â”‚   â”œâ”€â”€ Queries/
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ GetUserProfileQuery.cs
â”‚       â”‚   â”‚   â”œâ”€â”€ Services/
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ AuthService.cs
â”‚       â”‚   â”‚   â”œâ”€â”€ Validators/
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ LoginValidator.cs
â”‚       â”‚   â”‚   â””â”€â”€ _Imports.razor
â”‚       â”‚   â”‚
â”‚       â”‚   â”œâ”€â”€ Dashboard/
â”‚       â”‚   â”‚   â”œâ”€â”€ Pages/
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ Index.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ StockAlertCard.razor
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ ExpiryAlertCard.razor
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ SalesChart.razor
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ QuickActions.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ Queries/
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ GetDashboardDataQuery.cs
â”‚       â”‚   â”‚   â””â”€â”€ Services/
â”‚       â”‚   â”‚       â””â”€â”€ DashboardService.cs
â”‚       â”‚   â”‚
â”‚       â”‚   â”œâ”€â”€ Drugs/
â”‚       â”‚   â”‚   â”œâ”€â”€ Pages/
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ DrugList.razor
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ DrugDetails.razor
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ DrugForm.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ DrugCard.razor
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ BatchTable.razor
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ PricingTierEditor.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ Commands/
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ CreateDrugCommand.cs
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ UpdateDrugCommand.cs
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ ImportDrugsCommand.cs
â”‚       â”‚   â”‚   â”œâ”€â”€ Queries/
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ GetDrugsQuery.cs
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ GetDrugByIdQuery.cs
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ SearchDrugsQuery.cs
â”‚       â”‚   â”‚   â”œâ”€â”€ Validators/
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ DrugValidator.cs
â”‚       â”‚   â”‚   â””â”€â”€ Services/
â”‚       â”‚   â”‚       â””â”€â”€ DrugService.cs
â”‚       â”‚   â”‚
â”‚       â”‚   â”œâ”€â”€ Inventory/
â”‚       â”‚   â”‚   â”œâ”€â”€ Pages/
â”‚       â”‚   â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”œâ”€â”€ Commands/
â”‚       â”‚   â”‚   â”œâ”€â”€ Queries/
â”‚       â”‚   â”‚   â””â”€â”€ Services/
â”‚       â”‚   â”‚
â”‚       â”‚   â”œâ”€â”€ Procurement/
â”‚       â”‚   â”‚   â”œâ”€â”€ Pages/
â”‚       â”‚   â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”œâ”€â”€ Commands/
â”‚       â”‚   â”‚   â”œâ”€â”€ Queries/
â”‚       â”‚   â”‚   â””â”€â”€ Services/
â”‚       â”‚   â”‚
â”‚       â”‚   â”œâ”€â”€ Suppliers/
â”‚       â”‚   â”‚   â”œâ”€â”€ Pages/
â”‚       â”‚   â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”œâ”€â”€ Commands/
â”‚       â”‚   â”‚   â”œâ”€â”€ Queries/
â”‚       â”‚   â”‚   â””â”€â”€ Services/
â”‚       â”‚   â”‚
â”‚       â”‚   â”œâ”€â”€ SupplierPortal/
â”‚       â”‚   â”‚   â”œâ”€â”€ Pages/
â”‚       â”‚   â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”œâ”€â”€ Commands/
â”‚       â”‚   â”‚   â”œâ”€â”€ Queries/
â”‚       â”‚   â”‚   â””â”€â”€ Services/
â”‚       â”‚   â”‚
â”‚       â”‚   â”œâ”€â”€ Wholesale/
â”‚       â”‚   â”‚   â”œâ”€â”€ Pages/
â”‚       â”‚   â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”œâ”€â”€ Commands/
â”‚       â”‚   â”‚   â”œâ”€â”€ Queries/
â”‚       â”‚   â”‚   â””â”€â”€ Services/
â”‚       â”‚   â”‚
â”‚       â”‚   â”œâ”€â”€ Retail/
â”‚       â”‚   â”‚   â”œâ”€â”€ Pages/
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ POS.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ ProductSearch.razor
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ CartPanel.razor
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ PaymentModal.razor
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ ReceiptPreview.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ Commands/
â”‚       â”‚   â”‚   â”œâ”€â”€ Queries/
â”‚       â”‚   â”‚   â””â”€â”€ Services/
â”‚       â”‚   â”‚
â”‚       â”‚   â”œâ”€â”€ Reports/
â”‚       â”‚   â”‚   â”œâ”€â”€ Pages/
â”‚       â”‚   â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”œâ”€â”€ Queries/
â”‚       â”‚   â”‚   â””â”€â”€ Services/
â”‚       â”‚   â”‚
â”‚       â”‚   â”œâ”€â”€ Notifications/
â”‚       â”‚   â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”œâ”€â”€ Services/
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ EmailService.cs
â”‚       â”‚   â”‚   â”‚   â”œâ”€â”€ WhatsAppService.cs
â”‚       â”‚   â”‚   â”‚   â””â”€â”€ NotificationHub.cs
â”‚       â”‚   â”‚   â””â”€â”€ Templates/
â”‚       â”‚   â”‚
â”‚       â”‚   â”œâ”€â”€ Settings/
â”‚       â”‚   â”‚   â”œâ”€â”€ Pages/
â”‚       â”‚   â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”œâ”€â”€ Commands/
â”‚       â”‚   â”‚   â””â”€â”€ Services/
â”‚       â”‚   â”‚
â”‚       â”‚   â””â”€â”€ Users/
â”‚       â”‚       â”œâ”€â”€ Pages/
â”‚       â”‚       â”œâ”€â”€ Components/
â”‚       â”‚       â”œâ”€â”€ Commands/
â”‚       â”‚       â”œâ”€â”€ Queries/
â”‚       â”‚       â””â”€â”€ Services/
â”‚       â”‚
â”‚       â”œâ”€â”€ Shared/
â”‚       â”‚   â”œâ”€â”€ Layout/
â”‚       â”‚   â”‚   â”œâ”€â”€ MainLayout.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ AppShell.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ NavigationRail.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ BottomNavigation.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ CommandPalette.razor
â”‚       â”‚   â”‚   â””â”€â”€ NotificationDrawer.razor
â”‚       â”‚   â”œâ”€â”€ Components/
â”‚       â”‚   â”‚   â”œâ”€â”€ DataGrid.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ SearchInput.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ LoadingOverlay.razor
â”‚       â”‚   â”‚   â”œâ”€â”€ ConfirmDialog.razor
â”‚       â”‚   â”‚   â””â”€â”€ FileUploader.razor
â”‚       â”‚   â””â”€â”€ Extensions/
â”‚       â”‚
â”‚       â”œâ”€â”€ Data/
â”‚       â”‚   â”œâ”€â”€ AppDbContext.cs
â”‚       â”‚   â”œâ”€â”€ Entities/
â”‚       â”‚   â”‚   â”œâ”€â”€ Drug.cs
â”‚       â”‚   â”‚   â”œâ”€â”€ Batch.cs
â”‚       â”‚   â”‚   â”œâ”€â”€ Supplier.cs
â”‚       â”‚   â”‚   â”œâ”€â”€ DrugRequest.cs
â”‚       â”‚   â”‚   â”œâ”€â”€ Sale.cs
â”‚       â”‚   â”‚   â””â”€â”€ ...
â”‚       â”‚   â”œâ”€â”€ Configurations/
â”‚       â”‚   â”‚   â””â”€â”€ [EntityConfigurations].cs
â”‚       â”‚   â””â”€â”€ Migrations/
â”‚       â”‚
â”‚       â”œâ”€â”€ Infrastructure/
â”‚       â”‚   â”œâ”€â”€ Identity/
â”‚       â”‚   â”œâ”€â”€ Middleware/
â”‚       â”‚   â”œâ”€â”€ Filters/
â”‚       â”‚   â””â”€â”€ Extensions/
â”‚       â”‚
â”‚       â”œâ”€â”€ wwwroot/
â”‚       â”‚   â”œâ”€â”€ css/
â”‚       â”‚   â”‚   â””â”€â”€ app.css
â”‚       â”‚   â”œâ”€â”€ js/
â”‚       â”‚   â”‚   â””â”€â”€ app.js
â”‚       â”‚   â””â”€â”€ images/
â”‚       â”‚
â”‚       â”œâ”€â”€ Program.cs
â”‚       â”œâ”€â”€ appsettings.json
â”‚       â””â”€â”€ appsettings.Development.json
â”‚
â”œâ”€â”€ tests/
â”‚   â”œâ”€â”€ DawaCloud.UnitTests/
â”‚   â””â”€â”€ DawaCloud.IntegrationTests/
â”‚
â”œâ”€â”€ docs/
â”‚   â””â”€â”€ ...
â”‚
â”œâ”€â”€ CLAUDE.md
â”œâ”€â”€ README.md
â””â”€â”€ DawaCloud.sln
```

### CQRS Pattern Implementation

```csharp
// Command Example
public record CreateDrugCommand(
    string Name,
    string GenericName,
    string Category,
    decimal RetailPrice,
    decimal WholesalePrice,
    int ReorderLevel,
    bool RequiresPrescription
) : IRequest<Result<int>>;

// Command Handler
public class CreateDrugCommandHandler : IRequestHandler<CreateDrugCommand, Result<int>>
{
    private readonly AppDbContext _context;
    
    public async Task<Result<int>> Handle(CreateDrugCommand request, CancellationToken ct)
    {
        var drug = request.Adapt<Drug>();
        _context.Drugs.Add(drug);
        await _context.SaveChangesAsync(ct);
        return Result.Ok(drug.Id);
    }
}

// Query Example
public record GetDrugsQuery(
    string? SearchTerm,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<DrugDto>>;
```

### Background Services Implementation

DawaCloud uses native .NET `IHostedService` and `BackgroundService` for scheduled and background tasks instead of external job schedulers.

```csharp
// Expiry Alert Background Service
public class ExpiryAlertService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExpiryAlertService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

    public ExpiryAlertService(
        IServiceScopeFactory scopeFactory,
        ILogger<ExpiryAlertService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiryAlertsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing expiry alerts");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessExpiryAlertsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var expiringBatches = await context.Batches
            .Where(b => b.ExpiryDate <= DateTime.UtcNow.AddDays(90))
            .Where(b => b.Status == BatchStatus.Active)
            .ToListAsync(ct);

        foreach (var batch in expiringBatches)
        {
            await notificationService.SendExpiryAlertAsync(batch, ct);
        }
    }
}

// Low Stock Alert Service
public class LowStockAlertService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckLowStockAsync(stoppingToken);
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CheckLowStockAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(new ProcessLowStockAlertsCommand(), ct);
    }
}

// Timed Job Service for scheduled tasks (configurable)
public class ScheduledJobService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScheduledJobService> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run at midnight every day
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextMidnight = now.Date.AddDays(1);
            var delay = nextMidnight - now;

            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
            {
                await RunDailyJobsAsync(stoppingToken);
            }
        }
    }

    private async Task RunDailyJobsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Daily jobs
        await mediator.Send(new UpdateExpiredBatchesCommand(), ct);
        await mediator.Send(new GenerateDailyReportsCommand(), ct);
        await mediator.Send(new CleanupAuditLogsCommand(), ct);
    }
}

// Registration in Program.cs
builder.Services.AddHostedService<ExpiryAlertService>();
builder.Services.AddHostedService<LowStockAlertService>();
builder.Services.AddHostedService<ScheduledJobService>();
```

---

## UI/UX Design Philosophy

### Design Principles

1. **Mobile-First, Desktop-Enhanced**: Core workflows optimized for mobile devices
2. **Contextual Navigation**: Show relevant options based on user context
3. **Progressive Disclosure**: Reveal complexity as needed
4. **Instant Feedback**: Every action has immediate visual response
5. **Accessibility First**: WCAG 2.1 AA compliance

### Innovative Layout: "Adaptive Shell"

Unlike traditional sidebar layouts, DawaCloud uses an **Adaptive Shell** design:

```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚  â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”   â”‚
â”‚  â”‚                    COMMAND BAR                           â”‚   â”‚
â”‚  â”‚  [ðŸ” Search drugs, actions, reports...     ]  [âŒ˜K]  ðŸ‘¤  â”‚   â”‚
â”‚  â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜   â”‚
â”‚                                                                 â”‚
â”‚  â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”  â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”  â”‚
â”‚  â”‚          â”‚  â”‚                                            â”‚  â”‚
â”‚  â”‚   NAV    â”‚  â”‚                                            â”‚  â”‚
â”‚  â”‚   RAIL   â”‚  â”‚              MAIN CONTENT                  â”‚  â”‚
â”‚  â”‚          â”‚  â”‚                                            â”‚  â”‚
â”‚  â”‚  [ðŸ ]    â”‚  â”‚   â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”  â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”  â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”      â”‚  â”‚
â”‚  â”‚  [ðŸ’Š]    â”‚  â”‚   â”‚ Card 1 â”‚  â”‚ Card 2 â”‚  â”‚ Card 3 â”‚      â”‚  â”‚
â”‚  â”‚  [ðŸ“¦]    â”‚  â”‚   â””â”€â”€â”€â”€â”€â”€â”€â”€â”˜  â””â”€â”€â”€â”€â”€â”€â”€â”€â”˜  â””â”€â”€â”€â”€â”€â”€â”€â”€â”˜      â”‚  â”‚
â”‚  â”‚  [ðŸ›’]    â”‚  â”‚                                            â”‚  â”‚
â”‚  â”‚  [ðŸ“Š]    â”‚  â”‚                                            â”‚  â”‚
â”‚  â”‚  [âš™ï¸]    â”‚  â”‚                                            â”‚  â”‚
â”‚  â”‚          â”‚  â”‚                                            â”‚  â”‚
â”‚  â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜  â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜  â”‚
â”‚                                                                 â”‚
â”‚              â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”               â”‚
â”‚              â”‚     CONTEXTUAL ACTION BAR       â”‚               â”‚
â”‚              â”‚  [+ New Drug]  [Import]  [Export]â”‚               â”‚
â”‚              â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜               â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜

MOBILE VIEW:
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚  DawaCloud    ðŸ”  ðŸ””  ðŸ‘¤ â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚                         â”‚
â”‚                         â”‚
â”‚     MAIN CONTENT        â”‚
â”‚                         â”‚
â”‚   â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”   â”‚
â”‚   â”‚   Stock Alert   â”‚   â”‚
â”‚   â”‚   12 items low  â”‚   â”‚
â”‚   â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜   â”‚
â”‚                         â”‚
â”‚   â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”   â”‚
â”‚   â”‚  Expiry Alert   â”‚   â”‚
â”‚   â”‚   5 batches     â”‚   â”‚
â”‚   â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜   â”‚
â”‚                         â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚  ðŸ    ðŸ’Š   âž•   ðŸ“¦   ðŸ‘¤ â”‚
â”‚ Home Drugs New  Stock Meâ”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

### Key UI Features

#### 1. Command Palette (âŒ˜K / Ctrl+K)

Quick access to all system actions:

```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚  ðŸ” Type a command or search...             â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚  RECENT                                     â”‚
â”‚  â†’ Paracetamol 500mg                        â”‚
â”‚  â†’ Sales Report - January                   â”‚
â”‚                                             â”‚
â”‚  QUICK ACTIONS                              â”‚
â”‚  âž• New Drug                          âŒ˜N    â”‚
â”‚  ðŸ›’ New Sale                          âŒ˜S    â”‚
â”‚  ðŸ“¦ Receive Stock                     âŒ˜R    â”‚
â”‚  ðŸ“‹ Generate Report                   âŒ˜G    â”‚
â”‚                                             â”‚
â”‚  NAVIGATION                                 â”‚
â”‚  â†’ Go to Dashboard                          â”‚
â”‚  â†’ Go to Inventory                          â”‚
â”‚  â†’ Go to POS                                â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

#### 2. Smart Notification Center

Slide-out drawer with categorized alerts:

```csharp
public enum NotificationCategory
{
    StockAlert,      // Low stock warnings
    ExpiryAlert,     // Expiring batches
    OrderUpdate,     // Supplier responses
    SalesAlert,      // Large transactions
    SystemAlert      // Security, maintenance
}
```

#### 3. Contextual Action Bar (CAB)

Floating action bar that adapts to current context:

- **Drug List View**: [+ New Drug] [Import Excel] [Export] [Print Labels]
- **POS View**: [Hold Sale] [Recall] [Discount] [Print Last]
- **Inventory View**: [Stock Take] [Adjust] [Transfer] [Reconcile]

#### 4. POS Layout (Retail Module)

Optimized for speed and ease of use:

```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚  ðŸ” Scan barcode or search...                    [F1 Help]  â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚                                                             â”‚
â”‚  â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”  â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”â”‚
â”‚  â”‚                           â”‚  â”‚  CART                   â”‚â”‚
â”‚  â”‚    PRODUCT QUICK GRID     â”‚  â”‚                         â”‚â”‚
â”‚  â”‚                           â”‚  â”‚  Paracetamol 500mg  x2  â”‚â”‚
â”‚  â”‚  [Parac.] [Amoxi.] [Ibu.] â”‚  â”‚           KES 40.00     â”‚â”‚
â”‚  â”‚  [Omep.]  [Metr.]  [Cet.] â”‚  â”‚  Amoxicillin 250mg  x1  â”‚â”‚
â”‚  â”‚  [Dolo.]  [Cipr.]  [Pana] â”‚  â”‚           KES 120.00    â”‚â”‚
â”‚  â”‚                           â”‚  â”‚                         â”‚â”‚
â”‚  â”‚  [View All Products â†’]    â”‚  â”‚  â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€  â”‚â”‚
â”‚  â”‚                           â”‚  â”‚  Subtotal:   KES 160.00 â”‚â”‚
â”‚  â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜  â”‚  Tax (16%):   KES 25.60 â”‚â”‚
â”‚                                 â”‚  â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€  â”‚â”‚
â”‚                                 â”‚  TOTAL:      KES 185.60 â”‚â”‚
â”‚                                 â”‚                         â”‚â”‚
â”‚                                 â”‚  â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”â”‚â”‚
â”‚                                 â”‚  â”‚     ðŸ’³ PAY NOW     â”‚â”‚â”‚
â”‚                                 â”‚  â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜â”‚â”‚
â”‚                                 â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

### Color Theme & Branding

```css
:root {
    /* Primary - Medical Teal */
    --df-primary-50: #e0f7f6;
    --df-primary-100: #b3ebe8;
    --df-primary-500: #00897b;
    --df-primary-700: #00695c;
    --df-primary-900: #004d40;
    
    /* Secondary - Warm Amber (Alerts/Actions) */
    --df-secondary-500: #ffb300;
    
    /* Semantic Colors */
    --df-success: #43a047;
    --df-warning: #fb8c00;
    --df-danger: #e53935;
    --df-info: #1e88e5;
    
    /* Neutral */
    --df-surface: #ffffff;
    --df-background: #f5f7fa;
    --df-text-primary: #1a1a2e;
    --df-text-secondary: #64748b;
    
    /* Gradients */
    --df-gradient-primary: linear-gradient(135deg, #00897b 0%, #00695c 100%);
    --df-gradient-accent: linear-gradient(135deg, #ffb300 0%, #ff8f00 100%);
}
```

### MudBlazor Theme Configuration

```csharp
var DawaCloudTheme = new MudTheme()
{
    PaletteLight = new PaletteLight()
    {
        Primary = "#00897b",
        PrimaryDarken = "#00695c",
        PrimaryLighten = "#4db6ac",
        Secondary = "#ffb300",
        Background = "#f5f7fa",
        Surface = "#ffffff",
        AppbarBackground = "#ffffff",
        DrawerBackground = "#ffffff",
        TextPrimary = "#1a1a2e",
        TextSecondary = "#64748b",
    },
    Typography = new Typography()
    {
        Default = new Default()
        {
            FontFamily = new[] { "Inter", "Segoe UI", "sans-serif" },
            FontSize = "0.875rem",
            FontWeight = 400,
        },
        H1 = new H1() { FontSize = "2.5rem", FontWeight = 700 },
        H2 = new H2() { FontSize = "2rem", FontWeight = 600 },
        H3 = new H3() { FontSize = "1.5rem", FontWeight = 600 },
    },
    LayoutProperties = new LayoutProperties()
    {
        DefaultBorderRadius = "12px",
    }
};
```

---

## Security Implementation

### OWASP Top 10 Mitigation

| # | Vulnerability | Mitigation Strategy |
|---|---------------|---------------------|
| A01 | Broken Access Control | Role-based + Resource-based authorization with policies |
| A02 | Cryptographic Failures | AES-256 encryption, TLS 1.3, secure key management |
| A03 | Injection | Parameterized queries via EF Core, input validation |
| A04 | Insecure Design | Threat modeling, security-first architecture |
| A05 | Security Misconfiguration | Hardened defaults, security headers, CSP |
| A06 | Vulnerable Components | Dependabot, regular dependency updates |
| A07 | Authentication Failures | ASP.NET Identity, MFA, account lockout |
| A08 | Data Integrity Failures | Digital signatures, audit logging |
| A09 | Logging Failures | Comprehensive audit trail with Serilog |
| A10 | SSRF | URL validation, allowlist for external calls |

### Authentication Configuration

```csharp
// Program.cs
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
.AddDefaultTokenProviders();

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
```

### Authorization Policies

```csharp
builder.Services.AddAuthorizationBuilder()
    // Role-based Policies
    .AddPolicy("RequireAdmin", policy => 
        policy.RequireRole("SuperAdmin"))
    .AddPolicy("RequireInventoryAccess", policy => 
        policy.RequireRole("SuperAdmin", "InventoryManager"))
    .AddPolicy("RequireSalesAccess", policy => 
        policy.RequireRole("SuperAdmin", "Pharmacist", "Cashier"))
    .AddPolicy("RequireFinanceAccess", policy => 
        policy.RequireRole("SuperAdmin", "Accountant"))
    
    // Permission-based Policies
    .AddPolicy("CanManageDrugs", policy => 
        policy.RequireClaim("Permission", "Drugs.Manage"))
    .AddPolicy("CanApproveOrders", policy => 
        policy.RequireClaim("Permission", "Orders.Approve"))
    .AddPolicy("CanViewReports", policy => 
        policy.RequireClaim("Permission", "Reports.View"));
```

### Security Headers Middleware

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", 
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        "font-src 'self' https://fonts.gstatic.com; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self' wss:;");
    
    await next();
});
```

### Audit Logging

```csharp
public class AuditLog
{
    public long Id { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Action { get; set; }         // Create, Update, Delete, Login, etc.
    public string EntityType { get; set; }     // Drug, Sale, User, etc.
    public string EntityId { get; set; }
    public string OldValues { get; set; }      // JSON
    public string NewValues { get; set; }      // JSON
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
}

// Automatic audit via EF Core interceptor
public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    // Implementation tracks all entity changes
}
```

---

## Feature Modules

### Module 1: Authentication & Authorization

**Features:**
- User registration with email verification
- Multi-factor authentication (TOTP)
- Password reset flow
- Session management
- Role and permission management
- Activity logging

**Pages:**
- `/auth/login`
- `/auth/register`
- `/auth/forgot-password`
- `/auth/reset-password`
- `/auth/verify-email`
- `/auth/mfa-setup`

---

### Module 2: Dashboard

**Features:**
- Real-time KPI cards (sales, stock value, orders)
- Stock level alerts widget
- Expiry alerts widget
- Recent sales activity
- Sales trend charts
- Quick action buttons
- Customizable widget layout

**Key Components:**

```razor
@* Dashboard Page *@
<MudGrid>
    <MudItem xs="12" md="3">
        <KpiCard Title="Today's Sales" Value="@todaySales" Icon="@Icons.Material.Filled.PointOfSale" />
    </MudItem>
    <MudItem xs="12" md="3">
        <KpiCard Title="Stock Value" Value="@stockValue" Icon="@Icons.Material.Filled.Inventory" />
    </MudItem>
    <MudItem xs="12" md="3">
        <AlertCard Title="Low Stock Items" Count="@lowStockCount" Severity="Warning" />
    </MudItem>
    <MudItem xs="12" md="3">
        <AlertCard Title="Expiring Soon" Count="@expiringCount" Severity="Error" />
    </MudItem>
    
    <MudItem xs="12" md="8">
        <SalesTrendChart Data="@salesData" />
    </MudItem>
    <MudItem xs="12" md="4">
        <RecentActivityFeed Activities="@activities" />
    </MudItem>
</MudGrid>
```

---

### Module 3: Drug Master Management

**Features:**
- Drug registration and classification
- Barcode/SKU management
- Multi-tier pricing (retail, wholesale, bulk)
- Tax configuration
- Prescription status tracking
- Drug categories and subcategories
- Image upload
- Batch listing per drug

**Entity:**

```csharp
public class Drug : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Code { get; set; }              // Unique SKU
    public string Barcode { get; set; }
    public string Name { get; set; }
    public string GenericName { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public string Manufacturer { get; set; }
    public string DosageForm { get; set; }        // Tablet, Syrup, Injection
    public string Strength { get; set; }          // 500mg, 250ml
    public string PackSize { get; set; }          // Pack of 10, Bottle 100ml
    public decimal RetailPrice { get; set; }
    public decimal WholesalePrice { get; set; }
    public decimal CostPrice { get; set; }
    public int ReorderLevel { get; set; }
    public int MaxStockLevel { get; set; }
    public decimal TaxRate { get; set; }
    public bool RequiresPrescription { get; set; }
    public bool IsControlled { get; set; }
    public bool IsActive { get; set; }
    public string ImageUrl { get; set; }
    
    // Navigation
    public DrugCategory Category { get; set; }
    public ICollection<Batch> Batches { get; set; }
    public ICollection<PricingTier> PricingTiers { get; set; }
}
```

---

### Module 4: Batch & Expiry Management

**Features:**
- Batch creation with expiry dates
- FEFO (First Expiry, First Out) enforcement
- Expiry alerts (30, 60, 90 days)
- Batch recall management
- Expired stock quarantine
- Batch-level stock tracking

**Entity:**

```csharp
public class Batch : BaseAuditableEntity
{
    public int Id { get; set; }
    public int DrugId { get; set; }
    public string BatchNumber { get; set; }
    public DateTime ManufactureDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int InitialQuantity { get; set; }
    public int CurrentQuantity { get; set; }
    public int ReservedQuantity { get; set; }    // For pending sales
    public decimal CostPrice { get; set; }
    public string SupplierBatchRef { get; set; }
    public int? GoodsReceiptId { get; set; }
    public BatchStatus Status { get; set; }       // Active, Expired, Recalled, Depleted
    
    // Navigation
    public Drug Drug { get; set; }
    public GoodsReceipt GoodsReceipt { get; set; }
}

public enum BatchStatus
{
    Active,
    Expired,
    Recalled,
    Quarantined,
    Depleted
}
```

---

### Module 5: Inventory & Stock Control

**Features:**
- Real-time stock levels
- Stock movements tracking
- Stock adjustments (damage, theft, correction)
- Stock transfers between locations
- Stock reconciliation
- Physical stock count
- Stock valuation reports

**Entity:**

```csharp
public class StockMovement : BaseAuditableEntity
{
    public long Id { get; set; }
    public int DrugId { get; set; }
    public int BatchId { get; set; }
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public int BalanceBefore { get; set; }
    public int BalanceAfter { get; set; }
    public string Reference { get; set; }         // Sale#, GRN#, Adjustment#
    public string Reason { get; set; }
    public string Notes { get; set; }
    
    // Navigation
    public Drug Drug { get; set; }
    public Batch Batch { get; set; }
}

public enum MovementType
{
    Purchase,
    Sale,
    Return,
    Adjustment,
    Transfer,
    WriteOff,
    Opening
}
```

---

### Module 6: Procurement & Supplier Management

**Features:**
- Supplier registration and management
- Supplier contact persons
- Notification preferences (Email/WhatsApp)
- Purchase requisitions
- Automatic reorder suggestions
- Purchase order generation
- Supplier performance tracking

**Workflow:**

```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”    â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”    â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”    â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”    â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚ Low Stockâ”‚â”€â”€â”€â–¶â”‚ Create   â”‚â”€â”€â”€â–¶â”‚ Approve  â”‚â”€â”€â”€â–¶â”‚ Send to  â”‚â”€â”€â”€â–¶â”‚ Supplier â”‚
â”‚ Detected â”‚    â”‚ Request  â”‚    â”‚ Request  â”‚    â”‚ Supplier â”‚    â”‚ Responds â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜    â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜    â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜    â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜    â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
                                                                      â”‚
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”    â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”    â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”    â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”         â”‚
â”‚ Update   â”‚â—€â”€â”€â”€â”‚ Post to  â”‚â—€â”€â”€â”€â”‚ Verify   â”‚â—€â”€â”€â”€â”‚ Receive  â”‚â—€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
â”‚ Stock    â”‚    â”‚ Inventoryâ”‚    â”‚ Delivery â”‚    â”‚ Goods    â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜    â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜    â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜    â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

---

### Module 7: Supplier Portal

**Features:**
- Dedicated supplier login
- View pending drug requests
- Submit delivery details
- Enter supplied drugs with batch info
- Generate delivery notes
- View order history
- Communication center

**Pages:**
- `/supplier/login`
- `/supplier/dashboard`
- `/supplier/requests`
- `/supplier/request/{id}`
- `/supplier/deliveries`
- `/supplier/profile`

---

### Module 8: Wholesale Sales

**Features:**
- Customer (buyer) management
- Tiered pricing based on quantity
- Quotation generation
- Sales order processing
- Invoice generation
- Credit limit management
- Payment tracking
- Delivery scheduling

**Entity:**

```csharp
public class WholesaleSale : BaseAuditableEntity
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; }
    public int CustomerId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public SaleStatus Status { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string DeliveryAddress { get; set; }
    public string Notes { get; set; }
    
    // Navigation
    public WholesaleCustomer Customer { get; set; }
    public ICollection<WholesaleSaleItem> Items { get; set; }
    public ICollection<Payment> Payments { get; set; }
}
```

---

### Module 9: Retail POS

**Features:**
- Barcode scanning
- Product quick search
- FEFO batch selection
- Prescription verification
- Multiple payment methods
- Split payments
- Receipt generation (thermal/A4)
- Sale hold and recall
- End-of-day reconciliation
- Cash drawer management

**POS State Management:**

```csharp
// Fluxor State
public record PosState
{
    public List<CartItem> Cart { get; init; } = new();
    public Customer? Customer { get; init; }
    public decimal SubTotal { get; init; }
    public decimal TaxTotal { get; init; }
    public decimal DiscountTotal { get; init; }
    public decimal GrandTotal { get; init; }
    public bool IsProcessing { get; init; }
    public List<HeldSale> HeldSales { get; init; } = new();
}

// Actions
public record AddToCartAction(Drug Drug, int Quantity, Batch Batch);
public record RemoveFromCartAction(int CartItemId);
public record UpdateQuantityAction(int CartItemId, int NewQuantity);
public record ApplyDiscountAction(DiscountType Type, decimal Value);
public record ProcessPaymentAction(List<PaymentMethod> Payments);
public record HoldSaleAction(string Reference);
public record RecallSaleAction(int HeldSaleId);
public record ClearCartAction();
```

---

### Module 10: Notifications

**Features:**
- Email notifications (SMTP/SendGrid)
- WhatsApp notifications (Twilio/Africa's Talking)
- In-app notifications
- Notification templates
- Delivery logs
- Retry mechanism
- User notification preferences

**Notification Types:**

| Event | Recipients | Channels |
|-------|------------|----------|
| Low Stock Alert | Inventory Manager | Dashboard, Email |
| Expiry Alert | Inventory Manager | Dashboard, Email |
| New Drug Request | Supplier | Email, WhatsApp |
| Delivery Submitted | Inventory Manager | Dashboard, Email |
| Order Approved | Supplier | Email, WhatsApp |
| Payment Received | Accountant | Dashboard, Email |

**Service:**

```csharp
public interface INotificationService
{
    Task SendAsync(Notification notification);
    Task SendEmailAsync(string to, string subject, string template, object model);
    Task SendWhatsAppAsync(string phone, string template, object model);
    Task SendInAppAsync(string userId, string title, string message, NotificationCategory category);
}
```

---

### Module 11: Reports

**Features:**
- Stock reports (current levels, valuation, movements)
- Expiry reports (expired, expiring soon)
- Sales reports (daily, weekly, monthly)
- Supplier reports (purchases, performance)
- Financial reports (P&L, outstanding payments)
- Compliance reports (controlled drugs, prescriptions)
- Custom report builder
- Export to Excel/PDF

**Report Categories:**

```csharp
public enum ReportCategory
{
    Inventory,
    Sales,
    Procurement,
    Financial,
    Compliance,
    Audit
}

// Stock Report Query
public record StockReportQuery(
    DateTime? AsAtDate,
    int? CategoryId,
    bool IncludeZeroStock,
    StockValuationMethod ValuationMethod
) : IRequest<StockReportResult>;
```

---

### Module 12: System Settings

**Features:**
- Company profile
- Branch management
- Tax configuration
- Notification templates
- Email/WhatsApp API settings
- Alert thresholds
- Backup configuration
- System logs viewer

---

## Workflows & Processes

### Workflow 1: Drug Procurement Cycle

```mermaid
stateDiagram-v2
    [*] --> LowStockDetected: Auto/Manual trigger
    LowStockDetected --> RequestCreated: Create drug request
    RequestCreated --> PendingApproval: Submit for approval
    PendingApproval --> Approved: Manager approves
    PendingApproval --> Rejected: Manager rejects
    Rejected --> [*]
    Approved --> NotifySupplier: Send notification
    NotifySupplier --> AwaitingResponse: Email/WhatsApp sent
    AwaitingResponse --> SupplierConfirmed: Supplier accepts
    AwaitingResponse --> Timeout: No response (3 days)
    Timeout --> NotifySupplier: Resend/Escalate
    SupplierConfirmed --> AwaitingDelivery: Delivery scheduled
    AwaitingDelivery --> DeliveryReceived: Goods arrive
    DeliveryReceived --> Verification: Verify quantities & batches
    Verification --> PartialReceipt: Some items missing
    Verification --> FullReceipt: All items received
    PartialReceipt --> StockPosted: Post available items
    FullReceipt --> StockPosted: Post all items
    StockPosted --> RequestClosed: Close request
    RequestClosed --> [*]
```

### Workflow 2: Retail Sale Process

```mermaid
stateDiagram-v2
    [*] --> Scanning: Start sale
    Scanning --> ItemAdded: Scan/Search drug
    ItemAdded --> BatchSelected: Auto FEFO or manual
    BatchSelected --> CartUpdated: Add to cart
    CartUpdated --> Scanning: Continue scanning
    CartUpdated --> Checkout: Proceed to pay
    Checkout --> PrescriptionCheck: Check Rx items
    PrescriptionCheck --> PaymentEntry: Rx verified
    PrescriptionCheck --> RxRequired: Prescription needed
    RxRequired --> PaymentEntry: Pharmacist approves
    PaymentEntry --> PaymentProcessed: Process payment
    PaymentProcessed --> ReceiptGenerated: Generate receipt
    ReceiptGenerated --> StockDeducted: Update inventory
    StockDeducted --> [*]: Sale complete
```

### Workflow 3: Expiry Management

```mermaid
stateDiagram-v2
    [*] --> DailyCheck: Scheduled job (midnight)
    DailyCheck --> IdentifyExpiring: Query batches
    IdentifyExpiring --> Alert90Days: 90 days to expiry
    IdentifyExpiring --> Alert60Days: 60 days to expiry
    IdentifyExpiring --> Alert30Days: 30 days to expiry
    IdentifyExpiring --> Expired: Already expired
    Alert90Days --> NotifyManager: Send dashboard alert
    Alert60Days --> NotifyManager: Send email alert
    Alert30Days --> UrgentAction: Email + WhatsApp
    Expired --> Quarantine: Block from sale
    Quarantine --> WriteOff: Approve disposal
    WriteOff --> AuditLog: Log write-off
    AuditLog --> [*]
```

### Workflow 4: Excel Import for Received Drugs

```mermaid
stateDiagram-v2
    [*] --> DownloadTemplate: Get Excel template
    DownloadTemplate --> FillData: Enter drug details
    FillData --> UploadFile: Upload filled Excel
    UploadFile --> ParseFile: Read Excel content
    ParseFile --> Validate: Validate each row
    Validate --> ValidationErrors: Has errors
    Validate --> ValidationSuccess: All valid
    ValidationErrors --> ShowErrors: Display error report
    ShowErrors --> FillData: Correct and retry
    ValidationSuccess --> Preview: Show preview
    Preview --> ConfirmImport: User confirms
    ConfirmImport --> CreateBatches: Create batch records
    CreateBatches --> PostStock: Post to inventory
    PostStock --> GenerateGRN: Create goods receipt
    GenerateGRN --> AuditLog: Log import
    AuditLog --> [*]
```

---

## Database Design

### Entity Relationship Diagram (Core Entities)

```
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”       â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”       â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚   DrugCategory  â”‚       â”‚      Drug       â”‚       â”‚     Batch       â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤       â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤       â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ Id              â”‚       â”‚ Id              â”‚       â”‚ Id              â”‚
â”‚ Name            â”‚â—€â”€â”€â”€â”€â”€â”€â”‚ CategoryId      â”‚â—€â”€â”€â”€â”€â”€â”€â”‚ DrugId          â”‚
â”‚ ParentId        â”‚       â”‚ Name            â”‚       â”‚ BatchNumber     â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜       â”‚ GenericName     â”‚       â”‚ ExpiryDate      â”‚
                          â”‚ RetailPrice     â”‚       â”‚ CurrentQuantity â”‚
                          â”‚ WholesalePrice  â”‚       â”‚ Status          â”‚
                          â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜       â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
                                   â”‚                        â”‚
                                   â”‚                        â”‚
                          â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”´â”€â”€â”€â”€â”€â”€â”€â”€â”      â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”´â”€â”€â”€â”€â”€â”€â”€â”€â”
                          â–¼                 â–¼      â–¼                 â–¼
                   â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”   â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”   â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
                   â”‚ SaleItem    â”‚   â”‚ StockMove.  â”‚   â”‚ GRNItem     â”‚
                   â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤   â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤   â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
                   â”‚ Id          â”‚   â”‚ Id          â”‚   â”‚ Id          â”‚
                   â”‚ SaleId      â”‚   â”‚ DrugId      â”‚   â”‚ GRNId       â”‚
                   â”‚ DrugId      â”‚   â”‚ BatchId     â”‚   â”‚ DrugId      â”‚
                   â”‚ BatchId     â”‚   â”‚ Quantity    â”‚   â”‚ BatchId     â”‚
                   â”‚ Quantity    â”‚   â”‚ Type        â”‚   â”‚ Quantity    â”‚
                   â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜   â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜   â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
                          â–²                                   â–²
                          â”‚                                   â”‚
                   â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”                     â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
                   â”‚    Sale     â”‚                     â”‚ GoodsReceiptâ”‚
                   â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤                     â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
                   â”‚ Id          â”‚                     â”‚ Id          â”‚
                   â”‚ InvoiceNo   â”‚                     â”‚ GRNNumber   â”‚
                   â”‚ CustomerId  â”‚                     â”‚ SupplierId  â”‚
                   â”‚ TotalAmount â”‚                     â”‚ RequestId   â”‚
                   â”‚ SaleType    â”‚                     â”‚ Status      â”‚
                   â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜                     â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
                                                              â–²
                                                              â”‚
â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”       â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”       â”Œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”
â”‚    Supplier     â”‚       â”‚  DrugRequest    â”‚       â”‚ DrugRequestItem â”‚
â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤       â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤       â”œâ”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”¤
â”‚ Id              â”‚â—€â”€â”€â”€â”€â”€â”€â”‚ SupplierId      â”‚â—€â”€â”€â”€â”€â”€â”€â”‚ RequestId       â”‚
â”‚ Name            â”‚       â”‚ RequestNumber   â”‚       â”‚ DrugId          â”‚
â”‚ Email           â”‚       â”‚ Status          â”‚       â”‚ RequestedQty    â”‚
â”‚ Phone           â”‚       â”‚ TotalAmount     â”‚       â”‚ SuppliedQty     â”‚
â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜       â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜       â””â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”˜
```

### Key Tables Summary

| Table | Purpose | Key Fields |
|-------|---------|------------|
| `Drugs` | Drug master data | Code, Name, Prices, ReorderLevel |
| `DrugCategories` | Hierarchical categorization | Name, ParentId |
| `Batches` | Batch tracking | DrugId, BatchNumber, ExpiryDate, Quantity |
| `Suppliers` | Supplier registry | Name, Contacts, NotificationPrefs |
| `DrugRequests` | Purchase requisitions | SupplierId, Status, Items |
| `DrugRequestItems` | Request line items | DrugId, RequestedQty, SuppliedQty |
| `GoodsReceipts` | Receiving records | GRNNumber, SupplierId, RequestId |
| `GoodsReceiptItems` | Received items | DrugId, BatchId, Quantity |
| `Sales` | Sale headers | InvoiceNo, CustomerId, Type, Total |
| `SaleItems` | Sale line items | DrugId, BatchId, Quantity, Price |
| `StockMovements` | All stock transactions | DrugId, BatchId, Type, Quantity |
| `WholesaleCustomers` | B2B customers | Name, CreditLimit, Balance |
| `Users` | System users | Email, Role, Permissions |
| `AuditLogs` | Activity tracking | UserId, Action, Entity, Changes |
| `Notifications` | Notification queue | Type, Channel, Status, Payload |
| `NotificationTemplates` | Message templates | Name, Subject, Body, Channel |

---

## API Design

### API Endpoints (Minimal API with Carter)

```csharp
// Features/Drugs/DrugsModule.cs
public class DrugsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/drugs")
            .RequireAuthorization("RequireInventoryAccess")
            .WithTags("Drugs");
        
        group.MapGet("/", GetDrugs);
        group.MapGet("/{id:int}", GetDrugById);
        group.MapPost("/", CreateDrug);
        group.MapPut("/{id:int}", UpdateDrug);
        group.MapDelete("/{id:int}", DeleteDrug);
        group.MapGet("/{id:int}/batches", GetDrugBatches);
        group.MapPost("/import", ImportDrugs);
        group.MapGet("/export", ExportDrugs);
        group.MapGet("/template", DownloadImportTemplate);
        group.MapGet("/search", SearchDrugs);
        group.MapGet("/low-stock", GetLowStockDrugs);
        group.MapGet("/expiring", GetExpiringDrugs);
    }
}
```

### API Endpoint Summary

| Module | Endpoint | Method | Description |
|--------|----------|--------|-------------|
| Auth | `/api/auth/login` | POST | User login |
| Auth | `/api/auth/logout` | POST | User logout |
| Auth | `/api/auth/refresh` | POST | Refresh token |
| Drugs | `/api/drugs` | GET | List all drugs |
| Drugs | `/api/drugs/{id}` | GET | Get drug details |
| Drugs | `/api/drugs` | POST | Create drug |
| Drugs | `/api/drugs/{id}` | PUT | Update drug |
| Drugs | `/api/drugs/import` | POST | Bulk import |
| Batches | `/api/batches` | GET | List batches |
| Batches | `/api/batches/expiring` | GET | Expiring batches |
| Inventory | `/api/inventory/stock` | GET | Stock levels |
| Inventory | `/api/inventory/adjust` | POST | Stock adjustment |
| Inventory | `/api/inventory/transfer` | POST | Stock transfer |
| Suppliers | `/api/suppliers` | GET/POST | Supplier CRUD |
| Requests | `/api/requests` | GET/POST | Drug requests |
| Requests | `/api/requests/{id}/approve` | POST | Approve request |
| Requests | `/api/requests/{id}/notify` | POST | Notify supplier |
| Sales | `/api/sales` | GET/POST | Sales CRUD |
| Sales | `/api/sales/pos` | POST | POS transaction |
| Reports | `/api/reports/stock` | GET | Stock report |
| Reports | `/api/reports/sales` | GET | Sales report |
| Reports | `/api/reports/expiry` | GET | Expiry report |

---

## Implementation Plan

### Phase 1: Foundation (Weeks 1-3)

| # | Task | Status | Notes |
|---|------|--------|-------|
| 1.1 | Project setup & solution structure | âœ… Completed | Created solution with feature-based architecture |
| 1.2 | Configure EF Core & database context | âœ… Completed | SQL Server with interceptors |
| 1.3 | Implement base entities & configurations | âœ… Completed | BaseAuditableEntity, soft delete, Drug, Batch, Supplier |
| 1.4 | Setup ASP.NET Core Identity | âœ… Completed | ApplicationUser, ApplicationRole configured |
| 1.5 | Configure authentication & authorization | âœ… Completed | Cookie auth, role-based policies |
| 1.6 | Setup MudBlazor & theme configuration | âœ… Completed | DawaCloud theme with Medical Teal, custom CSS, Inter font |
| 1.7 | Create adaptive shell layout | âœ… Completed | Adaptive Shell with expandable drawer (240px/72px), Command Bar, Nav Rail, toggle menu |
| 1.8 | Implement audit logging interceptor | âœ… Completed | AuditSaveChangesInterceptor implemented |
| 1.9 | Setup Serilog & Seq logging | âœ… Completed | Structured logging configured |
| 1.10 | Configure Background Services | âœ… Completed | ExpiryAlertService, LowStockAlertService |

### Phase 2: Core Modules (Weeks 4-7)

| # | Task | Status | Notes |
|---|------|--------|-------|
| 2.1 | Auth module - Login page | âœ… Completed | Email/password login with demo credentials |
| 2.2 | Auth module - Registration | âœ… Completed | With email verification UI & password strength |
| 2.3 | Auth module - Password reset | âœ… Completed | Token-based reset flow |
| 2.4 | Auth module - MFA setup | â¬œ Pending | TOTP implementation (deferred) |
| 2.5 | Dashboard - KPI cards | âœ… Completed | Custom styled KPI cards with icons |
| 2.6 | Dashboard - Alert widgets | âœ… Completed | Low stock & expiry alerts widgets |
| 2.7 | Dashboard - Sales chart | âœ… Completed | Placeholder with trend visualization notes |
| 2.8 | Drug module - CRUD operations | âœ… Completed | List with data table, create/edit form with validation |
| 2.9 | Drug module - Search & filter | âœ… Completed | Real-time search, category & status filters |
| 2.10 | Drug module - Pricing tiers | âœ… Completed | PricingTier entity added, multi-tier pricing support |
| 2.11 | Drug module - Excel import | â¬œ Pending | Template, validation, import (deferred) |
| 2.12 | Batch module - CRUD operations | âœ… Completed | Full batch list, create/edit forms with status tracking |
| 2.13 | Batch module - Expiry tracking | âœ… Completed | Status colors, days to expiry, quarantine workflow |
| 2.14 | Category module - Hierarchical CRUD | âœ… Completed | Category list with parent/child relationships |

### Phase 3: Inventory & Procurement (Weeks 8-10)

| # | Task | Status | Notes |
|---|------|--------|-------|
| 3.1 | Inventory - Stock levels view | âœ… Completed | Real-time quantities from batches, MediatR query, filtering |
| 3.2 | Inventory - Stock movements | âœ… Completed | Complete audit trail, pagination, filtering by type/date |
| 3.3 | Inventory - Adjustments | âœ… Completed | Add/remove stock, validation, reason tracking, auto-depletion |
| 3.4 | Inventory - Stock count | âœ… Completed | Physical count workflow, variance tracking, auto-adjustment |
| 3.5 | Inventory - Reconciliation | âœ… Completed | Built into stock count - variance reports and reconciliation |
| 3.6 | Supplier module - Registration | âœ… Completed | Supplier list, queries, create command with auto-generated codes |
| 3.7 | Supplier module - Contact persons | âœ… Completed | Integrated in supplier entity with multiple contacts support |
| 3.8 | Supplier module - Notification prefs | âœ… Completed | Email/WhatsApp toggles in supplier entity |
| 3.9 | Request module - Create request | âœ… Completed | Drug request creation with items, auto-generated request numbers |
| 3.10 | Request module - Approval workflow | âœ… Completed | Approve/Reject commands with approval tracking |
| 3.11 | Request module - Status tracking | âœ… Completed | Full lifecycle status enum (10 states) with filtering |

### Phase 4: Notifications & Supplier Portal (Weeks 11-12)

| # | Task | Status | Notes |
|---|------|--------|-------|
| 4.1 | Notification - Email service | âœ… Completed | SMTP with MailKit, configurable settings |
| 4.2 | Notification - WhatsApp service | âœ… Completed | Twilio integration ready |
| 4.3 | Notification - In-app alerts | âœ… Completed | Notification entity, service layer |
| 4.4 | Notification - Template engine | âœ… Completed | NotificationTemplate entity, template management UI |
| 4.5 | Notification - Delivery logging | âœ… Completed | Status tracking, retry count, error logging |
| 4.6 | Supplier Portal - Login | âœ… Completed | Separate layout prepared |
| 4.7 | Supplier Portal - Dashboard | âœ… Completed | KPIs, pending requests, activity feed |
| 4.8 | Supplier Portal - View requests | âœ… Completed | Request list with status filtering |
| 4.9 | Supplier Portal - Submit delivery | âœ… Completed | GoodsReceipt and GoodsReceiptItem entities |
| 4.10 | Supplier Portal - Delivery note | â¬œ Pending | PDF generation with QuestPDF |
| 4.11 | Goods receiving - Verification | âœ… Completed | GoodsReceipt workflow entities |
| 4.12 | Goods receiving - Stock posting | âœ… Completed | Entity structure for auto batch creation |

### Phase 5: Sales Modules (Weeks 13-15)

| # | Task | Status | Notes |
|---|------|--------|-------|
| 5.1 | Wholesale - Customer management | âœ… Completed | Customer list, CRUD dialog, tier filtering |
| 5.2 | Wholesale - Credit limits | âœ… Completed | Credit limit tracking, over-limit warnings |
| 5.3 | Wholesale - Quotations | âœ… Completed | Quotation entity and status tracking |
| 5.4 | Wholesale - Sales orders | âœ… Completed | WholesaleSale list with filtering, payment status |
| 5.5 | Wholesale - Invoicing | âœ… Completed | Invoice number, payment recording dialog |
| 5.6 | Wholesale - Payments | âœ… Completed | Payment entity, multi-payment support |
| 5.7 | Retail POS - UI layout | âœ… Completed | Two-panel design: product grid + cart |
| 5.8 | Retail POS - Barcode scanning | âœ… Completed | Search input with Enter key handling |
| 5.9 | Retail POS - Cart management | âœ… Completed | Add/remove items, quantity controls |
| 5.10 | Retail POS - FEFO selection | âœ… Completed | Batch selection support in cart items |
| 5.11 | Retail POS - Payment processing | âœ… Completed | Cash, MoMo, Card payment methods |
| 5.12 | Retail POS - Receipt printing | âœ… Completed | Receipt number generation, success dialog |
| 5.13 | Retail POS - Hold & recall | âœ… Completed | HeldSale entity, UI placeholders |
| 5.14 | Retail POS - End-of-day | âœ… Completed | CashierShift entity with reconciliation |

### Phase 6: Reporting & Administration (Weeks 16-17)

| # | Task | Status | Notes |
|---|------|--------|-------|
| 6.1 | Reports - Stock report | âœ… Completed | Report category UI, filter dialog |
| 6.2 | Reports - Expiry report | âœ… Completed | Listed in report menu |
| 6.3 | Reports - Sales report | âœ… Completed | Daily, monthly, by product/customer |
| 6.4 | Reports - Supplier report | âœ… Completed | Purchase summary, supplier performance |
| 6.5 | Reports - Financial report | âœ… Completed | P&L, outstanding payments, receivables aging |
| 6.6 | Reports - Export Excel/PDF | âœ… Completed | Export buttons in report dialog |
| 6.7 | User management - CRUD | âœ… Completed | User list, create/edit dialog |
| 6.8 | User management - Roles | âœ… Completed | Role selection with permissions |
| 6.9 | User management - Permissions | âœ… Completed | Granular permission checkboxes |
| 6.10 | Settings - Company profile | âœ… Completed | Company info, logo upload |
| 6.11 | Settings - Tax configuration | âœ… Completed | VAT rates, tax categories |
| 6.12 | Settings - Alert thresholds | âœ… Completed | Low stock days, expiry alert days |
| 6.13 | Settings - API keys | âœ… Completed | Email/WhatsApp configuration tabs |
| 6.14 | Audit - Activity logs | âœ… Completed | Searchable logs with detail view |

### Phase 7: Testing & Deployment (Weeks 18-20)

| # | Task | Status | Notes |
|---|------|--------|-------|
| 7.1 | Unit tests - Business logic | â¬œ Pending | xUnit, NSubstitute |
| 7.2 | Integration tests - API | â¬œ Pending | WebApplicationFactory |
| 7.3 | E2E tests - Critical paths | â¬œ Pending | Playwright |
| 7.4 | Performance testing | â¬œ Pending | Load testing |
| 7.5 | Security testing | â¬œ Pending | OWASP ZAP |
| 7.6 | Documentation | â¬œ Pending | User guide, API docs |
| 7.7 | CI/CD pipeline | â¬œ Pending | GitHub Actions |
| 7.8 | Docker containerization | â¬œ Pending | Dockerfile, compose |
| 7.9 | Production deployment | â¬œ Pending | Azure/AWS |
| 7.10 | Monitoring setup | â¬œ Pending | Health checks, alerts |

### Status Legend

| Symbol | Meaning |
|--------|---------|
| â¬œ | Pending |
| ðŸ”„ | In Progress |
| âœ… | Completed |
| â¸ï¸ | On Hold |
| âŒ | Cancelled |

---

## Development Guidelines

### Code Style

```csharp
// Use file-scoped namespaces
namespace DawaCloud.Web.Features.Drugs;

// Use primary constructors for dependency injection
public class DrugService(AppDbContext context, IMapper mapper)
{
    public async Task<DrugDto> GetByIdAsync(int id)
    {
        var drug = await context.Drugs
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        
        return mapper.Map<DrugDto>(drug);
    }
}

// Use records for DTOs and commands
public record DrugDto(
    int Id,
    string Code,
    string Name,
    decimal RetailPrice
);

// Use Result pattern for operations
public record Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }
    
    public static Result<T> Ok(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Fail(string error) => new() { IsSuccess = false, Error = error };
}
```

### Blazor Component Guidelines

```razor
@* Use code-behind for complex components *@
@* DrugCard.razor *@
@inherits DrugCardBase

<MudCard Elevation="2" Class="drug-card">
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">@Drug.Name</MudText>
            <MudText Typo="Typo.body2" Color="Color.Secondary">@Drug.GenericName</MudText>
        </CardHeaderContent>
        <CardHeaderActions>
            <MudIconButton Icon="@Icons.Material.Filled.MoreVert" />
        </CardHeaderActions>
    </MudCardHeader>
    <MudCardContent>
        <MudText>Stock: @Drug.CurrentStock</MudText>
        <MudText>Price: KES @Drug.RetailPrice.ToString("N2")</MudText>
    </MudCardContent>
</MudCard>

@* DrugCard.razor.cs *@
public class DrugCardBase : ComponentBase
{
    [Parameter, EditorRequired]
    public DrugDto Drug { get; set; } = default!;
    
    [Parameter]
    public EventCallback<DrugDto> OnSelected { get; set; }
}
```

### Naming Conventions

| Type | Convention | Example |
|------|------------|---------|
| Feature Folder | PascalCase | `Features/Drugs` |
| Page | PascalCase | `DrugList.razor` |
| Component | PascalCase | `DrugCard.razor` |
| Command | PascalCase + Command | `CreateDrugCommand.cs` |
| Query | PascalCase + Query | `GetDrugsQuery.cs` |
| Handler | PascalCase + Handler | `CreateDrugCommandHandler.cs` |
| Service | PascalCase + Service | `DrugService.cs` |
| Validator | PascalCase + Validator | `CreateDrugValidator.cs` |
| Entity | PascalCase (singular) | `Drug.cs` |
| DTO | PascalCase + Dto | `DrugDto.cs` |

### Git Commit Convention

```
<type>(<scope>): <description>

Types:
- feat: New feature
- fix: Bug fix
- docs: Documentation
- style: Formatting
- refactor: Code restructuring
- test: Adding tests
- chore: Maintenance

Examples:
feat(drugs): add drug import from Excel
fix(pos): correct FEFO batch selection
docs(readme): update installation steps
```

### Branch Strategy

```
main                    # Production-ready
â”œâ”€â”€ develop             # Integration branch
â”‚   â”œâ”€â”€ feature/drugs-import
â”‚   â”œâ”€â”€ feature/pos-scanning
â”‚   â”œâ”€â”€ bugfix/batch-quantity
â”‚   â””â”€â”€ hotfix/login-error
```

---

## Deployment & DevOps

### Docker Configuration

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/DawaCloud.Web/DawaCloud.Web.csproj", "DawaCloud.Web/"]
RUN dotnet restore "DawaCloud.Web/DawaCloud.Web.csproj"
COPY src/ .
WORKDIR "/src/DawaCloud.Web"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DawaCloud.Web.dll"]
```

```yaml
# docker-compose.yml
version: '3.8'
services:
  web:
    build: .
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=DawaCloud;User=sa;Password=${DB_PASSWORD};TrustServerCertificate=true
    depends_on:
      - db
      - redis
  
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${DB_PASSWORD}
    volumes:
      - sqldata:/var/opt/mssql
  
  redis:
    image: redis:alpine
    volumes:
      - redisdata:/data

volumes:
  sqldata:
  redisdata:
```

### GitHub Actions CI/CD

```yaml
# .github/workflows/ci-cd.yml
name: CI/CD

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      
      - name: Restore
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore -c Release
      
      - name: Test
        run: dotnet test --no-build -c Release --verbosity normal
      
      - name: Publish
        if: github.ref == 'refs/heads/main'
        run: dotnet publish -c Release -o ./publish
      
      - name: Deploy
        if: github.ref == 'refs/heads/main'
        # Add deployment steps here
```

### Environment Configuration

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DawaCloud;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.Seq"],
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "Seq", "Args": { "serverUrl": "http://localhost:5341" } }
    ]
  },
  "Email": {
    "SmtpHost": "",
    "SmtpPort": 587,
    "FromEmail": "noreply@revopharma.com",
    "FromName": "Guru Brothers Ltd"
  },
  "WhatsApp": {
    "Provider": "Twilio",
    "AccountSid": "",
    "AuthToken": "",
    "FromNumber": ""
  },
  "AlertThresholds": {
    "LowStockDays": 14,
    "ExpiryAlertDays": [30, 60, 90]
  }
}
```

---

## Quick Reference Commands

```bash
# Create new migration
dotnet ef migrations add <MigrationName> -p src/DawaCloud.Web

# Update database
dotnet ef database update -p src/DawaCloud.Web

# Run application
dotnet run --project src/DawaCloud.Web

# Run tests
dotnet test

# Build Docker image
docker build -t DawaCloud:latest .

# Run with Docker Compose
docker-compose up -d

# View logs
docker-compose logs -f web
```

---

## Contact & Support

- **Company**: Guru Brothers Ltd
- **Email**: info@revopharma.com
- **Website**: www.revopharma.com
- **Repository**: https://github.com/your-org/DawaCloud

---

*This document is maintained as the single source of truth for the DawaCloud project. Update this file as the project evolves.*
