using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inven_edu.Models.Entities
{
    /// <summary>
    /// Represents an inventory item in the system
    /// </summary>
    public class InventoryItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for the inventory item
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the item name
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the item description
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the current quantity in stock
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the category ID for this item
        /// </summary>
        [Required]
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the minimum stock level for low stock alerts
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Minimum stock must be a positive number")]
        public int MinimumStock { get; set; }

        /// <summary>
        /// Gets or sets when the item was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets when the item was last updated
        /// </summary>
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property to the category
        /// </summary>
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        /// <summary>
        /// Navigation property for issuance records associated with this item
        /// </summary>
        public virtual ICollection<IssuanceRecord> IssuanceRecords { get; set; } = new List<IssuanceRecord>();

        /// <summary>
        /// Determines if the item is low in stock
        /// </summary>
        [NotMapped]
        public bool IsLowStock => Quantity <= MinimumStock;

        /// <summary>
        /// Determines if the item is out of stock
        /// </summary>
        [NotMapped]
        public bool IsOutOfStock => Quantity == 0;
    }
}
