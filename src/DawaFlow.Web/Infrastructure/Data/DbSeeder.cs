using DawaFlow.Web.Data;
using DawaFlow.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DawaFlow.Web.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Check if data already exists
        if (await context.Drugs.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Seed Categories
        var categories = new List<DrugCategory>
        {
            new() { Name = "Analgesics", Description = "Pain relief medications" },
            new() { Name = "Antibiotics", Description = "Antibacterial medications" },
            new() { Name = "Cardiovascular", Description = "Heart and blood pressure medications" },
            new() { Name = "Antidiabetics", Description = "Diabetes management medications" },
            new() { Name = "Antihistamines", Description = "Allergy relief medications" }
        };

        await context.DrugCategories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // Seed Drugs
        var drugs = new List<Drug>
        {
            new()
            {
                Code = "PARA500",
                Barcode = "123456789001",
                Name = "Paracetamol 500mg",
                GenericName = "Paracetamol",
                Description = "Analgesic and antipyretic",
                CategoryId = categories[0].Id,
                Manufacturer = "PharmaCo",
                DosageForm = "Tablet",
                Strength = "500mg",
                PackSize = "Pack of 10",
                RetailPrice = 5.00m,
                WholesalePrice = 3.50m,
                CostPrice = 3.00m,
                ReorderLevel = 500,
                MaxStockLevel = 5000,
                TaxRate = 16,
                RequiresPrescription = false,
                IsControlled = false,
                IsActive = true
            },
            new()
            {
                Code = "AMOX250",
                Barcode = "123456789002",
                Name = "Amoxicillin 250mg",
                GenericName = "Amoxicillin",
                Description = "Broad-spectrum antibiotic",
                CategoryId = categories[1].Id,
                Manufacturer = "MediCorp",
                DosageForm = "Capsule",
                Strength = "250mg",
                PackSize = "Pack of 21",
                RetailPrice = 8.00m,
                WholesalePrice = 6.00m,
                CostPrice = 5.00m,
                ReorderLevel = 1000,
                MaxStockLevel = 3000,
                TaxRate = 16,
                RequiresPrescription = true,
                IsControlled = false,
                IsActive = true
            },
            new()
            {
                Code = "IBU400",
                Barcode = "123456789003",
                Name = "Ibuprofen 400mg",
                GenericName = "Ibuprofen",
                Description = "NSAID for pain and inflammation",
                CategoryId = categories[0].Id,
                Manufacturer = "PharmaCo",
                DosageForm = "Tablet",
                Strength = "400mg",
                PackSize = "Pack of 10",
                RetailPrice = 4.50m,
                WholesalePrice = 3.00m,
                CostPrice = 2.50m,
                ReorderLevel = 500,
                MaxStockLevel = 4000,
                TaxRate = 16,
                RequiresPrescription = false,
                IsControlled = false,
                IsActive = true
            },
            new()
            {
                Code = "ASP100",
                Barcode = "123456789004",
                Name = "Aspirin 100mg",
                GenericName = "Acetylsalicylic Acid",
                Description = "Antiplatelet and analgesic",
                CategoryId = categories[2].Id,
                Manufacturer = "CardioMed",
                DosageForm = "Tablet",
                Strength = "100mg",
                PackSize = "Pack of 30",
                RetailPrice = 3.00m,
                WholesalePrice = 2.00m,
                CostPrice = 1.50m,
                ReorderLevel = 300,
                MaxStockLevel = 2000,
                TaxRate = 16,
                RequiresPrescription = false,
                IsControlled = false,
                IsActive = true
            },
            new()
            {
                Code = "CIPRO500",
                Barcode = "123456789005",
                Name = "Ciprofloxacin 500mg",
                GenericName = "Ciprofloxacin",
                Description = "Fluoroquinolone antibiotic",
                CategoryId = categories[1].Id,
                Manufacturer = "MediCorp",
                DosageForm = "Tablet",
                Strength = "500mg",
                PackSize = "Pack of 10",
                RetailPrice = 15.00m,
                WholesalePrice = 12.00m,
                CostPrice = 10.00m,
                ReorderLevel = 200,
                MaxStockLevel = 1500,
                TaxRate = 16,
                RequiresPrescription = true,
                IsControlled = false,
                IsActive = true
            },
            new()
            {
                Code = "METF500",
                Barcode = "123456789006",
                Name = "Metformin 500mg",
                GenericName = "Metformin",
                Description = "Antidiabetic medication",
                CategoryId = categories[3].Id,
                Manufacturer = "DiabeCare",
                DosageForm = "Tablet",
                Strength = "500mg",
                PackSize = "Pack of 30",
                RetailPrice = 6.00m,
                WholesalePrice = 4.50m,
                CostPrice = 3.50m,
                ReorderLevel = 400,
                MaxStockLevel = 3000,
                TaxRate = 16,
                RequiresPrescription = true,
                IsControlled = false,
                IsActive = true
            }
        };

        await context.Drugs.AddRangeAsync(drugs);
        await context.SaveChangesAsync();

        // Seed Batches
        var batches = new List<Batch>
        {
            // Paracetamol batches
            new()
            {
                DrugId = drugs[0].Id,
                BatchNumber = "BTH-2024-001",
                ManufactureDate = new DateTime(2024, 1, 15),
                ExpiryDate = new DateTime(2026, 1, 15),
                InitialQuantity = 5000,
                CurrentQuantity = 3200,
                ReservedQuantity = 150,
                CostPrice = 3.00m,
                Status = BatchStatus.Active
            },
            new()
            {
                DrugId = drugs[0].Id,
                BatchNumber = "BTH-2024-015",
                ManufactureDate = new DateTime(2024, 3, 10),
                ExpiryDate = new DateTime(2026, 3, 10),
                InitialQuantity = 3000,
                CurrentQuantity = 2800,
                ReservedQuantity = 0,
                CostPrice = 3.00m,
                Status = BatchStatus.Active
            },
            // Amoxicillin batches
            new()
            {
                DrugId = drugs[1].Id,
                BatchNumber = "BTH-2024-002",
                ManufactureDate = new DateTime(2024, 2, 10),
                ExpiryDate = new DateTime(2026, 3, 20),
                InitialQuantity = 3000,
                CurrentQuantity = 1800,
                ReservedQuantity = 0,
                CostPrice = 5.00m,
                Status = BatchStatus.Active
            },
            // Ibuprofen batches
            new()
            {
                DrugId = drugs[2].Id,
                BatchNumber = "BTH-2023-045",
                ManufactureDate = new DateTime(2023, 5, 20),
                ExpiryDate = new DateTime(2025, 5, 20),
                InitialQuantity = 4000,
                CurrentQuantity = 450,
                ReservedQuantity = 200,
                CostPrice = 2.50m,
                Status = BatchStatus.Active
            },
            // Ciprofloxacin batch (expiring soon)
            new()
            {
                DrugId = drugs[4].Id,
                BatchNumber = "BTH-2024-003",
                ManufactureDate = new DateTime(2024, 3, 15),
                ExpiryDate = DateTime.Now.AddDays(25),
                InitialQuantity = 1500,
                CurrentQuantity = 800,
                ReservedQuantity = 0,
                CostPrice = 10.00m,
                Status = BatchStatus.Active
            },
            // Metformin batch
            new()
            {
                DrugId = drugs[5].Id,
                BatchNumber = "BTH-2024-020",
                ManufactureDate = new DateTime(2024, 4, 1),
                ExpiryDate = new DateTime(2026, 4, 1),
                InitialQuantity = 2000,
                CurrentQuantity = 1500,
                ReservedQuantity = 0,
                CostPrice = 3.50m,
                Status = BatchStatus.Active
            }
        };

        await context.Batches.AddRangeAsync(batches);
        await context.SaveChangesAsync();

        // Seed Stock Movements (initial purchases for batches)
        var stockMovements = new List<StockMovement>
        {
            // Paracetamol batch 1 - Initial purchase
            new()
            {
                DrugId = drugs[0].Id,
                BatchId = batches[0].Id,
                Type = MovementType.Purchase,
                Quantity = 5000,
                BalanceBefore = 0,
                BalanceAfter = 5000,
                Reference = "GRN-2024-001",
                Reason = "Initial purchase",
                Notes = "Received from supplier PharmaCo",
                CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0),
                CreatedBy = "System"
            },
            // Paracetamol batch 1 - Sales
            new()
            {
                DrugId = drugs[0].Id,
                BatchId = batches[0].Id,
                Type = MovementType.Sale,
                Quantity = -1800,
                BalanceBefore = 5000,
                BalanceAfter = 3200,
                Reference = "Various sales",
                Reason = "Retail sales",
                CreatedAt = DateTime.Now.AddDays(-10),
                CreatedBy = "System"
            },
            // Amoxicillin - Initial purchase
            new()
            {
                DrugId = drugs[1].Id,
                BatchId = batches[2].Id,
                Type = MovementType.Purchase,
                Quantity = 3000,
                BalanceBefore = 0,
                BalanceAfter = 3000,
                Reference = "GRN-2024-002",
                Reason = "Initial purchase",
                Notes = "Received from supplier MediCorp",
                CreatedAt = new DateTime(2024, 2, 10, 14, 15, 0),
                CreatedBy = "System"
            },
            // Amoxicillin - Sales
            new()
            {
                DrugId = drugs[1].Id,
                BatchId = batches[2].Id,
                Type = MovementType.Sale,
                Quantity = -1200,
                BalanceBefore = 3000,
                BalanceAfter = 1800,
                Reference = "Various sales",
                Reason = "Retail sales",
                CreatedAt = DateTime.Now.AddDays(-5),
                CreatedBy = "System"
            },
            // Ibuprofen - Initial purchase
            new()
            {
                DrugId = drugs[2].Id,
                BatchId = batches[3].Id,
                Type = MovementType.Purchase,
                Quantity = 4000,
                BalanceBefore = 0,
                BalanceAfter = 4000,
                Reference = "GRN-2023-045",
                Reason = "Initial purchase",
                CreatedAt = new DateTime(2023, 5, 20, 9, 0, 0),
                CreatedBy = "System"
            },
            // Ibuprofen - Sales and adjustments
            new()
            {
                DrugId = drugs[2].Id,
                BatchId = batches[3].Id,
                Type = MovementType.Sale,
                Quantity = -3300,
                BalanceBefore = 4000,
                BalanceAfter = 700,
                Reference = "Various sales",
                Reason = "Retail sales",
                CreatedAt = DateTime.Now.AddDays(-30),
                CreatedBy = "System"
            },
            new()
            {
                DrugId = drugs[2].Id,
                BatchId = batches[3].Id,
                Type = MovementType.Adjustment,
                Quantity = -50,
                BalanceBefore = 700,
                BalanceAfter = 650,
                Reference = "ADJ-2024-001",
                Reason = "Damaged",
                Notes = "Found 50 damaged tablets during stock check",
                CreatedAt = DateTime.Now.AddDays(-3),
                CreatedBy = "Admin"
            },
            // Ciprofloxacin - Initial purchase
            new()
            {
                DrugId = drugs[4].Id,
                BatchId = batches[4].Id,
                Type = MovementType.Purchase,
                Quantity = 1500,
                BalanceBefore = 0,
                BalanceAfter = 1500,
                Reference = "GRN-2024-003",
                Reason = "Initial purchase",
                CreatedAt = new DateTime(2024, 3, 15, 11, 45, 0),
                CreatedBy = "System"
            },
            // Ciprofloxacin - Sales
            new()
            {
                DrugId = drugs[4].Id,
                BatchId = batches[4].Id,
                Type = MovementType.Sale,
                Quantity = -700,
                BalanceBefore = 1500,
                BalanceAfter = 800,
                Reference = "Various sales",
                Reason = "Retail sales",
                CreatedAt = DateTime.Now.AddDays(-7),
                CreatedBy = "System"
            }
        };

        await context.StockMovements.AddRangeAsync(stockMovements);
        await context.SaveChangesAsync();
    }
}
