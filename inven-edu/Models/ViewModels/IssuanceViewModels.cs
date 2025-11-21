using System.ComponentModel.DataAnnotations;

namespace inven_edu.Models.ViewModels
{
    /// <summary>
    /// View model for displaying issuance records
    /// </summary>
    public class IssuanceViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Item")]
        public string ItemName { get; set; } = string.Empty;

        [Display(Name = "User")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string UserEmail { get; set; } = string.Empty;

        [Display(Name = "Quantity Issued")]
        public int QuantityIssued { get; set; }

        [Display(Name = "Issued Date")]
        public DateTime IssuedDate { get; set; }

        [Display(Name = "Expected Return Date")]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Overdue")]
        public bool IsOverdue { get; set; }
    }

    /// <summary>
    /// View model for creating a new issuance
    /// </summary>
    public class CreateIssuanceViewModel
    {
        [Required(ErrorMessage = "Please select an item")]
        [Display(Name = "Item")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Please select a user")]
        [Display(Name = "Issue To")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        [Display(Name = "Quantity")]
        public int QuantityIssued { get; set; }

        [Display(Name = "Expected Return Date")]
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        [StringLength(1000)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        // For populating dropdowns
        public List<InventoryItemViewModel> AvailableItems { get; set; } = new();
        public List<UserViewModel> AvailableUsers { get; set; } = new();
        public int? AvailableQuantity { get; set; }
    }

    /// <summary>
    /// View model for searching issuance records
    /// </summary>
    public class IssuanceSearchViewModel
    {
        public List<IssuanceViewModel> Issuances { get; set; } = new();
        
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }
        
        [Display(Name = "Status")]
        public string? Status { get; set; }
        
        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }
        
        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }
        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }

    /// <summary>
    /// View model for material request by users
    /// </summary>
    public class MaterialRequestViewModel
    {
        [Required(ErrorMessage = "Please select an item")]
        [Display(Name = "Item")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        [Display(Name = "Quantity Requested")]
        public int QuantityRequested { get; set; }

        [Required(ErrorMessage = "Purpose is required")]
        [StringLength(1000)]
        [Display(Name = "Purpose/Reason")]
        public string Purpose { get; set; } = string.Empty;

        [Display(Name = "Expected Return Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpectedReturnDate { get; set; }

        // For populating dropdown
        public List<InventoryItemViewModel> AvailableItems { get; set; } = new();
    }
}
