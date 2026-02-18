using ClosedXML.Excel;
using DawaCloud.Web.Features.Expenses.Queries;

namespace DawaCloud.Web.Features.Expenses.Services;

public class ExpenseExportService
{
    public byte[] ExportToExcel(List<ExpenseDto> expenses)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Expenses");

        // Headers
        var headers = new[] { "Reference", "Date", "Category", "Description", "Vendor", "Amount", "Currency", "Payment Method", "Status", "Approved By", "Approved Date" };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1565C0");
            cell.Style.Font.FontColor = XLColor.White;
        }

        // Data
        for (int row = 0; row < expenses.Count; row++)
        {
            var e = expenses[row];
            worksheet.Cell(row + 2, 1).Value = e.ReferenceNumber;
            worksheet.Cell(row + 2, 2).Value = e.Date.ToString("yyyy-MM-dd");
            worksheet.Cell(row + 2, 3).Value = e.CategoryName;
            worksheet.Cell(row + 2, 4).Value = e.Description;
            worksheet.Cell(row + 2, 5).Value = e.VendorName ?? "";
            worksheet.Cell(row + 2, 6).Value = (double)e.Amount;
            worksheet.Cell(row + 2, 6).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row + 2, 7).Value = e.CurrencyCode ?? "Base";
            worksheet.Cell(row + 2, 8).Value = e.PaymentMethod;
            worksheet.Cell(row + 2, 9).Value = e.Status.ToString();
            worksheet.Cell(row + 2, 10).Value = e.ApprovedBy ?? "";
            worksheet.Cell(row + 2, 11).Value = e.ApprovedAt?.ToString("yyyy-MM-dd") ?? "";
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
