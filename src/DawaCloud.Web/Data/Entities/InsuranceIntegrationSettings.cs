namespace DawaCloud.Web.Data.Entities;

public class InsuranceIntegrationSettings : BaseAuditableEntity
{
    public int Id { get; set; }
    public bool IsEnabled { get; set; }
    public string? IntegrationProvider { get; set; }
    public string? ApiEndpointUrl { get; set; }
    public string? ApiKey { get; set; }
    public string? ApiSecret { get; set; }
    public string? WebhookUrl { get; set; }
    public DateTime? LastConnectionTestAt { get; set; }
    public bool? LastConnectionTestSuccess { get; set; }
    public string? LastConnectionTestMessage { get; set; }
}
