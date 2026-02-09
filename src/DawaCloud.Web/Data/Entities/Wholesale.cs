namespace DawaCloud.Web.Data.Entities;

public class WholesaleCustomer : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? TaxId { get; set; }

    // Credit limit in base currency (SSP)
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }

    // Default currency preference for this customer
    public int? DefaultCurrencyId { get; set; }

    public int PaymentTerms { get; set; } = 30; // Days
    public PricingTier PricingTier { get; set; } = PricingTier.Standard;
    public bool IsActive { get; set; } = true;

    // Navigation
    public Currency? DefaultCurrency { get; set; }
    public ICollection<WholesaleSale> Sales { get; set; } = new List<WholesaleSale>();
}

public class WholesaleSale : BaseAuditableEntity
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }

    // Currency tracking (amounts in base currency SSP by default)
    public int? CurrencyId { get; set; }
    public decimal? ExchangeRateUsed { get; set; }

    // Amounts in base currency (SSP)
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount => Math.Max(0, TotalAmount - PaidAmount);

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public SaleStatus Status { get; set; } = SaleStatus.Draft;
    public string? DeliveryAddress { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Currency? Currency { get; set; }
    public WholesaleCustomer Customer { get; set; } = null!;
    public ICollection<WholesaleSaleItem> Items { get; set; } = new List<WholesaleSaleItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class WholesaleSaleItem : BaseAuditableEntity
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int DrugId { get; set; }
    public int? BatchId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxRate { get; set; }
    public decimal LineTotal { get; set; }
    
    public WholesaleSale Sale { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
    public Batch? Batch { get; set; }
}

public class Payment : BaseAuditableEntity
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public int? WholesaleSaleId { get; set; }
    public int? RetailSaleId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    // Currency tracking
    public int? CurrencyId { get; set; }
    public decimal Amount { get; set; }
    public decimal AmountBase { get; set; }
    public decimal? ExchangeRateUsed { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Currency? Currency { get; set; }
    public WholesaleSale? WholesaleSale { get; set; }
    public RetailSale? RetailSale { get; set; }
}

public class Quotation : BaseAuditableEntity
{
    public int Id { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public DateTime QuotationDate { get; set; } = DateTime.UtcNow;
    public DateTime ValidUntil { get; set; }

    // Currency (defaults to SSP for sales quotes)
    public int? CurrencyId { get; set; }
    public decimal? ExchangeRateUsed { get; set; }

    // Amounts in base currency (SSP)
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public QuotationStatus Status { get; set; } = QuotationStatus.Draft;
    public string? Notes { get; set; }
    public int? ConvertedToSaleId { get; set; }

    // Navigation
    public Currency? Currency { get; set; }
    public WholesaleCustomer Customer { get; set; } = null!;
    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
}

public class QuotationItem : BaseAuditableEntity
{
    public int Id { get; set; }
    public int QuotationId { get; set; }
    public int DrugId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxRate { get; set; }
    public decimal LineTotal { get; set; }
    
    public Quotation Quotation { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
}

public enum PricingTier
{
    Standard,
    Silver,
    Gold,
    Platinum
}

public enum PaymentStatus
{
    Pending,
    PartiallyPaid,
    Paid,
    Overdue
}

public enum SaleStatus
{
    Draft,
    Confirmed,
    Processing,
    Shipped,
    Delivered,
    Completed,
    Cancelled
}

public enum PaymentMethod
{
    Cash,
    Card,
    MoMo,
    BankTransfer,
    Cheque,
    Credit
}

public enum QuotationStatus
{
    Draft,
    Sent,
    Accepted,
    Rejected,
    Expired,
    Converted
}
