using DawaCloud.Web.Data;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Features.Reports.Models;
using Microsoft.EntityFrameworkCore;

namespace DawaCloud.Web.Features.Reports.Services;

public interface IReportService
{
    Task<ReportResult> GenerateReportAsync(string reportType, DateTime? startDate, DateTime? endDate);
}

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ReportResult> GenerateReportAsync(string reportType, DateTime? startDate, DateTime? endDate)
    {
        return reportType switch
        {
            // Inventory
            "stock-levels" => await GetStockLevelsReport(),
            "stock-valuation" => await GetStockValuationReport(),
            "stock-movements" => await GetStockMovementsReport(startDate, endDate),
            "expiry" => await GetExpiryReport(startDate, endDate),
            "low-stock" => await GetLowStockReport(),

            // Sales
            "daily-sales" => await GetDailySalesReport(startDate, endDate),
            "monthly-sales" => await GetMonthlySalesReport(startDate, endDate),
            "sales-by-product" => await GetSalesByProductReport(startDate, endDate),
            "sales-by-customer" => await GetSalesByCustomerReport(startDate, endDate),
            "cashier-summary" => await GetCashierSummaryReport(startDate, endDate),

            // Financial
            "profit-loss" => await GetProfitLossReport(startDate, endDate),
            "outstanding-payments" => await GetOutstandingPaymentsReport(),
            "receivables-aging" => await GetReceivablesAgingReport(),
            "payment-history" => await GetPaymentHistoryReport(startDate, endDate),
            "tax-summary" => await GetTaxSummaryReport(startDate, endDate),

            // Supplier
            "purchase-summary" => await GetPurchaseSummaryReport(startDate, endDate),
            "supplier-performance" => await GetSupplierPerformanceReport(startDate, endDate),
            "goods-received" => await GetGoodsReceivedReport(startDate, endDate),
            "pending-orders" => await GetPendingOrdersReport(),

            // Compliance
            "controlled-drugs" => await GetControlledDrugsReport(startDate, endDate),
            "prescription-log" => await GetPrescriptionLogReport(startDate, endDate),
            "batch-traceability" => await GetBatchTraceabilityReport(startDate, endDate),
            "disposal-log" => await GetDisposalLogReport(startDate, endDate),

            // Audit
            "user-activity" => await GetUserActivityReport(startDate, endDate),
            "login-history" => await GetLoginHistoryReport(startDate, endDate),
            "data-changes" => await GetDataChangesReport(startDate, endDate),
            "system-log" => await GetSystemLogReport(startDate, endDate),

            // Expenses
            "expenses-by-category" => await GetExpensesByCategoryReport(startDate, endDate),
            "expenses-by-payment-method" => await GetExpensesByPaymentMethodReport(startDate, endDate),
            "expenses-monthly-summary" => await GetExpensesMonthlySummaryReport(startDate, endDate),
            "expenses-by-vendor" => await GetExpensesByVendorReport(startDate, endDate),
            "expenses-recurring" => await GetRecurringExpensesReport(),
            "expenses-budget-vs-actual" => await GetBudgetVsActualReport(startDate, endDate),

            _ => new ReportResult { Title = "Unknown Report" }
        };
    }

    // ========== INVENTORY REPORTS ==========

    private async Task<ReportResult> GetStockLevelsReport()
    {
        var drugs = await _context.Drugs
            .Include(d => d.Category)
            .Include(d => d.Batches.Where(b => b.Status == BatchStatus.Active))
            .Where(d => !d.IsDeleted && d.IsActive)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Stock Levels Report",
            SubTitle = "Current stock levels for all active drugs",
            Columns = new()
            {
                new() { Key = "code", Label = "Drug Code", Format = ColumnFormat.Text },
                new() { Key = "name", Label = "Drug Name", Format = ColumnFormat.Text },
                new() { Key = "category", Label = "Category", Format = ColumnFormat.Text },
                new() { Key = "available", Label = "Available Qty", Format = ColumnFormat.Number },
                new() { Key = "reserved", Label = "Reserved Qty", Format = ColumnFormat.Number },
                new() { Key = "reorderLevel", Label = "Reorder Level", Format = ColumnFormat.Number },
                new() { Key = "value", Label = "Stock Value (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "status", Label = "Status", Format = ColumnFormat.Text }
            }
        };

        foreach (var d in drugs)
        {
            var available = d.Batches.Sum(b => b.CurrentQuantity);
            var reserved = d.Batches.Sum(b => b.ReservedQuantity);
            var value = d.Batches.Sum(b => b.CurrentQuantity * b.CostPrice);
            var status = available == 0 ? "Out of Stock" : available < d.ReorderLevel ? "Low Stock" : "Good";

            result.Rows.Add(new()
            {
                ["code"] = d.Code,
                ["name"] = d.Name,
                ["category"] = d.Category?.Name ?? "N/A",
                ["available"] = available,
                ["reserved"] = reserved,
                ["reorderLevel"] = d.ReorderLevel,
                ["value"] = value,
                ["status"] = status
            });
        }

        result.Summary["Total SKUs"] = drugs.Count;
        result.Summary["Total Stock Value"] = result.Rows.Sum(r => (decimal)(r["value"] ?? 0m));
        result.Summary["Low Stock Items"] = result.Rows.Count(r => (string)(r["status"] ?? "") == "Low Stock");
        result.Summary["Out of Stock Items"] = result.Rows.Count(r => (string)(r["status"] ?? "") == "Out of Stock");

        return result;
    }

    private async Task<ReportResult> GetStockValuationReport()
    {
        var drugs = await _context.Drugs
            .Include(d => d.Category)
            .Include(d => d.Batches.Where(b => b.Status == BatchStatus.Active))
            .Where(d => !d.IsDeleted && d.IsActive)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Stock Valuation Report",
            SubTitle = "Current inventory valuation at cost price",
            Columns = new()
            {
                new() { Key = "code", Label = "Drug Code", Format = ColumnFormat.Text },
                new() { Key = "name", Label = "Drug Name", Format = ColumnFormat.Text },
                new() { Key = "category", Label = "Category", Format = ColumnFormat.Text },
                new() { Key = "quantity", Label = "Quantity", Format = ColumnFormat.Number },
                new() { Key = "avgCost", Label = "Avg Cost (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "totalValue", Label = "Total Value (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "percentOfTotal", Label = "% of Total", Format = ColumnFormat.Percent }
            }
        };

        var totalInventoryValue = 0m;
        var valuations = new List<(Dictionary<string, object?> Row, decimal Value)>();

        foreach (var d in drugs)
        {
            var qty = d.Batches.Sum(b => b.CurrentQuantity);
            if (qty == 0) continue;
            var totalValue = d.Batches.Sum(b => b.CurrentQuantity * b.CostPrice);
            var avgCost = totalValue / qty;
            totalInventoryValue += totalValue;

            var row = new Dictionary<string, object?>
            {
                ["code"] = d.Code,
                ["name"] = d.Name,
                ["category"] = d.Category?.Name ?? "N/A",
                ["quantity"] = qty,
                ["avgCost"] = avgCost,
                ["totalValue"] = totalValue,
                ["percentOfTotal"] = 0m
            };
            valuations.Add((row, totalValue));
        }

        foreach (var (row, value) in valuations)
        {
            row["percentOfTotal"] = totalInventoryValue > 0 ? Math.Round(value / totalInventoryValue * 100, 2) : 0m;
            result.Rows.Add(row);
        }

        result.Rows = result.Rows.OrderByDescending(r => (decimal)(r["totalValue"] ?? 0m)).ToList();
        result.Summary["Total Items"] = valuations.Count;
        result.Summary["Total Inventory Value"] = totalInventoryValue;

        return result;
    }

    private async Task<ReportResult> GetStockMovementsReport(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.StockMovements
            .Include(sm => sm.Drug)
            .Include(sm => sm.Batch)
            .AsNoTracking()
            .AsQueryable();

        if (startDate.HasValue) query = query.Where(sm => sm.CreatedAt >= startDate.Value);
        if (endDate.HasValue) query = query.Where(sm => sm.CreatedAt < endDate.Value.AddDays(1));

        var movements = await query.OrderByDescending(sm => sm.CreatedAt).Take(5000).ToListAsync();

        var result = new ReportResult
        {
            Title = "Stock Movements Report",
            SubTitle = $"All inventory movements",
            StartDate = startDate,
            EndDate = endDate,
            Columns = new()
            {
                new() { Key = "date", Label = "Date", Format = ColumnFormat.DateTime },
                new() { Key = "drug", Label = "Drug", Format = ColumnFormat.Text },
                new() { Key = "batch", Label = "Batch", Format = ColumnFormat.Text },
                new() { Key = "type", Label = "Type", Format = ColumnFormat.Text },
                new() { Key = "quantity", Label = "Quantity", Format = ColumnFormat.Number },
                new() { Key = "balanceBefore", Label = "Before", Format = ColumnFormat.Number },
                new() { Key = "balanceAfter", Label = "After", Format = ColumnFormat.Number },
                new() { Key = "reference", Label = "Reference", Format = ColumnFormat.Text }
            }
        };

        foreach (var sm in movements)
        {
            result.Rows.Add(new()
            {
                ["date"] = sm.CreatedAt,
                ["drug"] = sm.Drug?.Name ?? "N/A",
                ["batch"] = sm.Batch?.BatchNumber ?? "N/A",
                ["type"] = sm.Type.ToString(),
                ["quantity"] = sm.Quantity,
                ["balanceBefore"] = sm.BalanceBefore,
                ["balanceAfter"] = sm.BalanceAfter,
                ["reference"] = sm.Reference ?? ""
            });
        }

        result.Summary["Total Movements"] = movements.Count;
        result.Summary["Purchases"] = movements.Count(m => m.Type == MovementType.Purchase);
        result.Summary["Sales"] = movements.Count(m => m.Type == MovementType.Sale);
        result.Summary["Adjustments"] = movements.Count(m => m.Type == MovementType.Adjustment);

        return result;
    }

    private async Task<ReportResult> GetExpiryReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow;
        var to = endDate ?? DateTime.UtcNow.AddDays(90);

        var batches = await _context.Batches
            .Include(b => b.Drug)
            .Where(b => !b.IsDeleted && b.CurrentQuantity > 0
                && b.ExpiryDate >= from && b.ExpiryDate <= to)
            .OrderBy(b => b.ExpiryDate)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Expiry Report",
            SubTitle = $"Batches expiring between {from:dd/MM/yyyy} and {to:dd/MM/yyyy}",
            StartDate = startDate,
            EndDate = endDate,
            Columns = new()
            {
                new() { Key = "drug", Label = "Drug", Format = ColumnFormat.Text },
                new() { Key = "batch", Label = "Batch #", Format = ColumnFormat.Text },
                new() { Key = "expiryDate", Label = "Expiry Date", Format = ColumnFormat.Date },
                new() { Key = "daysUntil", Label = "Days Until Expiry", Format = ColumnFormat.Number },
                new() { Key = "quantity", Label = "Quantity", Format = ColumnFormat.Number },
                new() { Key = "value", Label = "Value (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "status", Label = "Status", Format = ColumnFormat.Text }
            }
        };

        foreach (var b in batches)
        {
            var daysUntil = (b.ExpiryDate - DateTime.UtcNow).Days;
            result.Rows.Add(new()
            {
                ["drug"] = b.Drug?.Name ?? "N/A",
                ["batch"] = b.BatchNumber,
                ["expiryDate"] = b.ExpiryDate,
                ["daysUntil"] = daysUntil,
                ["quantity"] = b.CurrentQuantity,
                ["value"] = b.CurrentQuantity * b.CostPrice,
                ["status"] = daysUntil < 0 ? "Expired" : daysUntil <= 30 ? "Critical" : daysUntil <= 60 ? "Warning" : "Approaching"
            });
        }

        result.Summary["Total Batches"] = batches.Count;
        result.Summary["Expired"] = result.Rows.Count(r => (string)(r["status"] ?? "") == "Expired");
        result.Summary["Critical (< 30 days)"] = result.Rows.Count(r => (string)(r["status"] ?? "") == "Critical");
        result.Summary["Total Value at Risk"] = result.Rows.Sum(r => (decimal)(r["value"] ?? 0m));

        return result;
    }

    private async Task<ReportResult> GetLowStockReport()
    {
        var drugs = await _context.Drugs
            .Include(d => d.Category)
            .Include(d => d.Batches.Where(b => b.Status == BatchStatus.Active))
            .Where(d => !d.IsDeleted && d.IsActive)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Low Stock Alert Report",
            SubTitle = "Drugs below reorder level",
            Columns = new()
            {
                new() { Key = "code", Label = "Drug Code", Format = ColumnFormat.Text },
                new() { Key = "name", Label = "Drug Name", Format = ColumnFormat.Text },
                new() { Key = "category", Label = "Category", Format = ColumnFormat.Text },
                new() { Key = "available", Label = "Available", Format = ColumnFormat.Number },
                new() { Key = "reorderLevel", Label = "Reorder Level", Format = ColumnFormat.Number },
                new() { Key = "deficit", Label = "Deficit", Format = ColumnFormat.Number },
                new() { Key = "status", Label = "Status", Format = ColumnFormat.Text }
            }
        };

        foreach (var d in drugs)
        {
            var available = d.Batches.Sum(b => b.CurrentQuantity);
            if (available >= d.ReorderLevel) continue;

            result.Rows.Add(new()
            {
                ["code"] = d.Code,
                ["name"] = d.Name,
                ["category"] = d.Category?.Name ?? "N/A",
                ["available"] = available,
                ["reorderLevel"] = d.ReorderLevel,
                ["deficit"] = d.ReorderLevel - available,
                ["status"] = available == 0 ? "Out of Stock" : "Low Stock"
            });
        }

        result.Rows = result.Rows.OrderBy(r => (int)(r["available"] ?? 0)).ToList();
        result.Summary["Low Stock Items"] = result.Rows.Count;
        result.Summary["Out of Stock"] = result.Rows.Count(r => (string)(r["status"] ?? "") == "Out of Stock");

        return result;
    }

    // ========== SALES REPORTS ==========

    private async Task<ReportResult> GetDailySalesReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var retailSales = await _context.RetailSales
            .Where(s => !s.IsDeleted && s.Status == RetailSaleStatus.Completed
                && s.SaleDate >= from && s.SaleDate < to.AddDays(1))
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => new { Date = g.Key, Count = g.Count(), Total = g.Sum(s => s.TotalAmount) })
            .ToListAsync();

        var wholesaleSales = await _context.WholesaleSales
            .Where(s => !s.IsDeleted && s.Status != SaleStatus.Cancelled
                && s.SaleDate >= from && s.SaleDate < to.AddDays(1))
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => new { Date = g.Key, Count = g.Count(), Total = g.Sum(s => s.TotalAmount) })
            .ToListAsync();

        var allDates = retailSales.Select(r => r.Date)
            .Union(wholesaleSales.Select(w => w.Date))
            .Distinct().OrderBy(d => d).ToList();

        var result = new ReportResult
        {
            Title = "Daily Sales Report",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "date", Label = "Date", Format = ColumnFormat.Date },
                new() { Key = "retailCount", Label = "Retail Txns", Format = ColumnFormat.Number },
                new() { Key = "retailTotal", Label = "Retail Total (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "wholesaleCount", Label = "Wholesale Txns", Format = ColumnFormat.Number },
                new() { Key = "wholesaleTotal", Label = "Wholesale Total (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "combinedTotal", Label = "Combined Total (SSP)", Format = ColumnFormat.Currency }
            }
        };

        foreach (var date in allDates)
        {
            var retail = retailSales.FirstOrDefault(r => r.Date == date);
            var wholesale = wholesaleSales.FirstOrDefault(w => w.Date == date);
            result.Rows.Add(new()
            {
                ["date"] = date,
                ["retailCount"] = retail?.Count ?? 0,
                ["retailTotal"] = retail?.Total ?? 0m,
                ["wholesaleCount"] = wholesale?.Count ?? 0,
                ["wholesaleTotal"] = wholesale?.Total ?? 0m,
                ["combinedTotal"] = (retail?.Total ?? 0m) + (wholesale?.Total ?? 0m)
            });
        }

        result.Summary["Total Retail Sales"] = retailSales.Sum(r => r.Total);
        result.Summary["Total Wholesale Sales"] = wholesaleSales.Sum(w => w.Total);
        result.Summary["Grand Total"] = retailSales.Sum(r => r.Total) + wholesaleSales.Sum(w => w.Total);
        result.Summary["Days with Sales"] = allDates.Count;

        return result;
    }

    private async Task<ReportResult> GetMonthlySalesReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-12);
        var to = endDate ?? DateTime.UtcNow;

        var retailSales = await _context.RetailSales
            .Where(s => !s.IsDeleted && s.Status == RetailSaleStatus.Completed
                && s.SaleDate >= from && s.SaleDate < to.AddDays(1))
            .ToListAsync();

        var wholesaleSales = await _context.WholesaleSales
            .Where(s => !s.IsDeleted && s.Status != SaleStatus.Cancelled
                && s.SaleDate >= from && s.SaleDate < to.AddDays(1))
            .ToListAsync();

        var retailByMonth = retailSales.GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count(), Total = g.Sum(s => s.TotalAmount) }).ToList();

        var wholesaleByMonth = wholesaleSales.GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count(), Total = g.Sum(s => s.TotalAmount) }).ToList();

        var allMonths = retailByMonth.Select(r => (r.Year, r.Month))
            .Union(wholesaleByMonth.Select(w => (w.Year, w.Month)))
            .Distinct().OrderBy(m => m.Year).ThenBy(m => m.Month).ToList();

        var result = new ReportResult
        {
            Title = "Monthly Sales Summary",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "month", Label = "Month", Format = ColumnFormat.Text },
                new() { Key = "retailCount", Label = "Retail Txns", Format = ColumnFormat.Number },
                new() { Key = "retailTotal", Label = "Retail Total (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "wholesaleCount", Label = "Wholesale Txns", Format = ColumnFormat.Number },
                new() { Key = "wholesaleTotal", Label = "Wholesale Total (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "combinedTotal", Label = "Combined Total (SSP)", Format = ColumnFormat.Currency }
            }
        };

        foreach (var (year, month) in allMonths)
        {
            var retail = retailByMonth.FirstOrDefault(r => r.Year == year && r.Month == month);
            var wholesale = wholesaleByMonth.FirstOrDefault(w => w.Year == year && w.Month == month);
            result.Rows.Add(new()
            {
                ["month"] = $"{new DateTime(year, month, 1):MMM yyyy}",
                ["retailCount"] = retail?.Count ?? 0,
                ["retailTotal"] = retail?.Total ?? 0m,
                ["wholesaleCount"] = wholesale?.Count ?? 0,
                ["wholesaleTotal"] = wholesale?.Total ?? 0m,
                ["combinedTotal"] = (retail?.Total ?? 0m) + (wholesale?.Total ?? 0m)
            });
        }

        result.Summary["Total Retail Sales"] = retailSales.Sum(s => s.TotalAmount);
        result.Summary["Total Wholesale Sales"] = wholesaleSales.Sum(s => s.TotalAmount);
        result.Summary["Grand Total"] = retailSales.Sum(s => s.TotalAmount) + wholesaleSales.Sum(s => s.TotalAmount);

        return result;
    }

    private async Task<ReportResult> GetSalesByProductReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var retailItems = await _context.RetailSaleItems
            .Include(i => i.Drug)
            .Include(i => i.Sale)
            .Where(i => !i.Sale.IsDeleted && i.Sale.Status == RetailSaleStatus.Completed
                && i.Sale.SaleDate >= from && i.Sale.SaleDate < to.AddDays(1))
            .ToListAsync();

        var wholesaleItems = await _context.WholesaleSaleItems
            .Include(i => i.Drug)
            .Include(i => i.Sale)
            .Where(i => !i.Sale.IsDeleted && i.Sale.Status != SaleStatus.Cancelled
                && i.Sale.SaleDate >= from && i.Sale.SaleDate < to.AddDays(1))
            .ToListAsync();

        var drugSales = new Dictionary<int, (string Code, string Name, int Qty, decimal Revenue, decimal Cost)>();

        foreach (var item in retailItems)
        {
            if (!drugSales.ContainsKey(item.DrugId))
                drugSales[item.DrugId] = (item.Drug.Code, item.Drug.Name, 0, 0m, 0m);
            var current = drugSales[item.DrugId];
            drugSales[item.DrugId] = (current.Code, current.Name,
                current.Qty + item.Quantity, current.Revenue + item.LineTotal, current.Cost + item.Quantity * item.Drug.CostPrice);
        }

        foreach (var item in wholesaleItems)
        {
            if (!drugSales.ContainsKey(item.DrugId))
                drugSales[item.DrugId] = (item.Drug.Code, item.Drug.Name, 0, 0m, 0m);
            var current = drugSales[item.DrugId];
            drugSales[item.DrugId] = (current.Code, current.Name,
                current.Qty + item.Quantity, current.Revenue + item.LineTotal, current.Cost + item.Quantity * item.Drug.CostPrice);
        }

        var result = new ReportResult
        {
            Title = "Sales by Product Report",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "code", Label = "Drug Code", Format = ColumnFormat.Text },
                new() { Key = "name", Label = "Drug Name", Format = ColumnFormat.Text },
                new() { Key = "qtySold", Label = "Qty Sold", Format = ColumnFormat.Number },
                new() { Key = "revenue", Label = "Revenue (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "cost", Label = "Cost (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "profit", Label = "Profit (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "margin", Label = "Margin %", Format = ColumnFormat.Percent }
            }
        };

        foreach (var (_, val) in drugSales.OrderByDescending(d => d.Value.Revenue))
        {
            var profit = val.Revenue - val.Cost;
            result.Rows.Add(new()
            {
                ["code"] = val.Code,
                ["name"] = val.Name,
                ["qtySold"] = val.Qty,
                ["revenue"] = val.Revenue,
                ["cost"] = val.Cost,
                ["profit"] = profit,
                ["margin"] = val.Revenue > 0 ? Math.Round(profit / val.Revenue * 100, 2) : 0m
            });
        }

        result.Summary["Total Products Sold"] = drugSales.Count;
        result.Summary["Total Revenue"] = drugSales.Values.Sum(v => v.Revenue);
        result.Summary["Total Cost"] = drugSales.Values.Sum(v => v.Cost);
        result.Summary["Total Profit"] = drugSales.Values.Sum(v => v.Revenue - v.Cost);

        return result;
    }

    private async Task<ReportResult> GetSalesByCustomerReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var sales = await _context.WholesaleSales
            .Include(s => s.Customer)
            .Where(s => !s.IsDeleted && s.Status != SaleStatus.Cancelled
                && s.SaleDate >= from && s.SaleDate < to.AddDays(1))
            .AsNoTracking()
            .ToListAsync();

        var byCustomer = sales.GroupBy(s => s.CustomerId)
            .Select(g => new
            {
                Customer = g.First().Customer,
                InvoiceCount = g.Count(),
                Total = g.Sum(s => s.TotalAmount),
                Paid = g.Sum(s => s.PaidAmount),
                Balance = g.Sum(s => s.BalanceAmount)
            }).OrderByDescending(c => c.Total).ToList();

        var result = new ReportResult
        {
            Title = "Sales by Customer Report",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "customer", Label = "Customer", Format = ColumnFormat.Text },
                new() { Key = "invoiceCount", Label = "Invoices", Format = ColumnFormat.Number },
                new() { Key = "total", Label = "Total (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "paid", Label = "Paid (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "balance", Label = "Balance (SSP)", Format = ColumnFormat.Currency }
            }
        };

        foreach (var c in byCustomer)
        {
            result.Rows.Add(new()
            {
                ["customer"] = c.Customer?.Name ?? "Unknown",
                ["invoiceCount"] = c.InvoiceCount,
                ["total"] = c.Total,
                ["paid"] = c.Paid,
                ["balance"] = c.Balance
            });
        }

        result.Summary["Total Customers"] = byCustomer.Count;
        result.Summary["Total Sales"] = byCustomer.Sum(c => c.Total);
        result.Summary["Total Outstanding"] = byCustomer.Sum(c => c.Balance);

        return result;
    }

    private async Task<ReportResult> GetCashierSummaryReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var sales = await _context.RetailSales
            .Where(s => !s.IsDeleted && s.Status == RetailSaleStatus.Completed
                && s.SaleDate >= from && s.SaleDate < to.AddDays(1))
            .AsNoTracking()
            .ToListAsync();

        var byCashier = sales.GroupBy(s => s.CashierId ?? "Unknown")
            .Select(g => new
            {
                CashierId = g.Key,
                Count = g.Count(),
                Total = g.Sum(s => s.TotalAmount),
                Avg = g.Average(s => s.TotalAmount)
            }).OrderByDescending(c => c.Total).ToList();

        var result = new ReportResult
        {
            Title = "Cashier Summary Report",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "cashier", Label = "Cashier", Format = ColumnFormat.Text },
                new() { Key = "transactions", Label = "Transactions", Format = ColumnFormat.Number },
                new() { Key = "totalSales", Label = "Total Sales (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "avgSale", Label = "Avg Sale (SSP)", Format = ColumnFormat.Currency }
            }
        };

        foreach (var c in byCashier)
        {
            result.Rows.Add(new()
            {
                ["cashier"] = c.CashierId,
                ["transactions"] = c.Count,
                ["totalSales"] = c.Total,
                ["avgSale"] = Math.Round(c.Avg, 2)
            });
        }

        result.Summary["Total Cashiers"] = byCashier.Count;
        result.Summary["Total Transactions"] = sales.Count;
        result.Summary["Total Sales"] = sales.Sum(s => s.TotalAmount);

        return result;
    }

    // ========== FINANCIAL REPORTS ==========

    private async Task<ReportResult> GetProfitLossReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var retailSales = await _context.RetailSales
            .Where(s => !s.IsDeleted && s.Status == RetailSaleStatus.Completed
                && s.SaleDate >= from && s.SaleDate < to.AddDays(1))
            .ToListAsync();

        var wholesaleSales = await _context.WholesaleSales
            .Where(s => !s.IsDeleted && s.Status != SaleStatus.Cancelled
                && s.SaleDate >= from && s.SaleDate < to.AddDays(1))
            .ToListAsync();

        var retailItems = await _context.RetailSaleItems
            .Include(i => i.Drug)
            .Include(i => i.Sale)
            .Where(i => !i.Sale.IsDeleted && i.Sale.Status == RetailSaleStatus.Completed
                && i.Sale.SaleDate >= from && i.Sale.SaleDate < to.AddDays(1))
            .ToListAsync();

        var wholesaleItems = await _context.WholesaleSaleItems
            .Include(i => i.Drug)
            .Include(i => i.Sale)
            .Where(i => !i.Sale.IsDeleted && i.Sale.Status != SaleStatus.Cancelled
                && i.Sale.SaleDate >= from && i.Sale.SaleDate < to.AddDays(1))
            .ToListAsync();

        var retailRevenue = retailSales.Sum(s => s.SubTotal);
        var wholesaleRevenue = wholesaleSales.Sum(s => s.SubTotal);
        var totalRevenue = retailRevenue + wholesaleRevenue;

        var retailCOGS = retailItems.Sum(i => i.Quantity * i.Drug.CostPrice);
        var wholesaleCOGS = wholesaleItems.Sum(i => i.Quantity * i.Drug.CostPrice);
        var totalCOGS = retailCOGS + wholesaleCOGS;

        var retailTax = retailSales.Sum(s => s.TaxAmount);
        var wholesaleTax = wholesaleSales.Sum(s => s.TaxAmount);

        var retailDiscount = retailSales.Sum(s => s.DiscountAmount);
        var wholesaleDiscount = wholesaleSales.Sum(s => s.DiscountAmount);

        var grossProfit = totalRevenue - totalCOGS;

        var result = new ReportResult
        {
            Title = "Profit & Loss Statement",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "item", Label = "Item", Format = ColumnFormat.Text },
                new() { Key = "retail", Label = "Retail (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "wholesale", Label = "Wholesale (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "total", Label = "Total (SSP)", Format = ColumnFormat.Currency }
            }
        };

        result.Rows.Add(new() { ["item"] = "Revenue (Sales)", ["retail"] = retailRevenue, ["wholesale"] = wholesaleRevenue, ["total"] = totalRevenue });
        result.Rows.Add(new() { ["item"] = "Cost of Goods Sold", ["retail"] = retailCOGS, ["wholesale"] = wholesaleCOGS, ["total"] = totalCOGS });
        result.Rows.Add(new() { ["item"] = "Gross Profit", ["retail"] = retailRevenue - retailCOGS, ["wholesale"] = wholesaleRevenue - wholesaleCOGS, ["total"] = grossProfit });
        result.Rows.Add(new() { ["item"] = "Tax Collected", ["retail"] = retailTax, ["wholesale"] = wholesaleTax, ["total"] = retailTax + wholesaleTax });
        result.Rows.Add(new() { ["item"] = "Discounts Given", ["retail"] = retailDiscount, ["wholesale"] = wholesaleDiscount, ["total"] = retailDiscount + wholesaleDiscount });

        result.Summary["Total Revenue"] = totalRevenue;
        result.Summary["Gross Profit"] = grossProfit;
        result.Summary["Gross Margin"] = totalRevenue > 0 ? Math.Round(grossProfit / totalRevenue * 100, 2) : 0m;

        return result;
    }

    private async Task<ReportResult> GetOutstandingPaymentsReport()
    {
        var sales = await _context.WholesaleSales
            .Include(s => s.Customer)
            .Where(s => !s.IsDeleted && s.BalanceAmount > 0 && s.Status != SaleStatus.Cancelled)
            .OrderByDescending(s => s.BalanceAmount)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Outstanding Payments Report",
            SubTitle = "All unpaid or partially paid wholesale invoices",
            Columns = new()
            {
                new() { Key = "invoice", Label = "Invoice #", Format = ColumnFormat.Text },
                new() { Key = "customer", Label = "Customer", Format = ColumnFormat.Text },
                new() { Key = "saleDate", Label = "Sale Date", Format = ColumnFormat.Date },
                new() { Key = "dueDate", Label = "Due Date", Format = ColumnFormat.Date },
                new() { Key = "total", Label = "Total (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "paid", Label = "Paid (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "balance", Label = "Balance (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "ageDays", Label = "Age (Days)", Format = ColumnFormat.Number }
            }
        };

        foreach (var s in sales)
        {
            result.Rows.Add(new()
            {
                ["invoice"] = s.InvoiceNumber,
                ["customer"] = s.Customer?.Name ?? "Unknown",
                ["saleDate"] = s.SaleDate,
                ["dueDate"] = s.DueDate,
                ["total"] = s.TotalAmount,
                ["paid"] = s.PaidAmount,
                ["balance"] = s.BalanceAmount,
                ["ageDays"] = (DateTime.UtcNow - s.SaleDate).Days
            });
        }

        result.Summary["Total Invoices"] = sales.Count;
        result.Summary["Total Outstanding"] = sales.Sum(s => s.BalanceAmount);

        return result;
    }

    private async Task<ReportResult> GetReceivablesAgingReport()
    {
        var sales = await _context.WholesaleSales
            .Include(s => s.Customer)
            .Where(s => !s.IsDeleted && s.BalanceAmount > 0 && s.Status != SaleStatus.Cancelled)
            .AsNoTracking()
            .ToListAsync();

        var byCustomer = sales.GroupBy(s => s.CustomerId).Select(g =>
        {
            var customer = g.First().Customer;
            var current = g.Where(s => (DateTime.UtcNow - s.SaleDate).Days <= 30).Sum(s => s.BalanceAmount);
            var days31_60 = g.Where(s => { var d = (DateTime.UtcNow - s.SaleDate).Days; return d > 30 && d <= 60; }).Sum(s => s.BalanceAmount);
            var days61_90 = g.Where(s => { var d = (DateTime.UtcNow - s.SaleDate).Days; return d > 60 && d <= 90; }).Sum(s => s.BalanceAmount);
            var over90 = g.Where(s => (DateTime.UtcNow - s.SaleDate).Days > 90).Sum(s => s.BalanceAmount);
            return new { Customer = customer?.Name ?? "Unknown", Current = current, Days31_60 = days31_60, Days61_90 = days61_90, Over90 = over90, Total = current + days31_60 + days61_90 + over90 };
        }).OrderByDescending(c => c.Total).ToList();

        var result = new ReportResult
        {
            Title = "Receivables Aging Report",
            Columns = new()
            {
                new() { Key = "customer", Label = "Customer", Format = ColumnFormat.Text },
                new() { Key = "current", Label = "Current (0-30)", Format = ColumnFormat.Currency },
                new() { Key = "days31_60", Label = "31-60 Days", Format = ColumnFormat.Currency },
                new() { Key = "days61_90", Label = "61-90 Days", Format = ColumnFormat.Currency },
                new() { Key = "over90", Label = "Over 90 Days", Format = ColumnFormat.Currency },
                new() { Key = "total", Label = "Total (SSP)", Format = ColumnFormat.Currency }
            }
        };

        foreach (var c in byCustomer)
        {
            result.Rows.Add(new()
            {
                ["customer"] = c.Customer,
                ["current"] = c.Current,
                ["days31_60"] = c.Days31_60,
                ["days61_90"] = c.Days61_90,
                ["over90"] = c.Over90,
                ["total"] = c.Total
            });
        }

        result.Summary["Total Receivables"] = byCustomer.Sum(c => c.Total);
        result.Summary["Current (0-30 days)"] = byCustomer.Sum(c => c.Current);
        result.Summary["Overdue (> 30 days)"] = byCustomer.Sum(c => c.Days31_60 + c.Days61_90 + c.Over90);

        return result;
    }

    private async Task<ReportResult> GetPaymentHistoryReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var payments = await _context.Payments
            .Include(p => p.WholesaleSale).ThenInclude(s => s!.Customer)
            .Include(p => p.RetailSale)
            .Where(p => p.PaymentDate >= from && p.PaymentDate < to.AddDays(1))
            .OrderByDescending(p => p.PaymentDate)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Payment History Report",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "date", Label = "Date", Format = ColumnFormat.DateTime },
                new() { Key = "reference", Label = "Reference", Format = ColumnFormat.Text },
                new() { Key = "method", Label = "Method", Format = ColumnFormat.Text },
                new() { Key = "amount", Label = "Amount (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "relatedSale", Label = "Related Sale", Format = ColumnFormat.Text },
                new() { Key = "type", Label = "Type", Format = ColumnFormat.Text }
            }
        };

        foreach (var p in payments)
        {
            var relatedSale = p.WholesaleSaleId.HasValue
                ? $"INV: {p.WholesaleSale?.InvoiceNumber}"
                : p.RetailSaleId.HasValue ? $"REC: {p.RetailSale?.ReceiptNumber}" : "N/A";
            var type = p.WholesaleSaleId.HasValue ? "Wholesale" : "Retail";

            result.Rows.Add(new()
            {
                ["date"] = p.PaymentDate,
                ["reference"] = p.ReferenceNumber,
                ["method"] = p.PaymentMethod.ToString(),
                ["amount"] = p.AmountBase,
                ["relatedSale"] = relatedSale,
                ["type"] = type
            });
        }

        result.Summary["Total Payments"] = payments.Count;
        result.Summary["Total Amount"] = payments.Sum(p => p.AmountBase);
        result.Summary["Cash Payments"] = payments.Where(p => p.PaymentMethod == PaymentMethod.Cash).Sum(p => p.AmountBase);
        result.Summary["MoMo Payments"] = payments.Where(p => p.PaymentMethod == PaymentMethod.MoMo).Sum(p => p.AmountBase);

        return result;
    }

    private async Task<ReportResult> GetTaxSummaryReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var retailSales = await _context.RetailSales
            .Where(s => !s.IsDeleted && s.Status == RetailSaleStatus.Completed
                && s.SaleDate >= from && s.SaleDate < to.AddDays(1))
            .ToListAsync();

        var wholesaleSales = await _context.WholesaleSales
            .Where(s => !s.IsDeleted && s.Status != SaleStatus.Cancelled
                && s.SaleDate >= from && s.SaleDate < to.AddDays(1))
            .ToListAsync();

        var allByMonth = retailSales.Select(s => new { s.SaleDate, s.SubTotal, s.TaxAmount, Type = "Retail" })
            .Concat(wholesaleSales.Select(s => new { s.SaleDate, s.SubTotal, s.TaxAmount, Type = "Wholesale" }))
            .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .ToList();

        var result = new ReportResult
        {
            Title = "Tax Summary Report",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "period", Label = "Period", Format = ColumnFormat.Text },
                new() { Key = "taxableAmount", Label = "Taxable Amount (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "taxCollected", Label = "Tax Collected (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "effectiveRate", Label = "Effective Rate %", Format = ColumnFormat.Percent }
            }
        };

        foreach (var g in allByMonth)
        {
            var taxable = g.Sum(s => s.SubTotal);
            var tax = g.Sum(s => s.TaxAmount);
            result.Rows.Add(new()
            {
                ["period"] = $"{new DateTime(g.Key.Year, g.Key.Month, 1):MMM yyyy}",
                ["taxableAmount"] = taxable,
                ["taxCollected"] = tax,
                ["effectiveRate"] = taxable > 0 ? Math.Round(tax / taxable * 100, 2) : 0m
            });
        }

        result.Summary["Total Taxable Amount"] = result.Rows.Sum(r => (decimal)(r["taxableAmount"] ?? 0m));
        result.Summary["Total Tax Collected"] = result.Rows.Sum(r => (decimal)(r["taxCollected"] ?? 0m));

        return result;
    }

    // ========== SUPPLIER REPORTS ==========

    private async Task<ReportResult> GetPurchaseSummaryReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-3);
        var to = endDate ?? DateTime.UtcNow;

        var receipts = await _context.GoodsReceipts
            .Include(gr => gr.Supplier)
            .Include(gr => gr.Items)
            .Where(gr => !gr.IsDeleted && gr.ReceivedDate >= from && gr.ReceivedDate < to.AddDays(1))
            .AsNoTracking()
            .ToListAsync();

        var bySupplier = receipts.GroupBy(gr => gr.SupplierId)
            .Select(g => new
            {
                Supplier = g.First().Supplier?.Name ?? "Unknown",
                GrnCount = g.Count(),
                TotalValue = g.Sum(gr => gr.TotalAmountBase),
                ItemsReceived = g.Sum(gr => gr.Items.Sum(i => i.AcceptedQuantity))
            }).OrderByDescending(s => s.TotalValue).ToList();

        var result = new ReportResult
        {
            Title = "Purchase Summary Report",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "supplier", Label = "Supplier", Format = ColumnFormat.Text },
                new() { Key = "grnCount", Label = "GRNs", Format = ColumnFormat.Number },
                new() { Key = "itemsReceived", Label = "Items Received", Format = ColumnFormat.Number },
                new() { Key = "totalValue", Label = "Total Value (SSP)", Format = ColumnFormat.Currency }
            }
        };

        foreach (var s in bySupplier)
        {
            result.Rows.Add(new()
            {
                ["supplier"] = s.Supplier,
                ["grnCount"] = s.GrnCount,
                ["itemsReceived"] = s.ItemsReceived,
                ["totalValue"] = s.TotalValue
            });
        }

        result.Summary["Total Suppliers"] = bySupplier.Count;
        result.Summary["Total GRNs"] = receipts.Count;
        result.Summary["Total Purchase Value"] = bySupplier.Sum(s => s.TotalValue);

        return result;
    }

    private async Task<ReportResult> GetSupplierPerformanceReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-6);
        var to = endDate ?? DateTime.UtcNow;

        var requests = await _context.DrugRequests
            .Include(dr => dr.Supplier)
            .Where(dr => !dr.IsDeleted && dr.RequestDate >= from && dr.RequestDate < to.AddDays(1))
            .AsNoTracking()
            .ToListAsync();

        var bySupplier = requests.GroupBy(dr => dr.SupplierId)
            .Select(g =>
            {
                var delivered = g.Where(dr => dr.ActualDeliveryDate.HasValue && dr.ExpectedDeliveryDate.HasValue).ToList();
                var onTime = delivered.Count(dr => dr.ActualDeliveryDate!.Value <= dr.ExpectedDeliveryDate!.Value);
                var avgLead = delivered.Any()
                    ? delivered.Average(dr => (dr.ActualDeliveryDate!.Value - dr.RequestDate).Days)
                    : 0.0;
                return new
                {
                    Supplier = g.First().Supplier?.Name ?? "Unknown",
                    Orders = g.Count(),
                    Delivered = delivered.Count,
                    OnTimePercent = delivered.Count > 0 ? Math.Round((double)onTime / delivered.Count * 100, 1) : 0,
                    AvgLeadDays = Math.Round(avgLead, 1),
                    TotalValue = g.Sum(dr => dr.TotalAmountBase)
                };
            }).OrderByDescending(s => s.TotalValue).ToList();

        var result = new ReportResult
        {
            Title = "Supplier Performance Report",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "supplier", Label = "Supplier", Format = ColumnFormat.Text },
                new() { Key = "orders", Label = "Orders", Format = ColumnFormat.Number },
                new() { Key = "delivered", Label = "Delivered", Format = ColumnFormat.Number },
                new() { Key = "onTimePercent", Label = "On-Time %", Format = ColumnFormat.Percent },
                new() { Key = "avgLeadDays", Label = "Avg Lead (Days)", Format = ColumnFormat.Number },
                new() { Key = "totalValue", Label = "Total Value (SSP)", Format = ColumnFormat.Currency }
            }
        };

        foreach (var s in bySupplier)
        {
            result.Rows.Add(new()
            {
                ["supplier"] = s.Supplier,
                ["orders"] = s.Orders,
                ["delivered"] = s.Delivered,
                ["onTimePercent"] = s.OnTimePercent,
                ["avgLeadDays"] = s.AvgLeadDays,
                ["totalValue"] = s.TotalValue
            });
        }

        result.Summary["Total Suppliers"] = bySupplier.Count;
        result.Summary["Total Orders"] = requests.Count;

        return result;
    }

    private async Task<ReportResult> GetGoodsReceivedReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var receipts = await _context.GoodsReceipts
            .Include(gr => gr.Supplier)
            .Include(gr => gr.Items)
            .Where(gr => !gr.IsDeleted && gr.ReceivedDate >= from && gr.ReceivedDate < to.AddDays(1))
            .OrderByDescending(gr => gr.ReceivedDate)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Goods Received Notes Report",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "grnNumber", Label = "GRN #", Format = ColumnFormat.Text },
                new() { Key = "supplier", Label = "Supplier", Format = ColumnFormat.Text },
                new() { Key = "date", Label = "Received Date", Format = ColumnFormat.Date },
                new() { Key = "items", Label = "Line Items", Format = ColumnFormat.Number },
                new() { Key = "totalValue", Label = "Total Value (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "status", Label = "Status", Format = ColumnFormat.Text }
            }
        };

        foreach (var gr in receipts)
        {
            result.Rows.Add(new()
            {
                ["grnNumber"] = gr.GRNNumber,
                ["supplier"] = gr.Supplier?.Name ?? "Unknown",
                ["date"] = gr.ReceivedDate,
                ["items"] = gr.Items.Count,
                ["totalValue"] = gr.TotalAmountBase,
                ["status"] = gr.Status.ToString()
            });
        }

        result.Summary["Total GRNs"] = receipts.Count;
        result.Summary["Total Value"] = receipts.Sum(gr => gr.TotalAmountBase);

        return result;
    }

    private async Task<ReportResult> GetPendingOrdersReport()
    {
        var requests = await _context.DrugRequests
            .Include(dr => dr.Supplier)
            .Where(dr => !dr.IsDeleted
                && dr.Status != DrugRequestStatus.Completed
                && dr.Status != DrugRequestStatus.Cancelled)
            .OrderBy(dr => dr.ExpectedDeliveryDate)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Pending Orders Report",
            SubTitle = "All open purchase orders",
            Columns = new()
            {
                new() { Key = "requestNumber", Label = "Request #", Format = ColumnFormat.Text },
                new() { Key = "supplier", Label = "Supplier", Format = ColumnFormat.Text },
                new() { Key = "requestDate", Label = "Request Date", Format = ColumnFormat.Date },
                new() { Key = "status", Label = "Status", Format = ColumnFormat.Text },
                new() { Key = "expectedDelivery", Label = "Expected Delivery", Format = ColumnFormat.Date },
                new() { Key = "totalValue", Label = "Value (SSP)", Format = ColumnFormat.Currency }
            }
        };

        foreach (var dr in requests)
        {
            result.Rows.Add(new()
            {
                ["requestNumber"] = dr.RequestNumber,
                ["supplier"] = dr.Supplier?.Name ?? "Unknown",
                ["requestDate"] = dr.RequestDate,
                ["status"] = dr.Status.ToString(),
                ["expectedDelivery"] = dr.ExpectedDeliveryDate,
                ["totalValue"] = dr.TotalAmountBase
            });
        }

        result.Summary["Total Pending Orders"] = requests.Count;
        result.Summary["Total Value"] = requests.Sum(dr => dr.TotalAmountBase);

        return result;
    }

    // ========== COMPLIANCE REPORTS ==========

    private async Task<ReportResult> GetControlledDrugsReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var movements = await _context.StockMovements
            .Include(sm => sm.Drug)
            .Include(sm => sm.Batch)
            .Where(sm => sm.Drug.IsControlled && sm.CreatedAt >= from && sm.CreatedAt < to.AddDays(1))
            .OrderByDescending(sm => sm.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Controlled Drugs Log",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "date", Label = "Date", Format = ColumnFormat.DateTime },
                new() { Key = "drug", Label = "Drug", Format = ColumnFormat.Text },
                new() { Key = "batch", Label = "Batch", Format = ColumnFormat.Text },
                new() { Key = "type", Label = "Movement Type", Format = ColumnFormat.Text },
                new() { Key = "quantity", Label = "Quantity", Format = ColumnFormat.Number },
                new() { Key = "balanceAfter", Label = "Balance After", Format = ColumnFormat.Number },
                new() { Key = "reference", Label = "Reference", Format = ColumnFormat.Text }
            }
        };

        foreach (var sm in movements)
        {
            result.Rows.Add(new()
            {
                ["date"] = sm.CreatedAt,
                ["drug"] = sm.Drug?.Name ?? "N/A",
                ["batch"] = sm.Batch?.BatchNumber ?? "N/A",
                ["type"] = sm.Type.ToString(),
                ["quantity"] = sm.Quantity,
                ["balanceAfter"] = sm.BalanceAfter,
                ["reference"] = sm.Reference ?? ""
            });
        }

        result.Summary["Total Movements"] = movements.Count;
        result.Summary["Controlled Drugs Tracked"] = movements.Select(m => m.DrugId).Distinct().Count();

        return result;
    }

    private async Task<ReportResult> GetPrescriptionLogReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var sales = await _context.RetailSales
            .Include(s => s.Items).ThenInclude(i => i.Drug)
            .Where(s => !s.IsDeleted && s.HasPrescriptionItems
                && s.SaleDate >= from && s.SaleDate < to.AddDays(1))
            .OrderByDescending(s => s.SaleDate)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Prescription Log",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "receipt", Label = "Receipt #", Format = ColumnFormat.Text },
                new() { Key = "date", Label = "Date", Format = ColumnFormat.DateTime },
                new() { Key = "prescriptionNumber", Label = "Prescription #", Format = ColumnFormat.Text },
                new() { Key = "customer", Label = "Customer", Format = ColumnFormat.Text },
                new() { Key = "items", Label = "Rx Items", Format = ColumnFormat.Number },
                new() { Key = "total", Label = "Total (SSP)", Format = ColumnFormat.Currency }
            }
        };

        foreach (var s in sales)
        {
            result.Rows.Add(new()
            {
                ["receipt"] = s.ReceiptNumber,
                ["date"] = s.SaleDate,
                ["prescriptionNumber"] = s.PrescriptionNumber ?? "N/A",
                ["customer"] = s.CustomerName ?? "Walk-in",
                ["items"] = s.Items.Count(i => i.IsPrescriptionItem),
                ["total"] = s.TotalAmount
            });
        }

        result.Summary["Total Prescriptions"] = sales.Count;
        result.Summary["Total Value"] = sales.Sum(s => s.TotalAmount);

        return result;
    }

    private async Task<ReportResult> GetBatchTraceabilityReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-3);
        var to = endDate ?? DateTime.UtcNow;

        var batches = await _context.Batches
            .Include(b => b.Drug)
            .Where(b => !b.IsDeleted && b.CreatedAt >= from && b.CreatedAt < to.AddDays(1))
            .OrderByDescending(b => b.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Batch Traceability Report",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "batchNumber", Label = "Batch #", Format = ColumnFormat.Text },
                new() { Key = "drug", Label = "Drug", Format = ColumnFormat.Text },
                new() { Key = "received", Label = "Received Date", Format = ColumnFormat.Date },
                new() { Key = "expiry", Label = "Expiry Date", Format = ColumnFormat.Date },
                new() { Key = "initialQty", Label = "Initial Qty", Format = ColumnFormat.Number },
                new() { Key = "currentQty", Label = "Current Qty", Format = ColumnFormat.Number },
                new() { Key = "status", Label = "Status", Format = ColumnFormat.Text }
            }
        };

        foreach (var b in batches)
        {
            result.Rows.Add(new()
            {
                ["batchNumber"] = b.BatchNumber,
                ["drug"] = b.Drug?.Name ?? "N/A",
                ["received"] = b.CreatedAt,
                ["expiry"] = b.ExpiryDate,
                ["initialQty"] = b.InitialQuantity,
                ["currentQty"] = b.CurrentQuantity,
                ["status"] = b.Status.ToString()
            });
        }

        result.Summary["Total Batches"] = batches.Count;
        result.Summary["Active Batches"] = batches.Count(b => b.Status == BatchStatus.Active);

        return result;
    }

    private async Task<ReportResult> GetDisposalLogReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-3);
        var to = endDate ?? DateTime.UtcNow;

        var movements = await _context.StockMovements
            .Include(sm => sm.Drug)
            .Include(sm => sm.Batch)
            .Where(sm => sm.Type == MovementType.WriteOff
                && sm.CreatedAt >= from && sm.CreatedAt < to.AddDays(1))
            .OrderByDescending(sm => sm.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Disposal/Write-off Log",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "date", Label = "Date", Format = ColumnFormat.DateTime },
                new() { Key = "drug", Label = "Drug", Format = ColumnFormat.Text },
                new() { Key = "batch", Label = "Batch", Format = ColumnFormat.Text },
                new() { Key = "quantity", Label = "Quantity", Format = ColumnFormat.Number },
                new() { Key = "value", Label = "Value (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "reason", Label = "Reason", Format = ColumnFormat.Text },
                new() { Key = "reference", Label = "Reference", Format = ColumnFormat.Text }
            }
        };

        foreach (var sm in movements)
        {
            result.Rows.Add(new()
            {
                ["date"] = sm.CreatedAt,
                ["drug"] = sm.Drug?.Name ?? "N/A",
                ["batch"] = sm.Batch?.BatchNumber ?? "N/A",
                ["quantity"] = Math.Abs(sm.Quantity),
                ["value"] = Math.Abs(sm.Quantity) * (sm.Batch?.CostPrice ?? 0m),
                ["reason"] = sm.Reason ?? "N/A",
                ["reference"] = sm.Reference ?? ""
            });
        }

        result.Summary["Total Write-offs"] = movements.Count;
        result.Summary["Total Value Lost"] = result.Rows.Sum(r => (decimal)(r["value"] ?? 0m));

        return result;
    }

    // ========== AUDIT REPORTS ==========

    private async Task<ReportResult> GetUserActivityReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddDays(-7);
        var to = endDate ?? DateTime.UtcNow;

        var logs = await _context.AuditLogs
            .Where(l => l.Timestamp >= from && l.Timestamp < to.AddDays(1))
            .OrderByDescending(l => l.Timestamp)
            .Take(5000)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "User Activity Log",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "timestamp", Label = "Timestamp", Format = ColumnFormat.DateTime },
                new() { Key = "user", Label = "User", Format = ColumnFormat.Text },
                new() { Key = "action", Label = "Action", Format = ColumnFormat.Text },
                new() { Key = "entity", Label = "Entity", Format = ColumnFormat.Text },
                new() { Key = "entityId", Label = "Entity ID", Format = ColumnFormat.Text },
                new() { Key = "ipAddress", Label = "IP Address", Format = ColumnFormat.Text }
            }
        };

        foreach (var l in logs)
        {
            result.Rows.Add(new()
            {
                ["timestamp"] = l.Timestamp,
                ["user"] = l.UserName,
                ["action"] = l.Action,
                ["entity"] = l.EntityType,
                ["entityId"] = l.EntityId ?? "",
                ["ipAddress"] = l.IpAddress ?? ""
            });
        }

        result.Summary["Total Activities"] = logs.Count;
        result.Summary["Unique Users"] = logs.Select(l => l.UserId).Distinct().Count();

        return result;
    }

    private async Task<ReportResult> GetLoginHistoryReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddDays(-30);
        var to = endDate ?? DateTime.UtcNow;

        var logs = await _context.AuditLogs
            .Where(l => (l.Action == "Login" || l.Action == "Logout")
                && l.Timestamp >= from && l.Timestamp < to.AddDays(1))
            .OrderByDescending(l => l.Timestamp)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Login History Report",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "timestamp", Label = "Timestamp", Format = ColumnFormat.DateTime },
                new() { Key = "user", Label = "User", Format = ColumnFormat.Text },
                new() { Key = "action", Label = "Action", Format = ColumnFormat.Text },
                new() { Key = "ipAddress", Label = "IP Address", Format = ColumnFormat.Text }
            }
        };

        foreach (var l in logs)
        {
            result.Rows.Add(new()
            {
                ["timestamp"] = l.Timestamp,
                ["user"] = l.UserName,
                ["action"] = l.Action,
                ["ipAddress"] = l.IpAddress ?? ""
            });
        }

        result.Summary["Total Logins"] = logs.Count(l => l.Action == "Login");
        result.Summary["Unique Users"] = logs.Select(l => l.UserId).Distinct().Count();

        return result;
    }

    private async Task<ReportResult> GetDataChangesReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddDays(-7);
        var to = endDate ?? DateTime.UtcNow;

        var logs = await _context.AuditLogs
            .Where(l => (l.Action == "Create" || l.Action == "Update" || l.Action == "Delete")
                && l.Timestamp >= from && l.Timestamp < to.AddDays(1))
            .OrderByDescending(l => l.Timestamp)
            .Take(5000)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Data Change Log",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "timestamp", Label = "Timestamp", Format = ColumnFormat.DateTime },
                new() { Key = "user", Label = "User", Format = ColumnFormat.Text },
                new() { Key = "action", Label = "Action", Format = ColumnFormat.Text },
                new() { Key = "entity", Label = "Entity", Format = ColumnFormat.Text },
                new() { Key = "entityId", Label = "Entity ID", Format = ColumnFormat.Text }
            }
        };

        foreach (var l in logs)
        {
            result.Rows.Add(new()
            {
                ["timestamp"] = l.Timestamp,
                ["user"] = l.UserName,
                ["action"] = l.Action,
                ["entity"] = l.EntityType,
                ["entityId"] = l.EntityId ?? ""
            });
        }

        result.Summary["Total Changes"] = logs.Count;
        result.Summary["Creates"] = logs.Count(l => l.Action == "Create");
        result.Summary["Updates"] = logs.Count(l => l.Action == "Update");
        result.Summary["Deletes"] = logs.Count(l => l.Action == "Delete");

        return result;
    }

    private async Task<ReportResult> GetSystemLogReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddDays(-7);
        var to = endDate ?? DateTime.UtcNow;

        var logs = await _context.AuditLogs
            .Where(l => l.Timestamp >= from && l.Timestamp < to.AddDays(1))
            .OrderByDescending(l => l.Timestamp)
            .Take(5000)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "System Events Log",
            StartDate = startDate, EndDate = endDate,
            Columns = new()
            {
                new() { Key = "timestamp", Label = "Timestamp", Format = ColumnFormat.DateTime },
                new() { Key = "user", Label = "User", Format = ColumnFormat.Text },
                new() { Key = "action", Label = "Action", Format = ColumnFormat.Text },
                new() { Key = "entity", Label = "Entity", Format = ColumnFormat.Text },
                new() { Key = "entityId", Label = "Entity ID", Format = ColumnFormat.Text },
                new() { Key = "ipAddress", Label = "IP Address", Format = ColumnFormat.Text }
            }
        };

        foreach (var l in logs)
        {
            result.Rows.Add(new()
            {
                ["timestamp"] = l.Timestamp,
                ["user"] = l.UserName,
                ["action"] = l.Action,
                ["entity"] = l.EntityType,
                ["entityId"] = l.EntityId ?? "",
                ["ipAddress"] = l.IpAddress ?? ""
            });
        }

        result.Summary["Total Events"] = logs.Count;
        result.Summary["Unique Users"] = logs.Select(l => l.UserId).Distinct().Count();

        return result;
    }

    // ========== EXPENSES REPORTS ==========

    private async Task<ReportResult> GetExpensesByCategoryReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var expenses = await _context.Expenses
            .Include(e => e.Category)
            .Where(e => !e.IsDeleted && e.Status != ExpenseStatus.Voided
                && e.Date >= from && e.Date < to.AddDays(1))
            .AsNoTracking()
            .ToListAsync();

        var byCategory = expenses.GroupBy(e => e.CategoryId)
            .Select(g => new
            {
                Category = g.First().Category?.Name ?? "Unknown",
                Count = g.Count(),
                Total = g.Sum(e => e.AmountBase)
            }).OrderByDescending(c => c.Total).ToList();

        var result = new ReportResult
        {
            Title = "Expenses by Category",
            StartDate = startDate,
            EndDate = endDate,
            Columns = new()
            {
                new() { Key = "category", Label = "Category", Format = ColumnFormat.Text },
                new() { Key = "count", Label = "# Expenses", Format = ColumnFormat.Number },
                new() { Key = "total", Label = "Total (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "percentage", Label = "% of Total", Format = ColumnFormat.Percent }
            }
        };

        var grandTotal = byCategory.Sum(c => c.Total);

        foreach (var c in byCategory)
        {
            result.Rows.Add(new()
            {
                ["category"] = c.Category,
                ["count"] = c.Count,
                ["total"] = c.Total,
                ["percentage"] = grandTotal > 0 ? Math.Round(c.Total / grandTotal * 100, 2) : 0m
            });
        }

        result.Summary["Total Categories"] = byCategory.Count;
        result.Summary["Total Expenses"] = expenses.Count;
        result.Summary["Total Amount"] = grandTotal;

        return result;
    }

    private async Task<ReportResult> GetExpensesByPaymentMethodReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var expenses = await _context.Expenses
            .Where(e => !e.IsDeleted && e.Status != ExpenseStatus.Voided
                && e.Date >= from && e.Date < to.AddDays(1))
            .AsNoTracking()
            .ToListAsync();

        var byMethod = expenses.GroupBy(e => e.PaymentMethod)
            .Select(g => new
            {
                Method = g.Key.ToString(),
                Count = g.Count(),
                Total = g.Sum(e => e.AmountBase)
            }).OrderByDescending(m => m.Total).ToList();

        var result = new ReportResult
        {
            Title = "Expenses by Payment Method",
            StartDate = startDate,
            EndDate = endDate,
            Columns = new()
            {
                new() { Key = "method", Label = "Payment Method", Format = ColumnFormat.Text },
                new() { Key = "count", Label = "# Transactions", Format = ColumnFormat.Number },
                new() { Key = "total", Label = "Total (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "percentage", Label = "% of Total", Format = ColumnFormat.Percent }
            }
        };

        var grandTotal = byMethod.Sum(m => m.Total);

        foreach (var m in byMethod)
        {
            result.Rows.Add(new()
            {
                ["method"] = m.Method,
                ["count"] = m.Count,
                ["total"] = m.Total,
                ["percentage"] = grandTotal > 0 ? Math.Round(m.Total / grandTotal * 100, 2) : 0m
            });
        }

        result.Summary["Total Expenses"] = expenses.Count;
        result.Summary["Total Amount"] = grandTotal;

        return result;
    }

    private async Task<ReportResult> GetExpensesMonthlySummaryReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-12);
        var to = endDate ?? DateTime.UtcNow;

        var expenses = await _context.Expenses
            .Include(e => e.Category)
            .Where(e => !e.IsDeleted && e.Status != ExpenseStatus.Voided
                && e.Date >= from && e.Date < to.AddDays(1))
            .AsNoTracking()
            .ToListAsync();

        var byMonth = expenses.GroupBy(e => new { e.Date.Year, e.Date.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Count = g.Count(),
                Total = g.Sum(e => e.AmountBase),
                Avg = g.Average(e => e.AmountBase)
            }).OrderBy(m => m.Year).ThenBy(m => m.Month).ToList();

        var result = new ReportResult
        {
            Title = "Monthly Expenses Summary",
            StartDate = startDate,
            EndDate = endDate,
            Columns = new()
            {
                new() { Key = "month", Label = "Month", Format = ColumnFormat.Text },
                new() { Key = "count", Label = "# Expenses", Format = ColumnFormat.Number },
                new() { Key = "total", Label = "Total (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "average", Label = "Avg Per Expense (SSP)", Format = ColumnFormat.Currency }
            }
        };

        foreach (var m in byMonth)
        {
            result.Rows.Add(new()
            {
                ["month"] = $"{new DateTime(m.Year, m.Month, 1):MMM yyyy}",
                ["count"] = m.Count,
                ["total"] = m.Total,
                ["average"] = Math.Round(m.Avg, 2)
            });
        }

        result.Summary["Total Expenses"] = expenses.Count;
        result.Summary["Total Amount"] = expenses.Sum(e => e.AmountBase);
        result.Summary["Average Per Month"] = byMonth.Any() ? Math.Round(byMonth.Average(m => m.Total), 2) : 0m;

        return result;
    }

    private async Task<ReportResult> GetExpensesByVendorReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = endDate ?? DateTime.UtcNow;

        var expenses = await _context.Expenses
            .Include(e => e.Category)
            .Where(e => !e.IsDeleted && e.Status != ExpenseStatus.Voided
                && e.Date >= from && e.Date < to.AddDays(1)
                && !string.IsNullOrEmpty(e.VendorName))
            .AsNoTracking()
            .ToListAsync();

        var byVendor = expenses.GroupBy(e => e.VendorName ?? "Unknown")
            .Select(g => new
            {
                Vendor = g.Key,
                Count = g.Count(),
                Total = g.Sum(e => e.AmountBase),
                LastExpense = g.Max(e => e.Date)
            }).OrderByDescending(v => v.Total).ToList();

        var result = new ReportResult
        {
            Title = "Expenses by Vendor/Payee",
            StartDate = startDate,
            EndDate = endDate,
            Columns = new()
            {
                new() { Key = "vendor", Label = "Vendor/Payee", Format = ColumnFormat.Text },
                new() { Key = "count", Label = "# Expenses", Format = ColumnFormat.Number },
                new() { Key = "total", Label = "Total (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "lastExpense", Label = "Last Expense Date", Format = ColumnFormat.Date }
            }
        };

        foreach (var v in byVendor)
        {
            result.Rows.Add(new()
            {
                ["vendor"] = v.Vendor,
                ["count"] = v.Count,
                ["total"] = v.Total,
                ["lastExpense"] = v.LastExpense
            });
        }

        result.Summary["Total Vendors"] = byVendor.Count;
        result.Summary["Total Expenses"] = expenses.Count;
        result.Summary["Total Amount"] = byVendor.Sum(v => v.Total);

        return result;
    }

    private async Task<ReportResult> GetRecurringExpensesReport()
    {
        var templates = await _context.Expenses
            .Include(e => e.Category)
            .Where(e => !e.IsDeleted && e.IsRecurring && e.RecurrenceTemplateId == null)
            .OrderBy(e => e.Category.Name)
            .ThenBy(e => e.Description)
            .AsNoTracking()
            .ToListAsync();

        var result = new ReportResult
        {
            Title = "Recurring Expenses",
            SubTitle = "Active recurring expense templates",
            Columns = new()
            {
                new() { Key = "reference", Label = "Reference", Format = ColumnFormat.Text },
                new() { Key = "description", Label = "Description", Format = ColumnFormat.Text },
                new() { Key = "category", Label = "Category", Format = ColumnFormat.Text },
                new() { Key = "amount", Label = "Amount (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "frequency", Label = "Frequency", Format = ColumnFormat.Text },
                new() { Key = "status", Label = "Status", Format = ColumnFormat.Text }
            }
        };

        foreach (var e in templates)
        {
            var frequency = e.RecurrenceFrequency != RecurrenceFrequency.None
                ? $"Every {e.RecurrenceInterval} {e.RecurrenceFrequency.ToString().ToLower()}(s)"
                : "Custom";

            result.Rows.Add(new()
            {
                ["reference"] = e.ReferenceNumber,
                ["description"] = e.Description,
                ["category"] = e.Category?.Name ?? "Unknown",
                ["amount"] = e.AmountBase,
                ["frequency"] = frequency,
                ["status"] = e.Status.ToString()
            });
        }

        result.Summary["Total Recurring Expenses"] = templates.Count;
        result.Summary["Total Monthly Estimate"] = templates
            .Where(e => e.RecurrenceFrequency == RecurrenceFrequency.Monthly)
            .Sum(e => e.AmountBase);

        return result;
    }

    private async Task<ReportResult> GetBudgetVsActualReport(DateTime? startDate, DateTime? endDate)
    {
        var from = startDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var to = endDate ?? DateTime.UtcNow;

        var categories = await _context.ExpenseCategories
            .Where(c => !c.IsDeleted && c.IsActive)
            .AsNoTracking()
            .ToListAsync();

        var expenses = await _context.Expenses
            .Where(e => !e.IsDeleted && e.Status != ExpenseStatus.Voided
                && e.Date >= from && e.Date < to.AddDays(1))
            .AsNoTracking()
            .ToListAsync();

        var expensesByCategory = expenses.GroupBy(e => e.CategoryId)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.AmountBase));

        var result = new ReportResult
        {
            Title = "Budget vs Actual Expenses",
            StartDate = startDate,
            EndDate = endDate,
            Columns = new()
            {
                new() { Key = "category", Label = "Category", Format = ColumnFormat.Text },
                new() { Key = "budget", Label = "Budget (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "actual", Label = "Actual (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "variance", Label = "Variance (SSP)", Format = ColumnFormat.Currency },
                new() { Key = "percentUsed", Label = "% Used", Format = ColumnFormat.Percent },
                new() { Key = "status", Label = "Status", Format = ColumnFormat.Text }
            }
        };

        foreach (var category in categories.OrderBy(c => c.Name))
        {
            var budget = category.BudgetLimit ?? 0m;
            var actual = expensesByCategory.GetValueOrDefault(category.Id, 0m);
            var variance = budget - actual;
            var percentUsed = budget > 0 ? Math.Round(actual / budget * 100, 2) : 0m;
            var status = budget == 0 ? "No Budget"
                : actual > budget ? "Over Budget"
                : percentUsed >= 90 ? "Warning"
                : "OK";

            result.Rows.Add(new()
            {
                ["category"] = category.Name,
                ["budget"] = budget,
                ["actual"] = actual,
                ["variance"] = variance,
                ["percentUsed"] = percentUsed,
                ["status"] = status
            });
        }

        result.Summary["Total Budget"] = categories.Sum(c => c.BudgetLimit ?? 0m);
        result.Summary["Total Actual"] = expenses.Sum(e => e.AmountBase);
        result.Summary["Categories Over Budget"] = result.Rows.Count(r => (string)(r["status"] ?? "") == "Over Budget");

        return result;
    }
}
