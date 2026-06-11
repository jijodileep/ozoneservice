using OzoneMobileService.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OzoneMobileService.Infrastructure.Services;

public static class InvoicePdfGenerator
{
    static InvoicePdfGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static byte[] Generate(
        Invoice invoice,
        string shopName,
        string branchName,
        decimal cgstRate,
        decimal sgstRate)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Header().Column(col =>
                {
                    col.Item().Text(shopName).FontSize(20).Bold();
                    col.Item().Text(branchName).FontSize(12);
                    col.Item().Text($"Invoice {invoice.InvoiceNumber}").FontSize(16).Bold();
                    col.Item().Text($"Type: {invoice.InvoiceType}");
                    col.Item().Text($"Date: {invoice.IssuedAt:yyyy-MM-dd}");
                });

                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Item().Text($"Customer: {invoice.CustomerName}");
                    col.Item().Text($"Phone: {invoice.CustomerPhone}");
                    col.Item().PaddingTop(16).LineHorizontal(1);
                    col.Item().PaddingTop(8).Row(row =>
                    {
                        row.RelativeItem().Text("Subtotal").Bold();
                        row.ConstantItem(100).AlignRight().Text($"₹{invoice.SubTotal:N2}");
                    });

                    if (invoice.CgstAmount > 0 || invoice.SgstAmount > 0)
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"CGST @ {cgstRate:N2}%");
                            row.ConstantItem(100).AlignRight().Text($"₹{invoice.CgstAmount:N2}");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"SGST @ {sgstRate:N2}%");
                            row.ConstantItem(100).AlignRight().Text($"₹{invoice.SgstAmount:N2}");
                        });
                    }
                    else
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Tax");
                            row.ConstantItem(100).AlignRight().Text($"₹{invoice.TaxAmount:N2}");
                        });
                    }

                    col.Item().PaddingTop(8).Row(row =>
                    {
                        row.RelativeItem().Text("Total").Bold().FontSize(14);
                        row.ConstantItem(100).AlignRight().Text($"₹{invoice.TotalAmount:N2}").Bold().FontSize(14);
                    });
                });

                page.Footer().AlignCenter().Text("Thank you for your business.");
            });
        }).GeneratePdf();
    }
}
