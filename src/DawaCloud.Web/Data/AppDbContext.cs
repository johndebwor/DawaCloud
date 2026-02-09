using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Drug Management
    public DbSet<Drug> Drugs => Set<Drug>();
    public DbSet<DrugCategory> DrugCategories => Set<DrugCategory>();
    public DbSet<Batch> Batches => Set<Batch>();

    // Supplier Management
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierContact> SupplierContacts => Set<SupplierContact>();

    // Procurement
    public DbSet<DrugRequest> DrugRequests => Set<DrugRequest>();
    public DbSet<DrugRequestItem> DrugRequestItems => Set<DrugRequestItem>();
    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
    public DbSet<GoodsReceiptItem> GoodsReceiptItems => Set<GoodsReceiptItem>();

    // Inventory Management
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<StockCount> StockCounts => Set<StockCount>();
    public DbSet<StockCountItem> StockCountItems => Set<StockCountItem>();

    // Wholesale
    public DbSet<WholesaleCustomer> WholesaleCustomers => Set<WholesaleCustomer>();
    public DbSet<WholesaleSale> WholesaleSales => Set<WholesaleSale>();
    public DbSet<WholesaleSaleItem> WholesaleSaleItems => Set<WholesaleSaleItem>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<QuotationItem> QuotationItems => Set<QuotationItem>();
    public DbSet<Payment> Payments => Set<Payment>();

    // Retail
    public DbSet<RetailSale> RetailSales => Set<RetailSale>();
    public DbSet<RetailSaleItem> RetailSaleItems => Set<RetailSaleItem>();
    public DbSet<HeldSale> HeldSales => Set<HeldSale>();
    public DbSet<CashierShift> CashierShifts => Set<CashierShift>();

    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();

    // Settings
    public DbSet<CompanySettings> CompanySettings => Set<CompanySettings>();
    public DbSet<TaxCategory> TaxCategories => Set<TaxCategory>();

    // Currency & Exchange Rates
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
    public DbSet<ExchangeRateHistory> ExchangeRateHistories => Set<ExchangeRateHistory>();

    // Audit
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Insurance
    public DbSet<InsuranceProvider> InsuranceProviders => Set<InsuranceProvider>();
    public DbSet<InsuredPatient> InsuredPatients => Set<InsuredPatient>();
    public DbSet<InsuranceClaim> InsuranceClaims => Set<InsuranceClaim>();
    public DbSet<InsuranceClaimItem> InsuranceClaimItems => Set<InsuranceClaimItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global query filters for soft delete
        modelBuilder.Entity<Drug>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<DrugCategory>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Batch>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Supplier>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<SupplierContact>().HasQueryFilter(e => !e.IsDeleted);

        // Configure relationships
        modelBuilder.Entity<DrugCategory>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Drug>()
            .HasOne(d => d.Category)
            .WithMany(c => c.Drugs)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Batch>()
            .HasOne(b => b.Drug)
            .WithMany(d => d.Batches)
            .HasForeignKey(b => b.DrugId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SupplierContact>()
            .HasOne(sc => sc.Supplier)
            .WithMany(s => s.Contacts)
            .HasForeignKey(sc => sc.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StockMovement>()
            .HasOne(sm => sm.Drug)
            .WithMany()
            .HasForeignKey(sm => sm.DrugId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockMovement>()
            .HasOne(sm => sm.Batch)
            .WithMany()
            .HasForeignKey(sm => sm.BatchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockCountItem>()
            .HasOne(sci => sci.StockCount)
            .WithMany(sc => sc.Items)
            .HasForeignKey(sci => sci.StockCountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StockCountItem>()
            .HasOne(sci => sci.Drug)
            .WithMany()
            .HasForeignKey(sci => sci.DrugId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockCountItem>()
            .HasOne(sci => sci.Batch)
            .WithMany()
            .HasForeignKey(sci => sci.BatchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DrugRequest>()
            .HasOne(dr => dr.Supplier)
            .WithMany()
            .HasForeignKey(dr => dr.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DrugRequestItem>()
            .HasOne(dri => dri.DrugRequest)
            .WithMany(dr => dr.Items)
            .HasForeignKey(dri => dri.DrugRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DrugRequestItem>()
            .HasOne(dri => dri.Drug)
            .WithMany()
            .HasForeignKey(dri => dri.DrugId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure indexes
        modelBuilder.Entity<Drug>()
            .HasIndex(d => d.Code)
            .IsUnique();

        modelBuilder.Entity<Drug>()
            .HasIndex(d => d.Barcode);

        modelBuilder.Entity<Drug>()
            .HasIndex(d => d.Name);

        modelBuilder.Entity<Batch>()
            .HasIndex(b => b.BatchNumber);

        modelBuilder.Entity<Batch>()
            .HasIndex(b => b.ExpiryDate);

        modelBuilder.Entity<Supplier>()
            .HasIndex(s => s.Code)
            .IsUnique();

        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.Timestamp);

        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.UserId);

        modelBuilder.Entity<StockMovement>()
            .HasIndex(sm => sm.DrugId);

        modelBuilder.Entity<StockMovement>()
            .HasIndex(sm => sm.BatchId);

        modelBuilder.Entity<StockMovement>()
            .HasIndex(sm => sm.CreatedAt);

        modelBuilder.Entity<StockCount>()
            .HasIndex(sc => sc.ReferenceNumber)
            .IsUnique();

        modelBuilder.Entity<StockCount>()
            .HasIndex(sc => sc.CountDate);

        modelBuilder.Entity<StockCount>()
            .HasIndex(sc => sc.Status);

        modelBuilder.Entity<StockCountItem>()
            .HasIndex(sci => sci.StockCountId);

        modelBuilder.Entity<DrugRequest>()
            .HasIndex(dr => dr.RequestNumber)
            .IsUnique();

        modelBuilder.Entity<DrugRequest>()
            .HasIndex(dr => dr.SupplierId);

        modelBuilder.Entity<DrugRequest>()
            .HasIndex(dr => dr.Status);

        modelBuilder.Entity<DrugRequest>()
            .HasIndex(dr => dr.RequestDate);

        modelBuilder.Entity<DrugRequestItem>()
            .HasIndex(dri => dri.DrugRequestId);

        // Retail Sale configurations
        modelBuilder.Entity<RetailSaleItem>()
            .HasOne(rsi => rsi.Sale)
            .WithMany(rs => rs.Items)
            .HasForeignKey(rsi => rsi.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RetailSaleItem>()
            .HasOne(rsi => rsi.Drug)
            .WithMany()
            .HasForeignKey(rsi => rsi.DrugId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RetailSaleItem>()
            .HasOne(rsi => rsi.Batch)
            .WithMany()
            .HasForeignKey(rsi => rsi.BatchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Wholesale Sale configurations
        modelBuilder.Entity<WholesaleSale>()
            .HasOne(ws => ws.Customer)
            .WithMany(c => c.Sales)
            .HasForeignKey(ws => ws.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WholesaleSaleItem>()
            .HasOne(wsi => wsi.Sale)
            .WithMany(ws => ws.Items)
            .HasForeignKey(wsi => wsi.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WholesaleSaleItem>()
            .HasOne(wsi => wsi.Drug)
            .WithMany()
            .HasForeignKey(wsi => wsi.DrugId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WholesaleSaleItem>()
            .HasOne(wsi => wsi.Batch)
            .WithMany()
            .HasForeignKey(wsi => wsi.BatchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Quotation configurations
        modelBuilder.Entity<Quotation>()
            .HasOne(q => q.Customer)
            .WithMany()
            .HasForeignKey(q => q.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuotationItem>()
            .HasOne(qi => qi.Quotation)
            .WithMany(q => q.Items)
            .HasForeignKey(qi => qi.QuotationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuotationItem>()
            .HasOne(qi => qi.Drug)
            .WithMany()
            .HasForeignKey(qi => qi.DrugId)
            .OnDelete(DeleteBehavior.Restrict);

        // Goods Receipt configurations
        modelBuilder.Entity<GoodsReceipt>()
            .HasOne(gr => gr.Supplier)
            .WithMany()
            .HasForeignKey(gr => gr.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GoodsReceipt>()
            .HasOne(gr => gr.DrugRequest)
            .WithMany()
            .HasForeignKey(gr => gr.DrugRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GoodsReceiptItem>()
            .HasOne(gri => gri.GoodsReceipt)
            .WithMany(gr => gr.Items)
            .HasForeignKey(gri => gri.GoodsReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GoodsReceiptItem>()
            .HasOne(gri => gri.Drug)
            .WithMany()
            .HasForeignKey(gri => gri.DrugId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GoodsReceiptItem>()
            .HasOne(gri => gri.Batch)
            .WithMany()
            .HasForeignKey(gri => gri.BatchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Payment configurations
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.WholesaleSale)
            .WithMany(ws => ws.Payments)
            .HasForeignKey(p => p.WholesaleSaleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.RetailSale)
            .WithMany(rs => rs.Payments)
            .HasForeignKey(p => p.RetailSaleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Currency configurations
        modelBuilder.Entity<Currency>()
            .HasIndex(c => c.Code)
            .IsUnique();

        modelBuilder.Entity<Currency>()
            .Property(c => c.Code)
            .HasMaxLength(3)
            .IsRequired();

        modelBuilder.Entity<Currency>()
            .Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Currency>()
            .Property(c => c.Symbol)
            .HasMaxLength(10)
            .IsRequired();

        modelBuilder.Entity<Currency>()
            .Property(c => c.Format)
            .HasMaxLength(50)
            .IsRequired();

        // ExchangeRate configurations
        modelBuilder.Entity<ExchangeRate>()
            .HasOne(er => er.FromCurrency)
            .WithMany()
            .HasForeignKey(er => er.FromCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExchangeRate>()
            .HasOne(er => er.ToCurrency)
            .WithMany()
            .HasForeignKey(er => er.ToCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExchangeRate>()
            .HasIndex(er => new { er.FromCurrencyId, er.ToCurrencyId, er.IsActive });

        modelBuilder.Entity<ExchangeRate>()
            .HasIndex(er => er.EffectiveDate);

        modelBuilder.Entity<ExchangeRate>()
            .Property(er => er.Rate)
            .HasPrecision(18, 6);

        // ExchangeRateHistory configurations
        modelBuilder.Entity<ExchangeRateHistory>()
            .HasOne(erh => erh.ExchangeRate)
            .WithMany(er => er.History)
            .HasForeignKey(erh => erh.ExchangeRateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExchangeRateHistory>()
            .HasIndex(erh => erh.ExchangeRateId);

        modelBuilder.Entity<ExchangeRateHistory>()
            .HasIndex(erh => erh.ChangedAt);

        modelBuilder.Entity<ExchangeRateHistory>()
            .Property(erh => erh.PreviousRate)
            .HasPrecision(18, 6);

        modelBuilder.Entity<ExchangeRateHistory>()
            .Property(erh => erh.NewRate)
            .HasPrecision(18, 6);

        modelBuilder.Entity<ExchangeRateHistory>()
            .Property(erh => erh.ChangedBy)
            .HasMaxLength(256)
            .IsRequired();

        // Insurance configurations
        modelBuilder.Entity<InsuredPatient>()
            .HasOne(ip => ip.Provider)
            .WithMany(p => p.Patients)
            .HasForeignKey(ip => ip.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceClaim>()
            .HasOne(ic => ic.Provider)
            .WithMany(p => p.Claims)
            .HasForeignKey(ic => ic.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceClaim>()
            .HasOne(ic => ic.Patient)
            .WithMany(p => p.Claims)
            .HasForeignKey(ic => ic.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceClaim>()
            .HasOne(ic => ic.Sale)
            .WithMany()
            .HasForeignKey(ic => ic.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceClaimItem>()
            .HasOne(ici => ici.Claim)
            .WithMany(ic => ic.Items)
            .HasForeignKey(ici => ici.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InsuranceClaimItem>()
            .HasOne(ici => ici.Drug)
            .WithMany()
            .HasForeignKey(ici => ici.DrugId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InsuranceClaimItem>()
            .HasOne(ici => ici.Batch)
            .WithMany()
            .HasForeignKey(ici => ici.BatchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure decimal precision for money/amount fields
        ConfigureDecimalPrecision(modelBuilder);
    }

    private static void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
    {
        // Drug entity
        modelBuilder.Entity<Drug>()
            .Property(d => d.CostPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Drug>()
            .Property(d => d.CostPriceOriginal).HasPrecision(18, 2);
        modelBuilder.Entity<Drug>()
            .Property(d => d.RetailPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Drug>()
            .Property(d => d.WholesalePrice).HasPrecision(18, 2);
        modelBuilder.Entity<Drug>()
            .Property(d => d.TaxRate).HasPrecision(5, 2);

        // Batch entity
        modelBuilder.Entity<Batch>()
            .Property(b => b.CostPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Batch>()
            .Property(b => b.CostPriceOriginal).HasPrecision(18, 2);
        modelBuilder.Entity<Batch>()
            .Property(b => b.ExchangeRateUsed).HasPrecision(18, 6);

        // Currency entities
        modelBuilder.Entity<ExchangeRate>()
            .Property(er => er.Rate).HasPrecision(18, 6);

        // Company Settings
        modelBuilder.Entity<CompanySettings>()
            .Property(cs => cs.DefaultTaxRate).HasPrecision(5, 2);

        // Cashier Shift
        modelBuilder.Entity<CashierShift>()
            .Property(cs => cs.OpeningBalance).HasPrecision(18, 2);
        modelBuilder.Entity<CashierShift>()
            .Property(cs => cs.ClosingBalance).HasPrecision(18, 2);
        modelBuilder.Entity<CashierShift>()
            .Property(cs => cs.ExpectedBalance).HasPrecision(18, 2);
        modelBuilder.Entity<CashierShift>()
            .Property(cs => cs.Variance).HasPrecision(18, 2);

        // Stock Count
        modelBuilder.Entity<StockCount>()
            .Property(sc => sc.TotalVarianceValue).HasPrecision(18, 2);
        modelBuilder.Entity<StockCountItem>()
            .Property(sci => sci.VarianceValue).HasPrecision(18, 2);

        // Drug Request
        modelBuilder.Entity<DrugRequest>()
            .Property(dr => dr.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<DrugRequest>()
            .Property(dr => dr.TotalAmountBase).HasPrecision(18, 2);
        modelBuilder.Entity<DrugRequest>()
            .Property(dr => dr.ActualAmount).HasPrecision(18, 2);
        modelBuilder.Entity<DrugRequest>()
            .Property(dr => dr.ActualAmountBase).HasPrecision(18, 2);
        modelBuilder.Entity<DrugRequest>()
            .Property(dr => dr.ExchangeRateUsed).HasPrecision(18, 6);

        modelBuilder.Entity<DrugRequestItem>()
            .Property(dri => dri.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<DrugRequestItem>()
            .Property(dri => dri.UnitPriceBase).HasPrecision(18, 2);
        modelBuilder.Entity<DrugRequestItem>()
            .Property(dri => dri.TotalPrice).HasPrecision(18, 2);
        modelBuilder.Entity<DrugRequestItem>()
            .Property(dri => dri.TotalPriceBase).HasPrecision(18, 2);
        modelBuilder.Entity<DrugRequestItem>()
            .Property(dri => dri.QuotedPrice).HasPrecision(18, 2);
        modelBuilder.Entity<DrugRequestItem>()
            .Property(dri => dri.QuotedPriceBase).HasPrecision(18, 2);

        // Goods Receipt
        modelBuilder.Entity<GoodsReceipt>()
            .Property(gr => gr.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<GoodsReceipt>()
            .Property(gr => gr.ExchangeRateUsed).HasPrecision(18, 6);

        // Wholesale Customer
        modelBuilder.Entity<WholesaleCustomer>()
            .Property(wc => wc.CreditLimit).HasPrecision(18, 2);
        modelBuilder.Entity<WholesaleCustomer>()
            .Property(wc => wc.CurrentBalance).HasPrecision(18, 2);

        // Wholesale Sale
        modelBuilder.Entity<WholesaleSale>()
            .Property(ws => ws.SubTotal).HasPrecision(18, 2);
        modelBuilder.Entity<WholesaleSale>()
            .Property(ws => ws.TaxAmount).HasPrecision(18, 2);
        modelBuilder.Entity<WholesaleSale>()
            .Property(ws => ws.DiscountAmount).HasPrecision(18, 2);
        modelBuilder.Entity<WholesaleSale>()
            .Property(ws => ws.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<WholesaleSale>()
            .Property(ws => ws.PaidAmount).HasPrecision(18, 2);
        modelBuilder.Entity<WholesaleSale>()
            .Property(ws => ws.ExchangeRateUsed).HasPrecision(18, 6);

        modelBuilder.Entity<WholesaleSaleItem>()
            .Property(wsi => wsi.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<WholesaleSaleItem>()
            .Property(wsi => wsi.DiscountPercent).HasPrecision(5, 2);
        modelBuilder.Entity<WholesaleSaleItem>()
            .Property(wsi => wsi.TaxRate).HasPrecision(5, 2);
        modelBuilder.Entity<WholesaleSaleItem>()
            .Property(wsi => wsi.LineTotal).HasPrecision(18, 2);

        // Quotation
        modelBuilder.Entity<Quotation>()
            .Property(q => q.SubTotal).HasPrecision(18, 2);
        modelBuilder.Entity<Quotation>()
            .Property(q => q.TaxAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Quotation>()
            .Property(q => q.DiscountAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Quotation>()
            .Property(q => q.TotalAmount).HasPrecision(18, 2);

        modelBuilder.Entity<QuotationItem>()
            .Property(qi => qi.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<QuotationItem>()
            .Property(qi => qi.DiscountPercent).HasPrecision(5, 2);
        modelBuilder.Entity<QuotationItem>()
            .Property(qi => qi.LineTotal).HasPrecision(18, 2);

        // Payment
        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount).HasPrecision(18, 2);

        // Retail Sale
        modelBuilder.Entity<RetailSale>()
            .Property(rs => rs.SubTotal).HasPrecision(18, 2);
        modelBuilder.Entity<RetailSale>()
            .Property(rs => rs.TaxAmount).HasPrecision(18, 2);
        modelBuilder.Entity<RetailSale>()
            .Property(rs => rs.DiscountAmount).HasPrecision(18, 2);
        modelBuilder.Entity<RetailSale>()
            .Property(rs => rs.TotalAmount).HasPrecision(18, 2);

        modelBuilder.Entity<RetailSaleItem>()
            .Property(rsi => rsi.UnitPrice).HasPrecision(18, 2);

        // Insurance
        modelBuilder.Entity<InsuranceClaim>()
            .Property(ic => ic.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<InsuranceClaim>()
            .Property(ic => ic.CoveredAmount).HasPrecision(18, 2);

        modelBuilder.Entity<InsuranceClaimItem>()
            .Property(ici => ici.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<InsuranceClaimItem>()
            .Property(ici => ici.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<InsuranceClaimItem>()
            .Property(ici => ici.CoveredAmount).HasPrecision(18, 2);
    }
}
