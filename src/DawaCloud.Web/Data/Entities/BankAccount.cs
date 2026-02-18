namespace DawaCloud.Web.Data.Entities;

public class BankAccount : BaseAuditableEntity
{
    public int Id { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string? AccountNumber { get; set; }
    public string? BranchCode { get; set; }
    public int? CurrencyId { get; set; }
    public BankAccountType AccountType { get; set; } = BankAccountType.Current;
    public decimal CurrentBalance { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    // Future integration fields
    public bool IntegrationEnabled { get; set; }
    public string? IntegrationProvider { get; set; }
    public string? IntegrationApiKey { get; set; }
    public DateTime? IntegrationLastSync { get; set; }

    // Navigation
    public Currency? Currency { get; set; }
}

public enum BankAccountType
{
    Current,
    Savings,
    FixedDeposit,
    MobileMoney
}
