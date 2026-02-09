using DawaFlow.Web.Data;
using DawaFlow.Web.Features.Wholesale.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DawaFlow.Web.Features.Wholesale.Services;

public interface IInvoiceService
{
    Task<byte[]> GenerateInvoicePdfAsync(int saleId, CancellationToken ct = default);
}

public class InvoiceService : IInvoiceService
{
    private readonly AppDbContext _context;
    private readonly IMediator _mediator;

    public InvoiceService(AppDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(int saleId, CancellationToken ct = default)
    {
        // Get sale details
        var sale = await _context.WholesaleSales
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Drug)
            .Include(s => s.Items)
                .ThenInclude(i => i.Batch)
            .FirstOrDefaultAsync(s => s.Id == saleId && !s.IsDeleted, ct);

        if (sale == null)
        {
            throw new InvalidOperationException($"Sale with ID {saleId} not found");
        }

        // Get company settings
        var companySettings = await _context.CompanySettings.FirstOrDefaultAsync(ct);

        // Generate PDF
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.PageColor(Colors.White);

                page.Header().Element(container => ComposeHeader(container, companySettings));
                page.Content().Element(container => ComposeContent(container, sale, companySettings));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, Data.Entities.CompanySettings? companySettings)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("INVOICE").FontSize(28).Bold().FontColor(Colors.Teal.Darken2);
                column.Item().Text(companySettings?.CompanyName ?? "Company Name").FontSize(16).SemiBold();
                column.Item().Text(companySettings?.Address ?? "Address").FontSize(10);
                column.Item().Text($"Tel: {companySettings?.Phone ?? "N/A"}").FontSize(10);
                column.Item().Text($"Email: {companySettings?.Email ?? "N/A"}").FontSize(10);
            });

            row.ConstantItem(100).AlignRight().Column(column =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(companySettings?.LogoUrl))
                    {
                        var logoPath = Path.Combine("wwwroot", companySettings.LogoUrl.TrimStart('/'));
                        if (File.Exists(logoPath))
                        {
                            column.Item().Height(80).Width(80).Image(logoPath);
                            return;
                        }
                    }

                    // Fallback to styled box if logo not found
                    column.Item().Height(80).Width(80).Border(2).BorderColor(Colors.Teal.Darken2)
                        .Background(Colors.Teal.Lighten5).Padding(10)
                        .AlignCenter().AlignMiddle()
                        .Text(companySettings?.CompanyName?.Substring(0, Math.Min(10, companySettings.CompanyName.Length)) ?? "COMPANY").FontSize(10).Bold().FontColor(Colors.Teal.Darken2);
                }
                catch
                {
                    // Fallback on any error
                    column.Item().Height(80).Width(80).Border(2).BorderColor(Colors.Teal.Darken2)
                        .Background(Colors.Teal.Lighten5).Padding(10)
                        .AlignCenter().AlignMiddle()
                        .Text(companySettings?.CompanyName?.Substring(0, Math.Min(10, companySettings.CompanyName.Length)) ?? "COMPANY").FontSize(10).Bold().FontColor(Colors.Teal.Darken2);
                }
            });
        });
    }

    private void ComposeContent(IContainer container, Data.Entities.WholesaleSale sale, Data.Entities.CompanySettings? settings)
    {
        container.PaddingVertical(20).Column(column =>
        {
            // Invoice details section
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Bill To:").SemiBold().FontSize(11);
                    col.Item().Text(sale.Customer.Name).FontSize(10);
                    col.Item().Text(sale.Customer.Email ?? "").FontSize(9);
                    col.Item().Text(sale.Customer.Phone ?? "").FontSize(9);
                    if (!string.IsNullOrEmpty(sale.DeliveryAddress))
                    {
                        col.Item().Text($"Delivery: {sale.DeliveryAddress}").FontSize(9);
                    }
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text($"Invoice #: {sale.InvoiceNumber}").SemiBold().FontSize(10);
                    col.Item().Text($"Date: {sale.SaleDate:dd/MM/yyyy}").FontSize(10);
                    col.Item().Text($"Status: {sale.Status}").FontSize(10);
                    col.Item().Text($"Payment: {sale.PaymentStatus}").FontSize(10);
                });
            });

            column.Item().PaddingVertical(20);

            // Line items table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40);     // #
                    columns.RelativeColumn(3);      // Description
                    columns.RelativeColumn(2);      // Batch
                    columns.RelativeColumn(1);      // Qty
                    columns.RelativeColumn(2);      // Unit Price
                    columns.RelativeColumn(2);      // Total
                });

                // Table header
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#").SemiBold();
                    header.Cell().Element(CellStyle).Text("Description").SemiBold();
                    header.Cell().Element(CellStyle).Text("Batch").SemiBold();
                    header.Cell().Element(CellStyle).AlignCenter().Text("Qty").SemiBold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Unit Price").SemiBold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Total").SemiBold();

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Medium)
                            .PaddingVertical(5);
                    }
                });

                // Table rows
                int index = 1;
                foreach (var item in sale.Items)
                {
                    table.Cell().Element(CellStyle).Text(index.ToString());
                    table.Cell().Element(CellStyle).Column(col =>
                    {
                        col.Item().Text(item.Drug.Name).FontSize(10);
                        col.Item().Text(item.Drug.Code).FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                    table.Cell().Element(CellStyle).Text(item.Batch.BatchNumber).FontSize(9);
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity.ToString());
                    table.Cell().Element(CellStyle).AlignRight().Text($"SSP {item.UnitPrice:N2}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"SSP {item.LineTotal:N2}");

                    index++;

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .PaddingVertical(5);
                    }
                }
            });

            column.Item().PaddingTop(20);

            // Totals section
            column.Item().AlignRight().Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Subtotal:");
                    row.ConstantItem(120).AlignRight().Text($"SSP {sale.SubTotal:N2}");
                });

                if (sale.DiscountAmount > 0)
                {
                    col.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Discount:");
                        row.ConstantItem(120).AlignRight().Text($"- SSP {sale.DiscountAmount:N2}").FontColor(Colors.Green.Medium);
                    });
                }

                col.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Tax (16%):");
                    row.ConstantItem(120).AlignRight().Text($"SSP {sale.TaxAmount:N2}");
                });

                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);

                col.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Total Amount:").SemiBold().FontSize(12);
                    row.ConstantItem(120).AlignRight().Text($"SSP {sale.TotalAmount:N2}").SemiBold().FontSize(12).FontColor(Colors.Teal.Darken2);
                });

                col.Item().PaddingTop(10);

                col.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Paid:");
                    row.ConstantItem(120).AlignRight().Text($"SSP {sale.PaidAmount:N2}").FontColor(Colors.Green.Medium);
                });

                if (sale.BalanceAmount > 0)
                {
                    col.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Balance Due:").SemiBold();
                        row.ConstantItem(120).AlignRight().Text($"SSP {sale.BalanceAmount:N2}").SemiBold().FontColor(Colors.Red.Medium);
                    });
                }
            });

            // Notes section
            if (!string.IsNullOrEmpty(sale.Notes))
            {
                column.Item().PaddingTop(30).Column(col =>
                {
                    col.Item().Text("Notes:").SemiBold().FontSize(10);
                    col.Item().Text(sale.Notes).FontSize(9);
                });
            }

            // Terms and conditions
            column.Item().PaddingTop(30).Column(col =>
            {
                col.Item().Text("Terms & Conditions:").SemiBold().FontSize(9);
                col.Item().Text("Payment due within 30 days. Late payments subject to 2% monthly interest.").FontSize(8);
                col.Item().Text("Goods sold are not returnable unless defective.").FontSize(8);
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Column(column =>
        {
            column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(10);
            column.Item().Text("Thank you for your business!").SemiBold().FontSize(10);
            column.Item().Text("This is a computer-generated document. No signature required.").FontSize(7).FontColor(Colors.Grey.Medium);
        });
    }
}
