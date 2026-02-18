namespace DawaCloud.Web.Data.Entities;

public class LocalizedReportDefinition : BaseAuditableEntity
{
    public int Id { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string Locale { get; set; } = "en-US";
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public string ColumnDefinitionsJson { get; set; } = "[]";
    public bool IsActive { get; set; } = true;
}
