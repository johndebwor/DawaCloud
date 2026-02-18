using ClosedXML.Excel;
using DawaCloud.Web.Data.Entities;
using DawaCloud.Web.Features.Reports.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DawaCloud.Web.Features.Reports.Services;

public interface IReportExportService
{
    byte[] ExportToExcel(ReportResult report);
    byte[] ExportToPdf(ReportResult report, CompanySettings? companySettings);
}

public class ReportExportService : IReportExportService
{
    public byte[] ExportToExcel(ReportResult report)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add(report.Title.Length > 31 ? report.Title[..31] : report.Title);

        // Title row
        ws.Cell(1, 1).Value = report.Title;
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Range(1, 1, 1, report.Columns.Count).Merge();

        // Date range row
        var dateRange = "";
        if (report.StartDate.HasValue && report.EndDate.HasValue)
            dateRange = $"Period: {report.StartDate.Value:dd/MM/yyyy} - {report.EndDate.Value:dd/MM/yyyy}";
        if (!string.IsNullOrEmpty(dateRange))
        {
            ws.Cell(2, 1).Value = dateRange;
            ws.Cell(2, 1).Style.Font.Italic = true;
            ws.Range(2, 1, 2, report.Columns.Count).Merge();
        }

        ws.Cell(3, 1).Value = $"Generated: {report.GeneratedAt:dd/MM/yyyy HH:mm}";
        ws.Cell(3, 1).Style.Font.FontSize = 9;
        ws.Range(3, 1, 3, report.Columns.Count).Merge();

        // Headers (row 5)
        var headerRow = 5;
        for (int col = 0; col < report.Columns.Count; col++)
        {
            ws.Cell(headerRow, col + 1).Value = report.Columns[col].Label;
        }

        var headerRange = ws.Range(headerRow, 1, headerRow, report.Columns.Count);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1565C0");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Data rows
        int dataRow = headerRow + 1;
        foreach (var row in report.Rows)
        {
            for (int col = 0; col < report.Columns.Count; col++)
            {
                var column = report.Columns[col];
                var value = row.GetValueOrDefault(column.Key);
                var cell = ws.Cell(dataRow, col + 1);

                if (value == null)
                {
                    cell.Value = "";
                    continue;
                }

                switch (column.Format)
                {
                    case ColumnFormat.Currency:
                        if (value is decimal decVal)
                        {
                            cell.Value = decVal;
                            cell.Style.NumberFormat.Format = "#,##0.00";
                        }
                        else cell.Value = value.ToString();
                        break;

                    case ColumnFormat.Number:
                        if (value is int intVal) cell.Value = intVal;
                        else if (value is long longVal) cell.Value = longVal;
                        else if (value is double dblVal) { cell.Value = dblVal; cell.Style.NumberFormat.Format = "#,##0.0"; }
                        else cell.Value = value.ToString();
                        break;

                    case ColumnFormat.Percent:
                        if (value is decimal pctDec) { cell.Value = pctDec; cell.Style.NumberFormat.Format = "#,##0.00\"%\""; }
                        else if (value is double pctDbl) { cell.Value = pctDbl; cell.Style.NumberFormat.Format = "#,##0.0\"%\""; }
                        else cell.Value = value.ToString();
                        break;

                    case ColumnFormat.Date:
                        if (value is DateTime dtVal) { cell.Value = dtVal; cell.Style.DateFormat.Format = "dd/MM/yyyy"; }
                        else cell.Value = value.ToString();
                        break;

                    case ColumnFormat.DateTime:
                        if (value is DateTime dtTimeVal) { cell.Value = dtTimeVal; cell.Style.DateFormat.Format = "dd/MM/yyyy HH:mm"; }
                        else cell.Value = value.ToString();
                        break;

                    default:
                        cell.Value = value.ToString();
                        break;
                }
            }
            dataRow++;
        }

        // Summary section
        if (report.Summary.Count > 0)
        {
            dataRow += 1;
            ws.Cell(dataRow, 1).Value = "Summary";
            ws.Cell(dataRow, 1).Style.Font.Bold = true;
            ws.Cell(dataRow, 1).Style.Font.FontSize = 12;
            dataRow++;

            foreach (var (key, value) in report.Summary)
            {
                ws.Cell(dataRow, 1).Value = key;
                ws.Cell(dataRow, 1).Style.Font.Bold = true;
                if (value is decimal sumDec)
                {
                    ws.Cell(dataRow, 2).Value = sumDec;
                    ws.Cell(dataRow, 2).Style.NumberFormat.Format = "#,##0.00";
                }
                else
                {
                    ws.Cell(dataRow, 2).Value = value?.ToString() ?? "";
                }
                dataRow++;
            }
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportToPdf(ReportResult report, CompanySettings? settings)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.PageColor(Colors.White);

                page.Header().Element(c => ComposeHeader(c, report, settings));
                page.Content().Element(c => ComposeContent(c, report));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, ReportResult report, CompanySettings? settings)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(settings?.CompanyName ?? "DawaCloud").FontSize(16).SemiBold();
                    if (!string.IsNullOrEmpty(settings?.Address))
                        col.Item().Text($"{settings.Address}, {settings.City ?? ""}, {settings.Country ?? ""}").FontSize(9);
                    if (!string.IsNullOrEmpty(settings?.Phone))
                        col.Item().Text($"Tel: {settings.Phone}").FontSize(9);
                });

                row.ConstantItem(100).AlignRight().Column(col =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(settings?.LogoUrl))
                        {
                            var logoPath = Path.Combine("wwwroot", settings.LogoUrl.TrimStart('/'));
                            if (File.Exists(logoPath))
                            {
                                col.Item().Height(60).Width(60).Image(logoPath);
                                return;
                            }
                        }
                    }
                    catch { }
                    col.Item().Height(60).Width(60).Border(1).BorderColor(Colors.Grey.Lighten2)
                        .Background(Colors.Grey.Lighten4).Padding(5)
                        .AlignCenter().AlignMiddle()
                        .Text(settings?.CompanyName?[..Math.Min(settings.CompanyName.Length, 8)] ?? "DC")
                        .FontSize(9).Bold();
                });
            });

            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Blue.Darken2);

            column.Item().PaddingTop(10).Text(report.Title).FontSize(14).Bold().FontColor(Colors.Blue.Darken2);

            if (!string.IsNullOrEmpty(report.SubTitle))
                column.Item().Text(report.SubTitle).FontSize(9).FontColor(Colors.Grey.Darken1);

            var dateInfo = "";
            if (report.StartDate.HasValue && report.EndDate.HasValue)
                dateInfo = $"Period: {report.StartDate.Value:dd/MM/yyyy} - {report.EndDate.Value:dd/MM/yyyy}";
            dateInfo += $"  |  Generated: {report.GeneratedAt:dd/MM/yyyy HH:mm}";
            column.Item().Text(dateInfo.TrimStart()).FontSize(8).FontColor(Colors.Grey.Medium);

            column.Item().PaddingTop(5);
        });
    }

    private void ComposeContent(IContainer container, ReportResult report)
    {
        container.Column(column =>
        {
            // Summary KPIs
            if (report.Summary.Count > 0)
            {
                column.Item().PaddingBottom(10).Row(row =>
                {
                    foreach (var (key, value) in report.Summary)
                    {
                        row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2)
                            .Background(Colors.Grey.Lighten4).Padding(8).Column(col =>
                            {
                                col.Item().Text(key).FontSize(8).FontColor(Colors.Grey.Darken1);
                                var displayValue = value is decimal d ? $"SSP {d:N2}" : value?.ToString() ?? "0";
                                col.Item().Text(displayValue).FontSize(11).SemiBold();
                            });
                    }
                });
            }

            if (report.Rows.Count == 0)
            {
                column.Item().PaddingTop(30).AlignCenter().Text("No data found for the selected criteria.")
                    .FontSize(12).FontColor(Colors.Grey.Medium);
                return;
            }

            // Data table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    foreach (var col in report.Columns)
                    {
                        if (col.Format == ColumnFormat.Currency)
                            columns.RelativeColumn(2);
                        else if (col.Format == ColumnFormat.Text && (col.Key.Contains("name") || col.Key.Contains("drug") || col.Key.Contains("customer") || col.Key.Contains("supplier")))
                            columns.RelativeColumn(3);
                        else
                            columns.RelativeColumn(1.5f);
                    }
                });

                table.Header(header =>
                {
                    foreach (var col in report.Columns)
                    {
                        header.Cell().Element(HeaderCellStyle).Text(col.Label).FontSize(8).SemiBold();
                    }

                    static IContainer HeaderCellStyle(IContainer container) =>
                        container.Background(Colors.Blue.Darken2).Padding(4).DefaultTextStyle(x => x.FontColor(Colors.White));
                });

                var rowIndex = 0;
                foreach (var row in report.Rows)
                {
                    var bgColor = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                    foreach (var col in report.Columns)
                    {
                        var value = row.GetValueOrDefault(col.Key);
                        var cellText = FormatValue(value, col.Format);

                        var cell = table.Cell().Element(c => DataCellStyle(c, bgColor));

                        if (col.Format == ColumnFormat.Currency || col.Format == ColumnFormat.Number || col.Format == ColumnFormat.Percent)
                            cell.AlignRight().Text(cellText).FontSize(8);
                        else
                            cell.Text(cellText).FontSize(8);
                    }
                    rowIndex++;
                }

                static IContainer DataCellStyle(IContainer container, string bgColor) =>
                    container.Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4);
            });

            column.Item().PaddingTop(5).Text($"Total rows: {report.Rows.Count}").FontSize(8).FontColor(Colors.Grey.Medium);
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Column(column =>
        {
            column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(5);
            column.Item().Row(row =>
            {
                row.RelativeItem().Text("DawaCloud - Pharmaceutical Management System").FontSize(7).FontColor(Colors.Grey.Medium);
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Page ").FontSize(7).FontColor(Colors.Grey.Medium);
                    text.CurrentPageNumber().FontSize(7).FontColor(Colors.Grey.Medium);
                    text.Span(" of ").FontSize(7).FontColor(Colors.Grey.Medium);
                    text.TotalPages().FontSize(7).FontColor(Colors.Grey.Medium);
                });
            });
        });
    }

    private static string FormatValue(object? value, ColumnFormat format)
    {
        if (value == null) return "";

        return format switch
        {
            ColumnFormat.Currency => value is decimal d ? $"SSP {d:N2}" : value.ToString() ?? "",
            ColumnFormat.Number => value is int i ? i.ToString("N0") : value is long l ? l.ToString("N0") : value is double db ? db.ToString("N1") : value.ToString() ?? "",
            ColumnFormat.Percent => value is decimal p ? $"{p:N2}%" : value is double pd ? $"{pd:N1}%" : value.ToString() ?? "",
            ColumnFormat.Date => value is DateTime dt ? dt.ToString("dd/MM/yyyy") : value.ToString() ?? "",
            ColumnFormat.DateTime => value is DateTime dtm ? dtm.ToString("dd/MM/yyyy HH:mm") : value.ToString() ?? "",
            _ => value.ToString() ?? ""
        };
    }
}
