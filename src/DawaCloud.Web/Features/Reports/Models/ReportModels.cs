namespace DawaCloud.Web.Features.Reports.Models;

public class ReportResult
{
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<ReportColumn> Columns { get; set; } = new();
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
    public Dictionary<string, object?> Summary { get; set; } = new();
}

public class ReportColumn
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public ColumnFormat Format { get; set; } = ColumnFormat.Text;
}

public enum ColumnFormat
{
    Text,
    Number,
    Currency,
    Date,
    DateTime,
    Percent
}
