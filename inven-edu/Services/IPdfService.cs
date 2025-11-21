using inven_edu.Models.Entities;

namespace inven_edu.Services
{
    /// <summary>
    /// Interface for PDF generation services
    /// </summary>
    public interface IPdfService
    {
        /// <summary>
        /// Generates a PDF report for inventory items
        /// </summary>
        byte[] GenerateInventoryReport(IEnumerable<InventoryItem> items);

        /// <summary>
        /// Generates a PDF report for issuance records
        /// </summary>
        byte[] GenerateIssuanceReport(IEnumerable<IssuanceRecord> issuances);
    }
}
