using System.ComponentModel.DataAnnotations;

namespace inven_edu.Models.Entities
{
    /// <summary>
    /// Represents a category for organizing inventory items
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Gets or sets the unique identifier for the category
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the category name
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category description
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets when the category was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property for inventory items in this category
        /// </summary>
        public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    }
}
