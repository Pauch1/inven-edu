using inven_edu.Models.Entities;

namespace inven_edu.Services
{
    /// <summary>
    /// Interface for inventory management operations
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Gets all inventory items
        /// </summary>
        Task<IEnumerable<InventoryItem>> GetAllItemsAsync();

        /// <summary>
        /// Gets an inventory item by ID
        /// </summary>
        Task<InventoryItem?> GetItemByIdAsync(int id);

        /// <summary>
        /// Creates a new inventory item
        /// </summary>
        Task<bool> CreateItemAsync(InventoryItem item);

        /// <summary>
        /// Updates an existing inventory item
        /// </summary>
        Task<bool> UpdateItemAsync(InventoryItem item);

        /// <summary>
        /// Deletes an inventory item
        /// </summary>
        Task<bool> DeleteItemAsync(int id);

        /// <summary>
        /// Gets all items with low stock levels
        /// </summary>
        Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync();

        /// <summary>
        /// Searches inventory items based on criteria
        /// </summary>
        Task<(IEnumerable<InventoryItem> items, int totalCount)> SearchItemsAsync(
            string? searchTerm, 
            int? categoryId, 
            bool? lowStockOnly, 
            bool? outOfStockOnly, 
            int pageNumber, 
            int pageSize);

        /// <summary>
        /// Checks if sufficient quantity is available for an item
        /// </summary>
        Task<bool> HasSufficientQuantityAsync(int itemId, int quantityNeeded);

        /// <summary>
        /// Updates item quantity (used during issuance and returns)
        /// </summary>
        Task<bool> UpdateItemQuantityAsync(int itemId, int quantityChange);
    }
}
