using inven_edu.Models.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace inven_edu.Services
{
    /// <summary>
    /// Implementation of PDF generation services
    /// </summary>
    public class PdfService : IPdfService
    {
        private readonly ILogger<PdfService> _logger;

        public PdfService(ILogger<PdfService> logger)
        {
            _logger = logger;
        }

        public byte[] GenerateInventoryReport(IEnumerable<InventoryItem> items)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var title = new Paragraph("InvenEdu - Inventory Report", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                // Date
                var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var date = new Paragraph($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}", dateFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 20
                };
                document.Add(date);

                // Table
                var table = new PdfPTable(5) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 3, 2, 1.5f, 1.5f, 1.5f });

                // Header
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
                var headerColor = new BaseColor(54, 116, 181);

                AddTableHeader(table, "Item Name", headerFont, headerColor);
                AddTableHeader(table, "Category", headerFont, headerColor);
                AddTableHeader(table, "Quantity", headerFont, headerColor);
                AddTableHeader(table, "Min Stock", headerFont, headerColor);
                AddTableHeader(table, "Status", headerFont, headerColor);

                // Data
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                foreach (var item in items)
                {
                    table.AddCell(new PdfPCell(new Phrase(item.Name, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Category?.Name ?? "N/A", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.MinimumStock.ToString(), cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER });

                    var status = item.IsOutOfStock ? "Out of Stock" : item.IsLowStock ? "Low Stock" : "In Stock";
                    var statusCell = new PdfPCell(new Phrase(status, cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    
                    if (item.IsOutOfStock)
                        statusCell.BackgroundColor = new BaseColor(220, 53, 69);
                    else if (item.IsLowStock)
                        statusCell.BackgroundColor = new BaseColor(255, 193, 7);
                    
                    table.AddCell(statusCell);
                }

                document.Add(table);

                // Summary
                var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var summary = new Paragraph($"\nTotal Items: {items.Count()}", summaryFont)
                {
                    SpacingBefore = 20
                };
                document.Add(summary);

                document.Close();
                writer.Close();

                _logger.LogInformation("Generated inventory report PDF with {ItemCount} items", items.Count());

                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating inventory report PDF");
                throw;
            }
        }

        public byte[] GenerateIssuanceReport(IEnumerable<IssuanceRecord> issuances)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                var document = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var title = new Paragraph("InvenEdu - Issuance Report", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                // Date
                var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var date = new Paragraph($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}", dateFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 20
                };
                document.Add(date);

                // Table
                var table = new PdfPTable(6) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 2.5f, 2.5f, 1.5f, 2f, 2f, 1.5f });

                // Header
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
                var headerColor = new BaseColor(54, 116, 181);

                AddTableHeader(table, "Item", headerFont, headerColor);
                AddTableHeader(table, "User", headerFont, headerColor);
                AddTableHeader(table, "Quantity", headerFont, headerColor);
                AddTableHeader(table, "Issued Date", headerFont, headerColor);
                AddTableHeader(table, "Return Date", headerFont, headerColor);
                AddTableHeader(table, "Status", headerFont, headerColor);

                // Data
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                foreach (var issuance in issuances)
                {
                    table.AddCell(new PdfPCell(new Phrase(issuance.Item?.Name ?? "N/A", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(issuance.User?.FullName ?? "N/A", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(issuance.QuantityIssued.ToString(), cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(issuance.IssuedDate.ToString("yyyy-MM-dd"), cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(issuance.ReturnDate?.ToString("yyyy-MM-dd") ?? "N/A", cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    
                    var statusCell = new PdfPCell(new Phrase(issuance.Status, cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    
                    if (issuance.IsOverdue)
                        statusCell.BackgroundColor = new BaseColor(220, 53, 69);
                    else if (issuance.Status == IssuanceStatus.Issued)
                        statusCell.BackgroundColor = new BaseColor(23, 162, 184);
                    else if (issuance.Status == IssuanceStatus.Returned)
                        statusCell.BackgroundColor = new BaseColor(40, 167, 69);
                    
                    table.AddCell(statusCell);
                }

                document.Add(table);

                // Summary
                var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var summary = new Paragraph($"\nTotal Issuances: {issuances.Count()}", summaryFont)
                {
                    SpacingBefore = 20
                };
                document.Add(summary);

                document.Close();
                writer.Close();

                _logger.LogInformation("Generated issuance report PDF with {IssuanceCount} records", issuances.Count());

                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating issuance report PDF");
                throw;
            }
        }

        private void AddTableHeader(PdfPTable table, string text, Font font, BaseColor color)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                BackgroundColor = color,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 8,
                BorderWidth = 1
            };
            table.AddCell(cell);
        }
    }
}
