using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inven_edu.Models.Entities
{
    /// <summary>
    /// Represents a record of an item issuance to a user
    /// </summary>
    public class IssuanceRecord
    {
        /// <summary>
        /// Gets or sets the unique identifier for the issuance record
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the inventory item ID
        /// </summary>
        [Required]
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the user ID who received the item
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the quantity issued
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity issued must be at least 1")]
        public int QuantityIssued { get; set; }

        /// <summary>
        /// Gets or sets when the item was issued
        /// </summary>
        [Required]
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets when the item was or is expected to be returned
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the current status of the issuance
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = IssuanceStatus.Issued;

        /// <summary>
        /// Gets or sets any additional notes about the issuance
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Navigation property to the inventory item
        /// </summary>
        [ForeignKey("ItemId")]
        public virtual InventoryItem Item { get; set; } = null!;

        /// <summary>
        /// Navigation property to the user
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Determines if the issuance is overdue
        /// </summary>
        [NotMapped]
        public bool IsOverdue => ReturnDate.HasValue && ReturnDate.Value < DateTime.UtcNow && Status == IssuanceStatus.Issued;
    }

    /// <summary>
    /// Constants for issuance status values
    /// </summary>
    public static class IssuanceStatus
    {
        public const string Issued = "Issued";
        public const string Returned = "Returned";
        public const string Overdue = "Overdue";
        public const string Lost = "Lost";
    }
}
