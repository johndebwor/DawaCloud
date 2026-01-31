using DawaFlow.Web.Data.Entities;
using DawaFlow.Web.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Data;

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

    // Audit
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

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
    }
}
