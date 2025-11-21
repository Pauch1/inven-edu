using System.ComponentModel.DataAnnotations;

namespace inven_edu.Models.ViewModels
{
    /// <summary>
    /// View model for displaying and editing inventory items
    /// </summary>
    public class InventoryItemViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(200)]
        [Display(Name = "Item Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Category")]
        public string? CategoryName { get; set; }

        [Required(ErrorMessage = "Minimum stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Minimum stock must be a positive number")]
        [Display(Name = "Minimum Stock Level")]
        public int MinimumStock { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime? UpdatedDate { get; set; }

        [Display(Name = "Low Stock")]
        public bool IsLowStock { get; set; }

        [Display(Name = "Out of Stock")]
        public bool IsOutOfStock { get; set; }
    }

    /// <summary>
    /// View model for searching inventory items
    /// </summary>
    public class InventorySearchViewModel
    {
        public List<InventoryItemViewModel> Items { get; set; } = new();
        public List<CategoryViewModel> Categories { get; set; } = new();
        
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }
        
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        
        [Display(Name = "Low Stock Only")]
        public bool? LowStockOnly { get; set; }
        
        [Display(Name = "Out of Stock Only")]
        public bool? OutOfStockOnly { get; set; }
        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
