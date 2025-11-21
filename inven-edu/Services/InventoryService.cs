using inven_edu.Data;
using inven_edu.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace inven_edu.Services
{
    /// <summary>
    /// Implementation of inventory management services
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(ApplicationDbContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<InventoryItem>> GetAllItemsAsync()
        {
            try
            {
                return await _context.InventoryItems
                    .Include(i => i.Category)
                    .OrderBy(i => i.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all inventory items");
                throw;
            }
        }

        public async Task<InventoryItem?> GetItemByIdAsync(int id)
        {
            try
            {
                return await _context.InventoryItems
                    .Include(i => i.Category)
                    .FirstOrDefaultAsync(i => i.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory item with ID {ItemId}", id);
                throw;
            }
        }

        public async Task<bool> CreateItemAsync(InventoryItem item)
        {
            try
            {
                item.CreatedDate = DateTime.UtcNow;
                item.UpdatedDate = DateTime.UtcNow;

                await _context.InventoryItems.AddAsync(item);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation("Created new inventory item: {ItemName}", item.Name);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory item: {ItemName}", item.Name);
                throw;
            }
        }

        public async Task<bool> UpdateItemAsync(InventoryItem item)
        {
            try
            {
                var existingItem = await _context.InventoryItems.FindAsync(item.Id);
                if (existingItem == null)
                {
                    _logger.LogWarning("Attempted to update non-existent item with ID {ItemId}", item.Id);
                    return false;
                }

                existingItem.Name = item.Name;
                existingItem.Description = item.Description;
                existingItem.Quantity = item.Quantity;
                existingItem.CategoryId = item.CategoryId;
                existingItem.MinimumStock = item.MinimumStock;
                existingItem.UpdatedDate = DateTime.UtcNow;

                _context.InventoryItems.Update(existingItem);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation("Updated inventory item: {ItemName} (ID: {ItemId})", item.Name, item.Id);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory item with ID {ItemId}", item.Id);
                throw;
            }
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                var item = await _context.InventoryItems.FindAsync(id);
                if (item == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent item with ID {ItemId}", id);
                    return false;
                }

                // Check if item has any issuance records
                var hasIssuances = await _context.IssuanceRecords.AnyAsync(ir => ir.ItemId == id);
                if (hasIssuances)
                {
                    _logger.LogWarning("Cannot delete item with ID {ItemId} because it has issuance records", id);
                    return false;
                }

                _context.InventoryItems.Remove(item);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted inventory item: {ItemName} (ID: {ItemId})", item.Name, id);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory item with ID {ItemId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<InventoryItem>> GetItemsByCategoryAsync(int categoryId)
        {
            try
            {
                return await _context.InventoryItems
                    .Include(i => i.Category)
                    .Where(i => i.CategoryId == categoryId)
                    .OrderBy(i => i.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items for category {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync()
        {
            try
            {
                return await _context.InventoryItems
                    .Include(i => i.Category)
                    .Where(i => i.Quantity <= i.MinimumStock && i.Quantity > 0)
                    .OrderBy(i => i.Quantity)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving low stock items");
                throw;
            }
        }

        public async Task<IEnumerable<InventoryItem>> GetOutOfStockItemsAsync()
        {
            try
            {
                return await _context.InventoryItems
                    .Include(i => i.Category)
                    .Where(i => i.Quantity == 0)
                    .OrderBy(i => i.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving out of stock items");
                throw;
            }
        }

        public async Task<(IEnumerable<InventoryItem> items, int totalCount)> SearchItemsAsync(
            string? searchTerm,
            int? categoryId,
            bool? lowStockOnly,
            bool? outOfStockOnly,
            int pageNumber,
            int pageSize)
        {
            try
            {
                var query = _context.InventoryItems
                    .Include(i => i.Category)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(i => i.Name.Contains(searchTerm) || 
                                           (i.Description != null && i.Description.Contains(searchTerm)));
                }

                if (categoryId.HasValue)
                {
                    query = query.Where(i => i.CategoryId == categoryId.Value);
                }

                if (lowStockOnly == true)
                {
                    query = query.Where(i => i.Quantity <= i.MinimumStock && i.Quantity > 0);
                }

                if (outOfStockOnly == true)
                {
                    query = query.Where(i => i.Quantity == 0);
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderBy(i => i.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (items, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching inventory items");
                throw;
            }
        }

        public async Task<bool> UpdateQuantityAsync(int itemId, int quantity)
        {
            try
            {
                var item = await _context.InventoryItems.FindAsync(itemId);
                if (item == null)
                {
                    _logger.LogWarning("Attempted to update quantity for non-existent item with ID {ItemId}", itemId);
                    return false;
                }

                item.Quantity = quantity;
                item.UpdatedDate = DateTime.UtcNow;

                _context.InventoryItems.Update(item);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation("Updated quantity for item {ItemName} to {Quantity}", item.Name, quantity);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quantity for item with ID {ItemId}", itemId);
                throw;
            }
        }

        public async Task<bool> HasSufficientQuantityAsync(int itemId, int requiredQuantity)
        {
            try
            {
                var item = await _context.InventoryItems.FindAsync(itemId);
                return item != null && item.Quantity >= requiredQuantity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking quantity for item with ID {ItemId}", itemId);
                throw;
            }
        }

        public async Task<bool> UpdateItemQuantityAsync(int itemId, int quantityChange)
        {
            try
            {
                var item = await _context.InventoryItems.FindAsync(itemId);
                if (item == null)
                {
                    _logger.LogWarning("Attempted to update quantity for non-existent item with ID {ItemId}", itemId);
                    return false;
                }

                item.Quantity += quantityChange;
                item.UpdatedDate = DateTime.UtcNow;

                _context.InventoryItems.Update(item);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation("Updated quantity for item {ItemName} by {QuantityChange}", item.Name, quantityChange);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quantity for item with ID {ItemId}", itemId);
                throw;
            }
        }
    }
}
