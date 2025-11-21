namespace inven_edu.Models.ViewModels
{
    /// <summary>
    /// View model for the admin dashboard
    /// </summary>
    public class AdminDashboardViewModel
    {
        public int TotalItems { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public int ActiveIssuances { get; set; }
        public int OverdueIssuances { get; set; }
        public int TotalUsers { get; set; }
        
        public List<IssuanceViewModel> RecentIssuances { get; set; } = new();
        public List<InventoryItemViewModel> LowStockItemsList { get; set; } = new();
    }

    /// <summary>
    /// View model for the user dashboard
    /// </summary>
    public class UserDashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public int TotalItemsIssued { get; set; }
        public int CurrentlyHolding { get; set; }
        public int TotalIssuances { get; set; }
        public int OverdueItems { get; set; }
        
        public List<IssuanceViewModel> RecentIssuances { get; set; } = new();
        public List<IssuanceViewModel> ActiveIssuances { get; set; } = new();
    }
}
