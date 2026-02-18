namespace DawaCloud.Web.Data.Entities;

public class Notification : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public string? RecipientEmail { get; set; }
    public string? RecipientPhone { get; set; }
    public string? RecipientUserId { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? Payload { get; set; } // JSON payload for additional data
    public DateTime? SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
}

public class NotificationTemplate : BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Localization fields
    public string Locale { get; set; } = "en-US";
    public string? VariablePlaceholders { get; set; } // JSON array: ["{{ProductName}}", "{{Quantity}}"]
}

public enum NotificationType
{
    StockAlert,
    ExpiryAlert,
    OrderUpdate,
    SalesAlert,
    SystemAlert,
    DeliveryUpdate,
    PaymentReceived,
    NewRequest
}

public enum NotificationChannel
{
    InApp,
    Email,
    WhatsApp,
    SMS
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Delivered,
    Read,
    Failed
}
