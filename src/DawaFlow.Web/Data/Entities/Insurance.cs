namespace DawaFlow.Web.Data.Entities;

/// <summary>
/// Health/Medical Insurance Provider (e.g., Phoenix Health, National Health Insurance Fund)
/// </summary>
public class InsuranceProvider : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // PHOENIX, NHIF, etc.
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }

    /// <summary>
    /// Default coverage percentage for prescription drugs (e.g., 80%)
    /// </summary>
    public decimal DefaultCoveragePercentage { get; set; } = 80m;

    /// <summary>
    /// Average days to process claims
    /// </summary>
    public int ClaimProcessingDays { get; set; } = 30;

    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // Navigation
    public ICollection<InsuredPatient> Patients { get; set; } = new List<InsuredPatient>();
    public ICollection<InsuranceClaim> Claims { get; set; } = new List<InsuranceClaim>();
}

/// <summary>
/// Patient registered with health insurance coverage
/// </summary>
public class InsuredPatient : BaseAuditableEntity
{
    public int Id { get; set; }
    public int ProviderId { get; set; }

    /// <summary>
    /// Insurance member/policy number
    /// </summary>
    public string MemberNumber { get; set; } = string.Empty;

    // Patient Information
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty; // Male, Female, Other
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? IdNumber { get; set; } // National ID

    // Coverage Details
    public DateTime CoverageStartDate { get; set; }
    public DateTime? CoverageEndDate { get; set; }

    /// <summary>
    /// Annual coverage limit in SSP (null = unlimited)
    /// </summary>
    public decimal? AnnualCoverageLimit { get; set; }

    /// <summary>
    /// Lifetime coverage limit in SSP (null = unlimited)
    /// </summary>
    public decimal? LifetimeCoverageLimit { get; set; }

    /// <summary>
    /// Amount used in current year
    /// </summary>
    public decimal YearToDateUsed { get; set; }

    /// <summary>
    /// Total amount used lifetime
    /// </summary>
    public decimal LifetimeUsed { get; set; }

    /// <summary>
    /// Custom coverage percentage (overrides provider default if set)
    /// </summary>
    public decimal? CoveragePercentage { get; set; }

    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // Navigation
    public InsuranceProvider Provider { get; set; } = null!;
    public ICollection<InsuranceClaim> Claims { get; set; } = new List<InsuranceClaim>();
}

/// <summary>
/// Insurance claim for prescription medication costs
/// </summary>
public class InsuranceClaim : BaseAuditableEntity
{
    public int Id { get; set; }
    public string ClaimNumber { get; set; } = string.Empty; // CLM-2024-0001
    public int ProviderId { get; set; }
    public int PatientId { get; set; }
    public int? SaleId { get; set; } // Link to retail sale if applicable

    public DateTime ClaimDate { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public DateTime? PaidDate { get; set; }

    /// <summary>
    /// Total prescription amount
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Amount covered by insurance
    /// </summary>
    public decimal CoveredAmount { get; set; }

    /// <summary>
    /// Patient co-payment (patient pays this)
    /// </summary>
    public decimal CoPaymentAmount { get; set; }

    /// <summary>
    /// Amount approved by insurance provider
    /// </summary>
    public decimal? ApprovedAmount { get; set; }

    /// <summary>
    /// Amount actually paid by insurance provider
    /// </summary>
    public decimal? PaidAmount { get; set; }

    public ClaimStatus Status { get; set; } = ClaimStatus.Draft;

    /// <summary>
    /// Provider's reference number for this claim
    /// </summary>
    public string? ProviderReference { get; set; }

    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public InsuranceProvider Provider { get; set; } = null!;
    public InsuredPatient Patient { get; set; } = null!;
    public RetailSale? Sale { get; set; }
    public ICollection<InsuranceClaimItem> Items { get; set; } = new List<InsuranceClaimItem>();
}

/// <summary>
/// Individual prescription items in an insurance claim
/// </summary>
public class InsuranceClaimItem : BaseAuditableEntity
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int DrugId { get; set; }
    public int? BatchId { get; set; }

    public string DrugName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Amount covered by insurance for this item
    /// </summary>
    public decimal CoveredAmount { get; set; }

    /// <summary>
    /// Amount approved by provider for this item
    /// </summary>
    public decimal? ApprovedAmount { get; set; }

    public bool IsApproved { get; set; }
    public string? RejectionReason { get; set; }

    // Navigation
    public InsuranceClaim Claim { get; set; } = null!;
    public Drug Drug { get; set; } = null!;
    public Batch? Batch { get; set; }
}

public enum ClaimStatus
{
    /// <summary>
    /// Claim created but not submitted
    /// </summary>
    Draft,

    /// <summary>
    /// Awaiting initial review
    /// </summary>
    Pending,

    /// <summary>
    /// Submitted to insurance provider
    /// </summary>
    Submitted,

    /// <summary>
    /// Provider is reviewing the claim
    /// </summary>
    UnderReview,

    /// <summary>
    /// Provider approved the claim
    /// </summary>
    Approved,

    /// <summary>
    /// Some items approved, others rejected
    /// </summary>
    PartiallyApproved,

    /// <summary>
    /// Claim fully rejected
    /// </summary>
    Rejected,

    /// <summary>
    /// Provider has paid the claim
    /// </summary>
    Paid,

    /// <summary>
    /// Claim cancelled by user
    /// </summary>
    Cancelled
}
