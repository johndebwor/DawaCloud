# DawaCloud - Pharmaceutical Management System

<div align="center">
  <img src="src/DawaCloud.Web/wwwroot/images/logo.svg" alt="DawaCloud Logo" width="200" />

  **DawaCloud**

  **Modern Drugs Wholesale & Retail Management System**

  *Currently deployed for Guru Brothers Ltd*
  
  [![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
  [![Blazor](https://img.shields.io/badge/Blazor-Server-purple)](https://blazor.net/)
  [![MudBlazor](https://img.shields.io/badge/MudBlazor-7.x-594AE2)](https://mudblazor.com/)
  [![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
</div>

---

## ðŸ“‹ Overview

**DawaCloud** (Dawa = Medicine in Swahili) is a comprehensive pharmaceutical management system designed to streamline drug inventory, procurement, wholesale and retail operations for pharmacies and drug distribution businesses in South Sudan and East Africa.

### Key Features

- **ðŸ¥ Drug Management**: Complete drug master data with batch tracking and expiry management
- **ðŸ“¦ Inventory Control**: Real-time stock levels, movements, adjustments, and reconciliation
- **ðŸ›’ Procurement**: Automated reordering, supplier management, and purchase workflows
- **ðŸ’³ Point of Sale**: Fast retail POS with barcode scanning and FEFO selection
- **ðŸ“Š Wholesale**: B2B customer management, quotations, invoicing, and credit control
- **ðŸ“ˆ Reports**: Comprehensive analytics for stock, sales, financial, and compliance reporting
- **ðŸ”” Notifications**: Email and WhatsApp alerts for stock and expiry management
- **ðŸ” Security**: Role-based access control with comprehensive audit logging

---

## ðŸš€ Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server 2022](https://www.microsoft.com/sql-server) (or Docker)
- [Node.js 18+](https://nodejs.org/) (for frontend tooling)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/DawaCloud.git
   cd DawaCloud
   ```

2. **Configure the database connection**
   
   Update `src/DawaCloud.Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=DawaCloud;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```

3. **Run database migrations**
   ```bash
   cd src/DawaCloud.Web
   dotnet ef database update
   ```

4. **Start the application**
   ```bash
   dotnet run
   ```

5. **Open your browser**
   
   Navigate to `https://localhost:5001`

### Demo Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@DawaCloud.com | Admin@123 |
| Pharmacist | pharmacist@DawaCloud.com | Demo@123 |
| Cashier | cashier@DawaCloud.com | Demo@123 |

---

## ðŸ—ï¸ Architecture

DawaCloud uses a **Feature-Based (Vertical Slice) Architecture** with CQRS pattern:

```
DawaCloud/
â”œâ”€â”€ src/
â”‚   â””â”€â”€ DawaCloud.Web/           # Main Blazor Server Application
â”‚       â”œâ”€â”€ Features/           # Feature modules (vertical slices)
â”‚       â”‚   â”œâ”€â”€ Auth/          # Authentication & Authorization
â”‚       â”‚   â”œâ”€â”€ Dashboard/     # Dashboard & KPIs
â”‚       â”‚   â”œâ”€â”€ Drugs/         # Drug management
â”‚       â”‚   â”œâ”€â”€ Batches/       # Batch & expiry tracking
â”‚       â”‚   â”œâ”€â”€ Inventory/     # Stock management
â”‚       â”‚   â”œâ”€â”€ Procurement/   # Drug requests & goods receiving
â”‚       â”‚   â”œâ”€â”€ Suppliers/     # Supplier management
â”‚       â”‚   â”œâ”€â”€ Wholesale/     # B2B sales & invoicing
â”‚       â”‚   â”œâ”€â”€ Retail/        # Point of Sale
â”‚       â”‚   â”œâ”€â”€ Reports/       # Analytics & reporting
â”‚       â”‚   â”œâ”€â”€ Notifications/ # Alert system
â”‚       â”‚   â”œâ”€â”€ Users/         # User management
â”‚       â”‚   â”œâ”€â”€ Settings/      # System configuration
â”‚       â”‚   â””â”€â”€ Audit/         # Activity logging
â”‚       â”œâ”€â”€ Data/              # Entity models & DbContext
â”‚       â”œâ”€â”€ Infrastructure/    # Cross-cutting concerns
â”‚       â””â”€â”€ Shared/            # Shared components
â”œâ”€â”€ tests/                     # Unit & Integration tests
â””â”€â”€ docs/                      # Documentation
```

---

## ðŸ› ï¸ Technology Stack

| Layer | Technology |
|-------|------------|
| **UI Framework** | Blazor Server |
| **Component Library** | MudBlazor 7.x |
| **Backend** | ASP.NET Core 10 |
| **Database** | SQL Server 2022 |
| **ORM** | Entity Framework Core 10 |
| **CQRS** | MediatR |
| **Validation** | FluentValidation |
| **Mapping** | Mapster |
| **Notifications** | MailKit, Twilio |
| **Reporting** | QuestPDF, ClosedXML |
| **Logging** | Serilog |

---

## ðŸ“– Documentation

- [CLAUDE.md](CLAUDE.md) - Comprehensive project documentation and implementation status
- [API Documentation](docs/api.md) - REST API reference
- [User Guide](docs/user-guide.md) - End-user documentation
- [Deployment Guide](docs/deployment.md) - Production deployment instructions

---

## ðŸ”§ Configuration

### Email (SMTP)
```json
{
  "Email": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "FromEmail": "noreply@DawaCloud.com",
    "FromName": "DawaCloud"
  }
}
```

### WhatsApp (Twilio)
```json
{
  "WhatsApp": {
    "Provider": "Twilio",
    "AccountSid": "your-account-sid",
    "AuthToken": "your-auth-token",
    "FromNumber": "+211XXXXXXXXX"
  }
}
```

### Alert Thresholds
```json
{
  "AlertThresholds": {
    "LowStockDays": 14,
    "ExpiryAlertDays": [30, 60, 90]
  }
}
```

---

## ðŸ³ Docker Deployment

```bash
# Build the image
docker build -t DawaCloud:latest .

# Run with Docker Compose
docker-compose up -d
```

---

## ðŸ¤ Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## ðŸ“œ License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## ðŸ“ž Support

- **Company**: Guru Brothers Ltd
- **Email**: info@revopharma.com
- **Website**: www.revopharma.com
- **Phone**: +211 920 123456
- **Location**: Mobil, Hai Cinema, Juba, South Sudan
- **Issues**: [GitHub Issues](https://github.com/your-org/DawaCloud/issues)

---

## ðŸ¢ About This Installation

This DawaCloud system is currently deployed for **Guru Brothers Ltd**, a leading pharmaceutical distribution company serving South Sudan.

**About DawaCloud:**
DawaCloud is a flexible pharmaceutical management platform that can be configured for any pharmacy or pharmaceutical distribution business. The system provides comprehensive tools to streamline drug inventory, procurement, and distribution operations.

**About Guru Brothers Ltd (Current Client):**
Guru Brothers Ltd uses DawaCloud to manage their pharmaceutical operations in South Sudan. The system is configured with features tailored to South Sudanese regulatory requirements and business practices.

---

<div align="center">
  DawaCloud - Pharmaceutical Management System<br/>
  Serving pharmacies and pharmaceutical businesses across South Sudan and East Africa
</div>
